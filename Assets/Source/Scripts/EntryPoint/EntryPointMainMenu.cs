using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Language;
using Assets.Source.Scripts.Player;
using System;
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
        [SerializeField] private MonoBehaviour[] _servicesMono;

        private LanguageSetter _languageSetter;
        private PlayerProgress _playerProgress;
        private List<IObjectInitilizable> _servicesInitializable = new();

        [Inject] 
        private SequenceDifficultyLevel _sequenceDifficultyLevel;

        private void Awake()
        {
            _playerProgress = new PlayerProgress();
            _languageSetter = new LanguageSetter();

            ValidateDependencies();
            _playerProgress.Initialize();
            _platformSetter.Initialize(_languageSetter);
            _modePanelsPresenter.Initialize(_playerProgress);

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
            if (_platformSetter == null)
                throw new ArgumentNullException(nameof(_platformSetter));

            if (_storeItemFactory == null)
                throw new ArgumentNullException(nameof(_storeItemFactory));

            if (_servicesMono == null)
                throw new ArgumentNullException(nameof(_servicesMono));
        }
    }
}