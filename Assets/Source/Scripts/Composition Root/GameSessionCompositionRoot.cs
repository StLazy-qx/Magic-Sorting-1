using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Colorize;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.InteractiveObjects;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Player;
using UnityEngine;

namespace Assets.Source.Scripts.Composition_Root
{
    public class GameSessionCompositionRoot : SessionCompositionRootBase
    {
        [Header("Game scene")]
        [SerializeField] private PlatformGameAdapter _platformGameAdapter;
        [SerializeField] private StoreItemFactory _storeItemFactory;
        [SerializeField] private MagicMixer _magicMixer;
        [SerializeField] private MagicCellRouter _magicCellRouter;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;

        protected override GameMode ResolveMode()
            => Progress.CurrentMode;

        protected override void ValidateSessionDependencies()
        {
            Validator.AllNotNull(
                (_platformGameAdapter, nameof(_platformGameAdapter)),
                (_storeItemFactory, nameof(_storeItemFactory)),
                (_magicMixer, nameof(_magicMixer)),
                (_magicCellRouter, nameof(_magicCellRouter)),
                (_clickModeSwitcher, nameof(_clickModeSwitcher)));
        }

        protected override void InitializeSessionSync()
        {
            _platformGameAdapter.Initialize();
            _magicMixer.Initialize(ColorContext.Recipes, ColorContext.Converter);
        }

        protected override void SpawnAdditionalFactories()
        {
            _storeItemFactory.Spawn();
        }

        private void OnEnable()
        {
            if (_magicCellRouter != null && _clickModeSwitcher != null)
                _magicCellRouter.CellSentToMixer += _clickModeSwitcher.OnMixerCellSent;

            if (_magicMixer != null && _clickModeSwitcher != null)
                _magicMixer.MixFailed += _clickModeSwitcher.OnMixFailed;
        }

        private void OnDisable()
        {
            if (_magicCellRouter != null && _clickModeSwitcher != null)
                _magicCellRouter.CellSentToMixer -= _clickModeSwitcher.OnMixerCellSent;

            if (_magicMixer != null && _clickModeSwitcher != null)
                _magicMixer.MixFailed -= _clickModeSwitcher.OnMixFailed;
        }
    }
}