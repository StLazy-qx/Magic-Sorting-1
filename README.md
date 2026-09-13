# Magic Sorting
Re-uploading the project with .gitignore

Magic Sorting

Игра в жанре **casual / puzzle** для платформы **Яндекс Игры**, разработанная на **Unity** с акцентом на чистую архитектуру, расширяемость и переиспользование логики.

Проект построен вокруг механики сортировки магических сущностей (жидкостей / ячеек) по цветам и состояниям с постепенным усложнением правил.

---

Платформа

- **Движок:** Unity
- **Целевая платформа:** WebGL (Яндекс Игры)
- **Интеграция:** YandexGame Framework (сохранения, реклама, локализация)
- **Язык:** C#

---

Архитектура проекта

Проект следует принципам:

- **SOLID**
- **Composition over inheritance**
- **Dependency Injection** (через Installers)
- **Data‑Driven подход** (ScriptableObjects)

Используемые паттерны:

- Factory (обобщённые фабрики)
- Strategy
- State
- Adapter (платформенная абстракция)
- Object Pool
- Observer / Event‑based взаимодействие
- MVC / MVP (в игровой логике и UI)

---

Структура Scripts

```
Scripts/
├── ActionHandlers
├── Audio
├── Colorize
├── EntryPoint
├── Enums
├── Extensions
├── Factory
├── GameBehaviour
├── GameDifficulty
├── Installers
├── InteractiveObjects
├── Items
├── Language
├── MagicCells
├── Player
├── Pool
├── SceneManagement
├── Storage
├── UI
├── Vessels
├── YG
```

---

ActionHandlers — взаимодействие игрока

**Назначение:** единая система обработки пользовательских действий.

- `ActionHandler` — базовый класс обработки действий
- `ClickHandler` — обработка кликов / тапа
- `IInteractable` — интерфейс для всех интерактивных объектов

Позволяет добавлять новые типы взаимодействий без изменения существующего кода.

Паттерны: **Command, Interface Segregation**

---

 Audio — звуковая система

- `SoundPlayer`, `AmbientAudioPlayer` — проигрывание SFX и ambience
- `AudioSetter`, `SoundSetter` — применение пользовательских настроек
- `AudioSettingsData` — ScriptableObject с конфигурацией

Аудио полностью отвязано от геймплея и управляется через данные.

---

Colorize — работа с цветами

- `IColorable` — контракт для окрашиваемых объектов
- `ColorMarker` — хранение цвета
- `ColorRandomizer` — генерация цветов
- `ShuffledColorDistributor` — распределение цветов без повторов

Используется для логики сортировки и проверки условий победы.

---

EntryPoint — точки входа

- `EntryPointMainMenu`
- `EntryPointGameSession`

Отвечают за:
- инициализацию сцены
- запуск нужных Installer‑ов
- адаптацию под платформу

Дополнительно:
- `PlatformAdapter`, `PlatformGameAdapter`, `PlatformMenuAdapter`

Паттерн: **Adapter**

---

Factory — генерация объектов

Обобщённая базовая фабрика:

- `Factory<T>`

Конкретные реализации:

- `ColumnsFactory`
- `MagicCellsFactory`
- `VesselFactory`
- `StoreItemFactory`

Позволяет централизованно управлять созданием игровых объектов.

Паттерн: **Factory Method + Generics**

---

GameBehaviour — логика игры

Ключевой слой проекта.

- `IGameHandler` — контракт состояния игры
- `GameHandler` — основной игровой процесс
- `MenuHandler` — логика меню
- `FinalGameSession` — завершение уровня
- `GameSessionHandler` — управление сессией
- `GameHandlerPresenter` — связь логики и представления

Реализация **State‑машины** игрового процесса.

---

GameDifficulty — сложность

- `DifficultyLevel` (enum)
- `DifficultySettings` — параметры сложности
- `DifficultyDatabase` — хранилище
- `DifficultyState` — текущее состояние
- `DifficultyInstaller` — внедрение зависимостей

