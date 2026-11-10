using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.Save_Policies;
using Assets.Source.Scripts.YG;
using System.Collections.Generic;
using UnityEngine;

namespace YG
{
    public partial class SavesYG
    {
        private const float InitialVolume = 0.5f;

        public int MainPoints;
        public int ActualRoundNumber;
        public int SavedGameMode = 0;
        public float MasterVolume = InitialVolume;
        public float AmbientVolume = InitialVolume;
        public float EffectVolume = InitialVolume;
        public string EquippedItem;
        public string SavedLanguage;
        public List<string> PurchasedItemIdentifiers = new();
        public List<RoundRatingEntry> RoundRatingEntries = new();
        private SaveCoordinator _saveCoordinator;
        private GameModeSection _gameModeSection;
        private ScoreSection _scoreSection;
        private RoundSection _roundSection;
        private AudioSettingsSection _audioSettingsSection;
        private InventorySection _inventorySection;
        private LanguageSection _languageSection;
        private RoundRatingSection _roundRatingSection;

        public GameModeSection GameMode => _gameModeSection;
        public ScoreSection Score => _scoreSection;
        public RoundSection Round => _roundSection;
        public AudioSettingsSection AudioSettings => _audioSettingsSection;
        public InventorySection Inventory => _inventorySection;
        public LanguageSection Language => _languageSection;
        public RoundRatingSection RoundRatings => _roundRatingSection;

        public int Points => Score.Points;
        public string EquippedItemIdentifier => Inventory.EquippedItemIdentifier;

        public void Initialize()
        {
            if (_saveCoordinator != null)
                return;

            var immediateSavePolicy = new ImmediateSavePolicy();
            var debouncedSavePolicy = new DebouncedSavePolicy();

            _gameModeSection = new GameModeSection(this, immediateSavePolicy);
            _scoreSection = new ScoreSection(this, immediateSavePolicy);
            _roundSection = new RoundSection(this, immediateSavePolicy);
            _inventorySection = new InventorySection(this, immediateSavePolicy);
            _languageSection = new LanguageSection(this, immediateSavePolicy);
            _roundRatingSection = new RoundRatingSection(this, immediateSavePolicy);
            _audioSettingsSection = new AudioSettingsSection(this, debouncedSavePolicy);

            _saveCoordinator = new SaveCoordinator(new BaseSection[]
            {
                _gameModeSection,
                _scoreSection,
                _roundSection,
                _audioSettingsSection,
                _inventorySection,
                _languageSection,
                _roundRatingSection
            });

            _saveCoordinator.RebuildAll();
        }

        public void EnableAutoSaveOnExit()
            => Application.quitting += OnApplicationQuit;

        public void DisableAutoSaveOnExit()
            => Application.quitting -= OnApplicationQuit;

        public void ForceSaveAudio()
            => _audioSettingsSection?.Flush();
        public void Shutdown()
        {
            Application.quitting -= OnApplicationQuit;

            _saveCoordinator?.Dispose();
            _saveCoordinator = null;
        }

        private void OnApplicationQuit()
            => _saveCoordinator?.FlushAll();
    }
}

//namespace YG
//{
//    public partial class SavesYG
//    {
//        private const float BeginVolume = 0.5f;

//        public int MainPoints;
//        public int ActualRoundNumber;
//        public int SavedGameMode = 0;
//        public float MasterVolume = BeginVolume;
//        public float AmbientVolume = BeginVolume;
//        public float EffectVolume = BeginVolume;
//        public string EquippedItem;
//        public string SavedLanguage;
//        public List<string> ItemIDs = new();
//        public List<RoundRatingEntry> RoundRatingEntries = new();
//        private GameModeSection _mode;
//        private ScoreSection _score;
//        private RoundSection _round;
//        private AudioSettingsSection _audio;
//        private InventorySection _inventory;
//        private LanguageSection _language;
//        private RoundRatingSection _roundRatings;

//        public GameModeSection Mode => _mode ??= new GameModeSection(this);
//        public ScoreSection Score => _score ??= new ScoreSection(this);
//        public RoundSection Round => _round ??= new RoundSection(this);
//        public AudioSettingsSection Audio => _audio ??= new AudioSettingsSection(this);
//        public InventorySection Inventory => _inventory ??= new InventorySection(this);
//        public LanguageSection Language => _language ??= new LanguageSection(this);
//        public RoundRatingSection RoundRatings => _roundRatings ??= new RoundRatingSection(this);

//        public int Points => Score.Points;
//        public string EquippedItemID => Inventory.EquippedItemID;

//        public void EnableAutoSaveOnExit()
//        {
//            Application.quitting += OnApplicationQuit;
//        }

//        public void DisableAutoSaveOnExit()
//        {
//            Application.quitting -= OnApplicationQuit;
//        }

//        public void SaveScore(int points)
//            => Score.SaveScore(points);

//        public void DecreaseScore(int points)
//            => Score.DecreaseScore(points);

//        public void AddItem(string itemID)
//            => Inventory.AddItem(itemID);

//        public void SaveEquippedItem(string itemID)
//            => Inventory.SaveEquippedItem(itemID);

//        public IReadOnlyList<string> GetPurchasedItems()
//            => Inventory.GetPurchasedItems();

//        public void SaveRoundNumber(int number)
//            => Round.SaveRoundNumber(number);

//        public int GetRoundNumber()
//            => Round.GetRoundNumber();

//        public void SaveMasterVolume(float value)
//            => Audio.SaveMasterVolume(value);

//        public void SaveAmbientVolume(float value)
//            => Audio.SaveAmbientVolume(value);

//        public void SaveEffectVolume(float value)
//            => Audio.SaveEffectVolume(value);

//        public void ForceSaveAudio()
//            => Audio.ForceSaver();

//        public void SaveLanguage(string language)
//            => Language.SaveLanguage(language);

//        public void SaveMode(GameMode mode)
//            => Mode.SaveMode(mode);

//        public GameMode LoadMode()
//            => Mode.LoadMode();

//        public bool HasSavedMode
//            => Mode.HasSavedMode;

//        public RoundRating GetRoundRating(GameMode mode, int roundIndex)
//            => RoundRatings.Get(mode, roundIndex);

//        public bool TryGetRoundRating(GameMode mode, int roundIndex, out RoundRating rating)
//            => RoundRatings.TryGet(mode, roundIndex, out rating);

//        public void SaveRoundRating(GameMode mode, int roundIndex, RoundRating rating, bool keepBest = true)
//            => RoundRatings.Save(mode, roundIndex, rating, keepBest);

//        public Dictionary<int, RoundRating> GetRoundRatingsForMode(GameMode mode)
//            => RoundRatings.GetRatingsForMode(mode);
//        private void OnApplicationQuit()
//        {
//            Audio.ForceSaver();
//            YG2.SaveProgress();
//        }
//    }
//}