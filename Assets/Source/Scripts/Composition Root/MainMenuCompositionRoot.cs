using Assets.Source.Scripts.DebugTools;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Factory;
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

        [Inject] private SequenceDifficultyLevel _sequenceDifficultyLevel;
        [Inject] private PlayerProgress _playerProgress;
        [Inject] private RoundSequenceProvider _roundSequenceProvider;

        protected override void ValidateDependencies()
        {
            Validator.AllNotNull(
                (_platformSetter, nameof(_platformSetter)),
                (_storeItemFactory, nameof(_storeItemFactory)),
                (_modePanelsPresenter, nameof(_modePanelsPresenter)),
                (_roundProgressDisplay, nameof(_roundProgressDisplay)),
                (_playerProgress, nameof(_playerProgress)),
                (_sequenceDifficultyLevel, nameof(_sequenceDifficultyLevel)),
                (_roundSequenceProvider, nameof(_roundSequenceProvider)));
        }

        protected override void CollectSceneInitializables()
        {
            if (_sequenceDifficultyLevel is IObjectInitilizable sequence)
                AddInitializable(sequence);
        }

        protected override void InitializeSceneDependenciesSync()
        {
            _languageSetter = new LanguageSetter();

            DebugSavesSeeder.SeedRatingsForAllRounds();
            _playerProgress.Initialize();
            _platformSetter.Initialize(_languageSetter);
            _modePanelsPresenter.Initialize();
            _roundProgressDisplay.Initialize(_roundSequenceProvider);
        }

        protected override IEnumerator InitializeFactoriesAsync()
        {
            _storeItemFactory.Spawn();

            yield break;
        }
    }
}
