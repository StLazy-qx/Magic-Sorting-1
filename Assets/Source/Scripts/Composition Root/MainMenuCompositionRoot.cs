using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Game_Modes;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Language;
using Assets.Source.Scripts.Player;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.Composition_Root
{
    public class MainMenuCompositionRoot : CompositionRootBase
    {
        [Header("Scene")]
        [SerializeField] private PlatformMenuAdapter _platformSetter;
        [SerializeField] private StoreItemFactory _storeItemFactory;
        [SerializeField] private MainMenuPanelsPresenter _modePanelsPresenter;
        [SerializeField] private RoundProgressDisplay _roundProgressDisplay;

        private LanguageSetter _languageSetter;

        [Inject] private PlayerProgress _playerProgress;
        [Inject] private IGameModeDataProvider _gameModeDataProvider;

        protected override void ValidateDependencies()
        {
            Validator.AllNotNull(
                (_platformSetter, nameof(_platformSetter)),
                (_storeItemFactory, nameof(_storeItemFactory)),
                (_modePanelsPresenter, nameof(_modePanelsPresenter)),
                (_roundProgressDisplay, nameof(_roundProgressDisplay)));
        }

        protected override void InitializeSceneDependenciesSync()
        {
            _languageSetter = new LanguageSetter();

#if UNITY_EDITOR
            // Полное имя: в билде namespace DebugTools не существует, обычный using сломал бы сборку
            Assets.Source.Scripts.DebugTools.DebugSavesSeeder.SeedRatingsForAllRounds(_gameModeDataProvider);
#endif
            // Меню каждый раз перечитывает снимок из сейва (после игры он устаревает)
            _playerProgress.Reload();
            _platformSetter.Initialize(_languageSetter);
            _modePanelsPresenter.Initialize();
            _roundProgressDisplay.Initialize();
        }

        protected override IEnumerator InitializeFactoriesAsync()
        {
            _storeItemFactory.Spawn();

            yield break;
        }
    }
}