Позволяет масштабировать сложность без изменения логики.

---

MagicCells — основная механика

- `MagicCell` — базовая игровая сущность
- `MagicCellMover` — перемещение
- `MagicCellRouter` — маршрутизация

Ядро сортировочной механики.

---

Vessels — сосуды и жидкости

- `Vessel` — контейнер
- `Liquid` — содержимое
- `VolumeAggregator` — подсчёт объёма
- `VesselsFullingBehaviour` — логика наполнения
- `VesselCompletionEffecter` — эффекты завершения

Проверка условий правильной сортировки.

---

Player

- `PlayerEntity` — сущность игрока
- `Inventory` — инвентарь
- `Wallet` — валюта
- `MagicianAnimator` — анимации
- `SkinSetter` — визуальные скины

---

Pool — пулы объектов

- `ParticlePool`
- `StackMagicCellsHandler`

Снижение аллокаций и GC в WebGL.

Паттерн: **Object Pool**

---

Storage / YG

- `Store` — внутриигровой магазин
- `SavesYG` — сохранения через YandexGame

Поддержка автосохранения и прогресса.

---

UI

- Адаптация под платформы:
  - `CanvasDesktopSetter`
  - `CanvasMobileSetter`

- Кнопки:
  - `BaseMenuButton`
  - `ButtonAdvertisingActivater`
  - `ButtonNewRoundBeginner`

UI отделён от логики и реагирует на события.

---

Installers

- `ProjectInstaller`
- `SceneInstaller`

Централизованная регистрация зависимостей и сервисов.

---

Локализация

- `LanguageSetter`

Поддержка языков через Yandex Games API.

---

Extensions

- `Vector3Extensions`

Вспомогательные расширения для чистоты кода.

---

Core Gameplay Loop

1. Инициализация сцены через `EntryPointGameSession`
2. Загрузка настроек сложности (`DifficultyState`)
3. Генерация игровых объектов через фабрики
4. Распределение цветов и начальных состояний
5. Ожидание действий игрока (ActionHandlers)
6. Перемещение и сортировка магических сущностей
7. Проверка условий заполнения сосудов
8. Завершение сессии (`FinalGameSession`)
9. Сохранение прогресса и переход

---

Архитектурная схема (логическая)

```
[ EntryPoint ]
      ↓
[ Installers ] → Services / Settings
      ↓
[ GameHandler (State) ]
      ↓
[ Factories ] → Game Objects
      ↓
[ Player Actions ] → ActionHandlers
      ↓
[ MagicCells / Vessels ]
      ↓
[ UI / Audio / Effects ]
```

---

Производительность и WebGL

- Используется **Object Pooling** для частиц и игровых сущностей
- Минимизация GC-аллокций
- Отсутствие `Update()` там, где возможно
- Data-driven настройки через ScriptableObjects

---

Расширяемость

### Добавление нового типа сосуда

1. Унаследоваться от `Vessel`
2. Реализовать нужное поведение наполнения
3. Зарегистрировать в `VesselFactory`
4. Настроить визуал и эффекты

### Добавление новой сложности

1. Создать новый `DifficultySettings`
2. Добавить в `DifficultyDatabase`
3. Подключить через `DifficultyInstaller`

---

Сборка и публикация

- Платформа: WebGL
- Интеграция: YandexGame Framework
- Поддержка:
  - сохранений
  - рекламы
  - локализации

Проект готов к публикации в **Яндекс Игры** без изменения архитектуры.

---

Портфолио-ценность проекта

Проект демонстрирует:

- системное мышление
- умение проектировать архитектуру
- владение паттернами проектирования
- опыт оптимизации под WebGL
- работу с платформенными SDK

---

Итог

**Magic Sorting** — это завершённый, масштабируемый игровой проект, ориентированный на production-подход и дальнейшее развитие.

---

Автор проекта: *StLazy-qx*
