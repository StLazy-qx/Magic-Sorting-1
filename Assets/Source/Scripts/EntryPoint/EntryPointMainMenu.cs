using Assets.Source.Scripts.DebugTools;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Language;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.EntryPoint
{
    public class EntryPointMainMenu : MonoBehaviour
    {
        [SerializeField] private PlatformMenuAdapter _platformSetter;
        [SerializeField] private StoreItemFactory _storeItemFactory;
        [SerializeField] private MainMenuPanelsPresenter _modePanelsPresenter;
        [SerializeField] private RoundProgressDisplay _roundProgressDisplay;
        [SerializeField] private MonoBehaviour[] _servicesMono;

        private LanguageSetter _languageSetter;
        private List<IObjectInitilizable> _servicesInitializable = new();

        [Inject] private SequenceDifficultyLevel _sequenceDifficultyLevel;
        [Inject] private PlayerProgress _playerProgress;
        [Inject] private RoundSequenceProvider _roundSequenceProvider;

        private void Awake()
        {
            _languageSetter = new LanguageSetter();

            ValidateDependencies();

            DebugSavesSeeder.SeedRatingsForAllRounds();

            _playerProgress.Initialize();

            Debug.Log("---- Проверка загрузки в PlayerProgress");
            Debug.Log(_playerProgress.CurrentMode);
            Debug.Log(_playerProgress.CurrentRoundNumber);

            _platformSetter.Initialize(_languageSetter);
            _modePanelsPresenter.Initialize();
            _roundProgressDisplay.Initialize(_roundSequenceProvider);

            foreach (var mono in _servicesMono)
            {
                if (mono is IObjectInitilizable initObj)
                    _servicesInitializable.Add(initObj);
            }

            if (_sequenceDifficultyLevel is IObjectInitilizable sequence)
                _servicesInitializable.Add(sequence);

            StartCoroutine(InitializeServices());
        }

        private IEnumerator InitializeServices()
        {
            _storeItemFactory.Spawn();

            foreach (IObjectInitilizable currentObject in _servicesInitializable)
                currentObject.Initialize();

            yield return new WaitUntil(()
                => _servicesInitializable.TrueForAll
                (currentObject => currentObject.IsInitialized));
        }

        private void ValidateDependencies()
        {
            Guard.NotNull(_platformSetter, nameof(_platformSetter));
            Guard.NotNull(_storeItemFactory, nameof(_storeItemFactory));
            Guard.NotNull(_modePanelsPresenter, nameof(_modePanelsPresenter));
            Guard.NotNull(_roundProgressDisplay, nameof(_roundProgressDisplay));
            Guard.NotNull(_servicesMono, nameof(_servicesMono));
            Guard.NotNull(_playerProgress, nameof(_playerProgress));
            Guard.NotNull(_sequenceDifficultyLevel, nameof(_sequenceDifficultyLevel));
            Guard.NotNull(_roundSequenceProvider, nameof(_roundSequenceProvider));
        }
    }
}