#if UNITY_EDITOR && UNITY_EDITOR_WIN
namespace EasyConnectADB
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using System.Collections.Generic;
    using Process = System.Diagnostics.Process;
    using Debug = UnityEngine.Debug;

    public class MobileADBConnect : EditorWindow
    {
        const string K_ADB_PATH = "AdbWifi.adbPath";
        const string K_PHONE_IP = "AdbWifi.phoneIp";
        const string K_PAIR_PORT = "AdbWifi.pairPort";
        const string K_PAIR_CODE = "AdbWifi.pairCode";
        const string K_WIRELESS_PORT = "AdbWifi.wirelessPort";

        const int TIMEOUT_CONNECT_MS = 7000;
        const int TIMEOUT_PAIR_MS = 15000;
        const int TIMEOUT_GENERIC_MS = 8000;

        string adbPath;
        string phoneIp = "192.168.0.23";
        const string port5555 = "5555";

        bool usbFoldout = false;
        bool wifiFoldout = false;

        string port2;
        string pairCode;
        string port1;

        // LOGCAT
        Process logcatProc;
        bool logcatRunning;
        readonly StringBuilder logcatBuf = new StringBuilder(64_000);
        enum LogcatPreset { UnityAndCrashes, CrashesOnly, VerboseAll, ThisAppOnly }
        LogcatPreset logcatPreset = LogcatPreset.UnityAndCrashes;
        bool logcatDoNotClear = true;       // не чистить буфер перед стартом
        string packageNameOverride = "";    // если хочешь указать пакет вручную

        StringBuilder log = new StringBuilder(4096);
        Vector2 scroll;
        [NonSerialized] GUIStyle bigBtn;
        GUIStyle BigBtn
        {
            get
            {
                if (bigBtn == null)
                {
                    var s = new GUIStyle(GUI.skin.button);
                    s.fontSize = 16;
                    s.fixedHeight = 46;
                    s.alignment = TextAnchor.MiddleCenter;
                    s.stretchWidth = true;
                    bigBtn = s;
                }
                return bigBtn;
            }
        }

        [MenuItem("Tools/Connect Mobile by ADB/Menu", false, 1)]
        public static void ShowWindow()
        {
            var w = GetWindow<MobileADBConnect>("Connect Mobile by ADB");
            w.minSize = new Vector2(560, 300);
        }

        [MenuItem("Tools/Connect Mobile by ADB/Fast Connect", false, 2)]
        public static void SmartConnectMenu()
        {
            string ip = EditorPrefs.GetString(K_PHONE_IP, "");
            string wprt = EditorPrefs.GetString(K_WIRELESS_PORT, "");
            string pprt = EditorPrefs.GetString(K_PAIR_PORT, "");
            string pcod = EditorPrefs.GetString(K_PAIR_CODE, "");
            string adb = EditorPrefs.GetString(K_ADB_PATH, "");

            bool ok = SmartConnectHeadless(ip, wprt, pprt, pcod, adb, out string report);
            if (ok) Debug.Log($"[ADB Wi-Fi] {report}");
            else Debug.LogError($"[ADB Wi-Fi] {report}");
        }

        void OnEnable()
        {
            adbPath = EditorPrefs.GetString(K_ADB_PATH, "");
            phoneIp = EditorPrefs.GetString(K_PHONE_IP, phoneIp);
            port2 = EditorPrefs.GetString(K_PAIR_PORT, port2);
            pairCode = EditorPrefs.GetString(K_PAIR_CODE, pairCode);
            port1 = EditorPrefs.GetString(K_WIRELESS_PORT, port1);

            if (string.IsNullOrEmpty(adbPath))
                adbPath = TryAutoFindAdb();

            AssemblyReloadEvents.beforeAssemblyReload += StopLogcatSafe;
        }

        void OnDisable()
        {
            EditorPrefs.SetString(K_ADB_PATH, adbPath ?? "");
            EditorPrefs.SetString(K_PHONE_IP, phoneIp ?? "");
            EditorPrefs.SetString(K_PAIR_PORT, port2 ?? "");
            EditorPrefs.SetString(K_PAIR_CODE, pairCode ?? "");
            EditorPrefs.SetString(K_WIRELESS_PORT, port1 ?? "");

            StopLogcatSafe();
            AssemblyReloadEvents.beforeAssemblyReload -= StopLogcatSafe;
        }

        void OnGUI()
        {
            // ADB Path
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.BeginHorizontal();
#if RU_YG2
                string pathAdbStr = "Путь к ADB (adb.exe)";
                string reviewStr = "обзор...";
                string findAdbStr = "Найти ADB";
#else
                string pathAdbStr = "ADB Path (adb.exe)";
                string reviewStr = "review...";
                string findAdbStr = "To find ADB";
#endif
                adbPath = EditorGUILayout.TextField(pathAdbStr, adbPath ?? "");
                if (GUILayout.Button(reviewStr, GUILayout.Width(80)))
                {
                    string p = EditorUtility.OpenFilePanel("Specify adb.exe", "", "exe");
                    if (!string.IsNullOrEmpty(p)) adbPath = p;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(findAdbStr, GUILayout.Width(120)))
                    Enqueue(() => { adbPath = TryAutoFindAdb(); Append($"[INFO] ADB: {(string.IsNullOrEmpty(adbPath) ? "not found" : adbPath)}\n"); });
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            // IP Device
            GUILayout.Space(6);
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                phoneIp = EditorGUILayout.TextField(new GUIContent("Device IP"), phoneIp ?? "");

            // USB activation
#if RU_YG2
            usbFoldout = EditorGUILayout.Foldout(usbFoldout, "Активация по USB кабелю");
#else
            usbFoldout = EditorGUILayout.Foldout(usbFoldout, "Activation via USB cable");
#endif
            if (usbFoldout)
            {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
#if RU_YG2
                    EditorGUILayout.HelpBox("Чтобы активировать WIFI соединение с помощью подключения по USB кабелю: подключить телефон по проводу, включите USB отладку, выберите режим подключения - передача файлов и нажмите кнопку ниже.", MessageType.None);

                    if (GUILayout.Button("Активировать WIFI соединение по USB кабелю"))
                        Enqueue(() => { if (!UsbTo5555ThenConnect()) Error("USB-устройство не найдено/не авторизовано."); });
#else
                    EditorGUILayout.HelpBox("To activate the WIFI connection using a USB cable connection: connect the phone by wire, turn on USB debugging, select the connection mode - file transfer and click the button below.", MessageType.None);

                    if (GUILayout.Button("Activate WIFI connection via USB cable"))
                        Enqueue(() => { if (!UsbTo5555ThenConnect()) Error("The USB device was not found/authorized."); });
#endif
                }
            }

            // Wi-Fi activation
#if RU_YG2
            wifiFoldout = EditorGUILayout.Foldout(wifiFoldout, "Активация по Wi-Fi (для Android 11+)");
#else
            wifiFoldout = EditorGUILayout.Foldout(wifiFoldout, "Wi-Fi activation (for Android 11+)");
#endif
            if (wifiFoldout)
            {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
#if RU_YG2
                    EditorGUILayout.HelpBox(
                        "1) Настройки телефона → Developer options → Wireless debugging.\n" +
                        "2) В строку Port 1 введите текущий порт из (IP-адрес и порт)\n" +
                        "3) Нажмите (Подключить устройство с помощью кода подключения)\n" +
                        "4) В строку Port 2 введите порт указанный под кодом.\n" +
                        "5) В строку Code введите код подключения.\n" +
                        "6) Нажмите на кнопку ниже.",
                        MessageType.None);
#else
                    EditorGUILayout.HelpBox(
                        "1) Phone: Developer Options → Wireless Debugging.\n" +
                        "2) In the Port 1 line, enter the current port from (IP address & Port)\n" +
                        "3) Click (Pair device with pairing code)\n" +
                        "3) In the Port 2 line, enter the port specified under the code.\n" +
                        "4) Enter the connection code in the Code line.\n" +
                        "6) Click on the button below.",
                        MessageType.None);
#endif


                    port1 = EditorGUILayout.TextField(new GUIContent("Port 1"), port1 ?? "");
                    port2 = EditorGUILayout.TextField(new GUIContent("Port 2"), port2 ?? "");
                    pairCode = EditorGUILayout.TextField(new GUIContent("Code"), pairCode ?? "");

                    EditorGUILayout.BeginHorizontal();
#if RU_YG2
                    string wiwiConnectStr = "Активировать Wi-Fi соединение и запомнить устройство";
#else
                    string wiwiConnectStr = "Activate the Wi-Fi connection and remember the device";
#endif
                    if (GUILayout.Button(wiwiConnectStr))
                    {
                        Enqueue(() => { if (!WifiPairConnectSwitch()) Error("Check IP/portd/code."); });
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            // Connect button
            GUILayout.Space(4);
#if RU_YG2
            if (GUILayout.Button("ПОДКЛЮЧИТЬСЯ", BigBtn))
                Enqueue(SmartConnect);
            EditorGUILayout.HelpBox("Активация может слететь после перезагрузки телефона. В таком случае её необходимо произвести заново.", MessageType.None);
#else
            if (GUILayout.Button("CONNECT", BigBtn))
                Enqueue(SmartConnect);
            EditorGUILayout.HelpBox("Activation may fail after restarting the phone. In this case, it must be done again.", MessageType.None);
#endif

            // Height of the bottom panel
            const float footerH = 38f;

            // Log (scroll)
            GUILayout.Space(8);
            GUILayout.Label("Log", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
            {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
                    using (var sv = new EditorGUILayout.ScrollViewScope(scroll, GUILayout.ExpandHeight(true)))
                    {
                        scroll = sv.scrollPosition;

                        // Всё как раньше, но теперь можно выделять текст мышкой
                        GUIStyle selectable = new GUIStyle(EditorStyles.wordWrappedLabel)
                        {
                            richText = false
                        };

                        // Обрабатываем события выделения
                        Rect rect = GUILayoutUtility.GetRect(
                            new GUIContent(log.ToString()),
                            selectable,
                            GUILayout.ExpandWidth(true),
                            GUILayout.ExpandHeight(true)
                        );

                        EditorGUI.SelectableLabel(rect, log.ToString(), selectable);
                    }
                }


                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.Height(footerH)))
                {
#if RU_YG2
                    string clearStr = "Очистить лог";
                    string devicesStr = "Показать устройства";
                    string disconnectStr = "Отсоединить";
                    string versionAdbStr = "Версия ADB";
                    string reloadAdbStr = "Перезапустить ADB";
#else
                    string clearStr = "Clear log";
                    string devicesStr = "Show devices";
                    string disconnectStr = "Disconnect";
                    string versionAdbStr = "ADB version";
                    string reloadAdbStr = "Restart ADB";
#endif
                    if (GUILayout.Button(clearStr, GUILayout.Height(24))) { log.Length = 0; Repaint(); }
                    if (GUILayout.Button(devicesStr, GUILayout.Height(24))) Enqueue(() => RunAdb("devices"));
                    if (GUILayout.Button(disconnectStr, GUILayout.Height(24))) Enqueue(() => RunAdb("disconnect"));
                    if (GUILayout.Button(versionAdbStr, GUILayout.Height(24))) Enqueue(() => RunAdb("version"));
                    if (GUILayout.Button(reloadAdbStr, GUILayout.Height(24))) Enqueue(() => { RunAdb("kill-server"); RunAdb("start-server"); });
                    GUILayout.FlexibleSpace();
                }

                GUILayout.Space(6);
                using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                {
#if RU_YG2
                    GUILayout.Label("Настройки логов (logcat)", EditorStyles.boldLabel);
                    string doNotClearStr = "Не очищать буфер при старте";
                    string pkgLabel = "Пакет приложения";
                    string pkgOverrideLabel = "Package override (опционально)";
#else
                    GUILayout.Label("Logcat settings", EditorStyles.boldLabel);
                    string doNotClearStr = "Do not clear buffer on start";
                    string pkgLabel = "App package";
                    string pkgOverrideLabel = "Package override (optional)";
#endif
                    logcatPreset = (LogcatPreset)EditorGUILayout.EnumPopup("Logcat preset", logcatPreset);
                    logcatDoNotClear = EditorGUILayout.ToggleLeft(doNotClearStr, logcatDoNotClear);

#if UNITY_ANDROID
                    if (string.IsNullOrEmpty(packageNameOverride))
                        EditorGUILayout.LabelField(pkgLabel, UnityEditor.PlayerSettings.applicationIdentifier);
                    packageNameOverride = EditorGUILayout.TextField(new GUIContent(pkgOverrideLabel), packageNameOverride);
#endif
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("Start Logcat", GUILayout.Height(24))) Enqueue(StartLogcat);
                            if (GUILayout.Button("Stop Logcat", GUILayout.Height(24))) Enqueue(StopLogcatSafe);
                            if (GUILayout.Button("Save Log", GUILayout.Height(24))) Enqueue(SaveLogToFile);
                        }

                        GUILayout.FlexibleSpace();

                        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(270)))
                        {
                            if (GUILayout.Button("Last Crash Report", GUILayout.Height(24))) Enqueue(CrashDumpOnce);
                            if (GUILayout.Button("Launch App", GUILayout.Height(24))) Enqueue(LaunchAndTailAll);
                        }
                    }

                }

            }
        }

        #region Logic

        void SmartConnect()
        {
            if (string.IsNullOrWhiteSpace(phoneIp)) { Error("First, specify the phone's IP address."); return; }

            Info("== Smart Connect: we're trying it out IP:5555, then USB, then Wi-Fi ==");
            if (Connect5555()) { RunAdb("devices"); return; }
            if (UsbTo5555ThenConnect()) { RunAdb("devices"); return; }
            if (!string.IsNullOrEmpty(port1) && WifiConnectThenSwitchTo5555()) { RunAdb("devices"); return; }
            Error("Couldn't connect. Check the IP/USB or specify the Wi-Fi port (Android 11+).");
        }

        bool WifiPairConnectSwitch()
        {
            if (string.IsNullOrWhiteSpace(phoneIp) || string.IsNullOrWhiteSpace(port1))
            {
                Error("Specify the IP and Wi-Fi connect port (Port 1).");
                return false;
            }

            // 1) We are trying to connect to the CONNECT port (Port 1)
            RunAdb($"connect {phoneIp}:{port1}");

            // 2) If it didn't work out, do a PAIR on Port 2 and CONNECT again on Port 1
            if (!DevicesContain($"{phoneIp}:{port1}"))
            {
                if (!string.IsNullOrWhiteSpace(port2) && !string.IsNullOrWhiteSpace(pairCode))
                {
                    RunAdb($"pair {phoneIp}:{port2} {pairCode}");
                    RunAdb($"connect {phoneIp}:{port1}");
                }
            }

            if (!DevicesContain($"{phoneIp}:{port1}"))
                return false;

            // 3) Switch adbd to 5555 and connect
            RunAdb($"-s {phoneIp}:{port1} tcpip 5555");
            RunAdb($"connect {phoneIp}:5555");

            return DevicesContain($"{phoneIp}:5555");
        }

        bool Connect5555()
        {
            Info($"adb connect {phoneIp}:{port5555}");
            RunAdb($"connect {phoneIp}:{port5555}");
            return DevicesContain($"{phoneIp}:{port5555}");
        }

        bool UsbTo5555ThenConnect()
        {
            var usb = GetFirstUsbDeviceSerial();
            if (string.IsNullOrEmpty(usb))
            {
                Info("USB-device was not found. Insert the cable and allow debugging.");
                return false;
            }
            Info($"USB: {usb} → tcpip 5555");
            RunAdb($"-s {usb} tcpip 5555");
            return Connect5555();
        }

        bool WifiConnectThenSwitchTo5555()
        {
            if (string.IsNullOrWhiteSpace(phoneIp)) { Error("First, specify the phone's IP address."); return false; }
            if (string.IsNullOrWhiteSpace(port1)) { Error("The Wi-Fi connect port (Port 1) is not specified."); return false; }

            Info($"Wi-Fi connect: {phoneIp}:{port1}");
            RunAdb($"connect {phoneIp}:{port1}");
            if (!DevicesContain($"{phoneIp}:{port1}"))
            {
                if (!string.IsNullOrEmpty(port2) && !string.IsNullOrEmpty(pairCode))
                {
                    Info("Trying Pair then Connect...");
                    RunAdb($"pair {phoneIp}:{port2} {pairCode}");
                    RunAdb($"connect {phoneIp}:{port1}");
                }
            }
            if (!DevicesContain($"{phoneIp}:{port1}")) return false;

            RunAdb($"-s {phoneIp}:{port1} tcpip 5555");
            return Connect5555();
        }

        // AUXILIARY
        void Enqueue(Action action)
        {
            // Performing an action after the current GUI event
            EditorApplication.delayCall += () =>
            {
                try { action?.Invoke(); }
                catch (Exception e) { Error(e.Message); }
                Repaint();
            };

            // Important: we don't log ExitGUIException — we just log out of the IMGUI loop correctly.
            GUIUtility.ExitGUI();
        }

        string GetFirstUsbDeviceSerial()
        {
            string outp = RunAdbSilent("devices");
            if (string.IsNullOrEmpty(outp)) return null;
            var lines = outp.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var ln in lines)
            {
                if (ln.StartsWith("List of devices")) continue;
                var parts = ln.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    string serial = parts[0];
                    string state = parts[1];
                    if (state.Equals("device", StringComparison.OrdinalIgnoreCase) && !serial.Contains(":"))
                        return serial; // USB serial number
                }
            }
            return null;
        }

        bool DevicesContain(string needle)
        {
            string outp = RunAdbSilent("devices");
            return outp != null && outp.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        string TryAutoFindAdb()
        {
            var candidates = new List<string>();

            string sdk = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            if (string.IsNullOrEmpty(sdk))
                sdk = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (!string.IsNullOrEmpty(sdk))
                candidates.Add(Path.Combine(sdk, "platform-tools", "adb.exe"));

            string localSdk = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Android", "Sdk", "platform-tools", "adb.exe");
            candidates.Add(localSdk);

            string progFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            candidates.Add(Path.Combine(progFiles, "Android", "Android Studio", "platform-tools", "adb.exe"));

            foreach (var c in candidates)
                if (File.Exists(c)) return c;

            return ""; // empty = we will call "adb" from the PATH
        }

        void Info(string msg) => Append($"[INFO] {msg}\n");
        void Error(string msg) => Append($"[ERROR] {msg}\n");
        void Append(string s) { log.Append(s ?? ""); }

        int GetTimeoutForArgs(string args)
        {
            if (string.IsNullOrEmpty(args)) return TIMEOUT_GENERIC_MS;
            string a = args.Trim().ToLowerInvariant();

            if (a.StartsWith("connect ") || a == "disconnect" || a.StartsWith("disconnect "))
                return TIMEOUT_CONNECT_MS;

            if (a.StartsWith("pair "))
                return TIMEOUT_PAIR_MS;

            return TIMEOUT_GENERIC_MS;
        }

        string RunAdbSilent(string args)
        {
            string exe = adbPath;
            bool useDirect = !string.IsNullOrEmpty(exe) && File.Exists(exe);

            var stdoutSb = new StringBuilder();
            var stderrSb = new StringBuilder();
            int timeoutMs = GetTimeoutForArgs(args);

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = useDirect ? exe : "adb",
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                };

                using (var p = new Process { StartInfo = psi, EnableRaisingEvents = false })
                {
                    p.OutputDataReceived += (_, e) => { if (e.Data != null) stdoutSb.AppendLine(e.Data); };
                    p.ErrorDataReceived += (_, e) => { if (e.Data != null) stderrSb.AppendLine(e.Data); };

                    if (!p.Start())
                        throw new InvalidOperationException("Couldn't start adb. Check the path.");

                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    if (!p.WaitForExit(timeoutMs))
                    {
                        try { p.Kill(); } catch { /* ignore */ }
                        Append($"   (stderr) [TIMEOUT] adb {args}\n");
                        return null;
                    }

                    p.WaitForExit();
                }

                string stdout = stdoutSb.ToString();
                string stderr = stderrSb.ToString();

                if (!string.IsNullOrEmpty(stderr))
                    Append($"   (stderr) {stderr}\n");

                return stdout;
            }
            catch (Exception e)
            {
                Error(e.Message);
                return null;
            }
        }

        void RunAdb(string args)
        {
            Append($"> adb {args}\n");
            string stdout = RunAdbSilent(args);
            if (!string.IsNullOrEmpty(stdout))
                Append(stdout + "\n");
        }

        // Headless
        static bool SmartConnectHeadless(
            string ip, string wifiPort, string pairPort, string pairCode, string adbPathPref,
            out string report)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                report = "The phone's IP is not set (open the ADB Wi-Fi window and specify the IP).";
                return false;
            }

            // local helpers
            string adbExe = ResolveAdb(adbPathPref);
            string Out(string args) => RunAdbOnce(adbExe, args, out _);

            // 1) direct attempt to 5555
            Out($"connect {ip}:5555");
            string devs = Out("devices");
            if (devs.IndexOf($"{ip}:5555", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                report = $"Connected to {ip}:5555";
                return true;
            }

            // 2) let's try via USB: find the first USB serial number
            string list = Out("devices");
            string usb = ParseFirstUsbSerial(list);
            if (!string.IsNullOrEmpty(usb))
            {
                Out($"-s {usb} tcpip 5555");
                Out($"connect {ip}:5555");
                devs = Out("devices");
                if (devs.IndexOf($"{ip}:5555", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    report = $"USB → tcpip 5555 → Connected to {ip}:5555";
                    return true;
                }
            }

            // 3) if the Wi-Fi port is set, we will connect and switch to 5555
            if (!string.IsNullOrEmpty(wifiPort))
            {
                Out($"connect {ip}:{wifiPort}");
                devs = Out("devices");
                if (devs.IndexOf($"{ip}:{wifiPort}", StringComparison.OrdinalIgnoreCase) < 0 && !string.IsNullOrEmpty(pairPort) && !string.IsNullOrEmpty(pairCode))
                {
                    Out($"pair {ip}:{pairPort} {pairCode}");
                    Out($"connect {ip}:{wifiPort}");
                    devs = Out("devices");
                }
                if (devs.IndexOf($"{ip}:{wifiPort}", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Out($"-s {ip}:{wifiPort} tcpip 5555");
                    Out($"connect {ip}:5555");
                    devs = Out("devices");
                    if (devs.IndexOf($"{ip}:5555", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        report = $"Wi-Fi порт {wifiPort} → tcpip 5555 → Connected to {ip}:5555";
                        return true;
                    }
                }
            }

            report = "Smart Connect couldn't connect. Check the IP/USB or Wi-Fi port (Android 11+).";
            return false;
        }

        static string ResolveAdb(string pref)
        {
            if (!string.IsNullOrEmpty(pref) && File.Exists(pref)) return pref;

            // ANDROID_SDK_ROOT / ANDROID_HOME
            string sdk = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
            if (string.IsNullOrEmpty(sdk))
                sdk = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (!string.IsNullOrEmpty(sdk))
            {
                string p = Path.Combine(sdk, "platform-tools", "adb.exe");
                if (File.Exists(p)) return p;
            }

            string local = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                        "Android", "Sdk", "platform-tools", "adb.exe");
            if (File.Exists(local)) return local;

            string studio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                         "Android", "Android Studio", "platform-tools", "adb.exe");
            if (File.Exists(studio)) return studio;

            return "adb"; // we hope that in the PATH
        }

        static string RunAdbOnce(string adbExe, string args, out string stderr)
        {
            stderr = null;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = adbExe,
                    Arguments = args,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                };
                using (var p = Process.Start(psi))
                {
                    string so = p.StandardOutput.ReadToEnd();
                    string se = p.StandardError.ReadToEnd();
                    p.WaitForExit();
                    stderr = se;
                    return so;
                }
            }
            catch (Exception e)
            {
                return $"[ERROR running adb {args}] {e.Message}";
            }
        }

        static string ParseFirstUsbSerial(string adbDevicesOutput)
        {
            if (string.IsNullOrEmpty(adbDevicesOutput)) return null;
            var lines = adbDevicesOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var ln in lines)
            {
                if (ln.StartsWith("List of devices")) continue;
                var parts = ln.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    string serial = parts[0];
                    string state = parts[1];
                    if (state.Equals("device", StringComparison.OrdinalIgnoreCase) && !serial.Contains(":"))
                        return serial;
                }
            }
            return null;
        }


        // LOGCAT
        void StartLogcat()
        {
            if (logcatRunning) { Info("Logcat already running."); return; }

            string serial = GetPreferredDevice();
            if (string.IsNullOrEmpty(serial))
            {
                Error("No active device. Connect by USB/Wi-Fi first (adb devices must show 'device').");
                return;
            }

            if (!logcatDoNotClear)
                RunAdbSilent($"-s {serial} logcat -c");

            string filterArgs = BuildFilter(serial);
            string adbExe = ResolveAdb(adbPath);
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = adbExe,
                    Arguments = $"-s {serial} logcat {filterArgs}",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };
                logcatProc = new Process { StartInfo = psi, EnableRaisingEvents = true };
                logcatProc.OutputDataReceived += (_, e) => { if (e.Data != null) AppendLogcatLine(e.Data); };
                logcatProc.ErrorDataReceived += (_, e) => { if (e.Data != null) AppendLogcatLine("[stderr] " + e.Data); };

                if (!logcatProc.Start()) { Error("Failed to start adb logcat process."); return; }
                logcatProc.BeginOutputReadLine();
                logcatProc.BeginErrorReadLine();
                logcatRunning = true;
                Info($"Logcat started on {serial}: {filterArgs}");
            }
            catch (Exception ex)
            {
                Error("Logcat start error: " + ex.Message);
                StopLogcatSafe();
            }
        }

        void StopLogcatSafe()
        {
            if (!logcatRunning) return;
            try
            {
                if (logcatProc != null && !logcatProc.HasExited)
                {
                    try { logcatProc.Kill(); } catch { /* ignore */ }
                    try { logcatProc.Dispose(); } catch { }
                }
            }
            finally
            {
                logcatProc = null;
                logcatRunning = false;
                Info("Logcat stopped.");
            }
        }

        void AppendLogcatLine(string line)
        {
            const int MAX_LEN = 120_000;
            logcatBuf.AppendLine(line);
            if (logcatBuf.Length > MAX_LEN)
                logcatBuf.Remove(0, logcatBuf.Length - MAX_LEN);

            Append(line + "\n");
            Repaint();
        }

        void SaveLogToFile()
        {
            var path = EditorUtility.SaveFilePanel("Save filtered logcat", "", "logcat_unity.txt", "txt");
            if (string.IsNullOrEmpty(path)) return;
            File.WriteAllText(path, logcatBuf.ToString(), Encoding.UTF8);
            Info("Saved: " + path);
        }

        string GetPreferredDevice()
        {
            string wifiSerial = string.IsNullOrWhiteSpace(phoneIp) ? null : $"{phoneIp}:5555";
            string outp = RunAdbSilent("devices");
            if (string.IsNullOrEmpty(outp)) return null;

            string chosen = null;
            var lines = outp.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var ln in lines)
            {
                if (ln.StartsWith("List of devices")) continue;
                var parts = ln.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;
                string serial = parts[0];
                string state = parts[1];
                if (!state.Equals("device", StringComparison.OrdinalIgnoreCase)) continue;

                if (!string.IsNullOrEmpty(wifiSerial) && serial.Equals(wifiSerial, StringComparison.OrdinalIgnoreCase))
                    return serial;

                if (chosen == null && !serial.Contains(":")) chosen = serial;
                else if (chosen == null) chosen = serial;
            }
            return chosen;
        }

        string BuildFilter(string serial)
        {
            switch (logcatPreset)
            {
                case LogcatPreset.CrashesOnly:
                    return "-v time AndroidRuntime:E ActivityManager:E libc:E System.err:E *:S";
                case LogcatPreset.VerboseAll:
                    return "-v time -b main -b system -b events -b crash *:V";
                case LogcatPreset.ThisAppOnly:
                    string pkg = packageNameOverride;
#if UNITY_ANDROID
                    if (string.IsNullOrEmpty(pkg)) pkg = UnityEditor.PlayerSettings.applicationIdentifier;
#endif
                    if (string.IsNullOrEmpty(pkg))
                        return "-v time Unity:* Yandex:* AndroidRuntime:E *:S";
                                                                              
                    string pid = RunAdbSilent($"-s {serial} shell pidof {pkg}")?.Trim();
                    if (string.IsNullOrEmpty(pid))
                    {
                        Info($"PID for {pkg} not found yet. Start the app and restart Logcat.");
                        return "-v time Unity:* Yandex:* AndroidRuntime:E *:S";
                    }
                    return $"-v time --pid {pid}";
                default:
                    return "-v time Unity:* Yandex:* AndroidRuntime:E *:S";
            }
        }

        void CrashDumpOnce()
        {
            string serial = GetPreferredDevice();
            if (string.IsNullOrEmpty(serial)) { Error("No device for crash dump."); return; }
            string dump = RunAdbSilent($"-s {serial} logcat -b crash -v time -t 200");
            if (string.IsNullOrEmpty(dump)) { Info("Crash buffer is empty."); return; }
            AppendLogcatLine("=== CRASH BUFFER BEGIN ===");
            AppendLogcatLine(dump);
            AppendLogcatLine("=== CRASH BUFFER END ===");
        }

        void LaunchAndTailAll()
        {
            string serial = GetPreferredDevice();
            if (string.IsNullOrEmpty(serial)) { Error("No active device."); return; }

            logcatDoNotClear = true;
            logcatPreset = LogcatPreset.VerboseAll;

            string pkg = packageNameOverride;
#if UNITY_ANDROID
            if (string.IsNullOrEmpty(pkg)) pkg = UnityEditor.PlayerSettings.applicationIdentifier;
#endif
            if (string.IsNullOrEmpty(pkg)) { Error("Package name is empty. Set Package override."); return; }

            StartLogcat();

            RunAdb($"-s {serial} shell monkey -p {pkg} -c android.intent.category.LAUNCHER 1");
        }
        #endregion
    }
}
#endif
