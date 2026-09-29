using Assets.Source.Scripts.UI.GamePanel;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Vessels;
using Assets.Source.Scripts.UI.GameModeView;
using Assets.Source.Scripts.UI.Buttons;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Factory;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.EntryPoint
{
    public class PlatformTutorialGameAdapter : BasePlatformAdapter
    {
        [Header("Begin Objects Position")]
        [SerializeField] private ObjectsBeginPositionSetter _desktopObjectsPosition;
        [SerializeField] private ObjectsBeginPositionSetter _mobileObjectsPosition;
        [Header("UI Elements")]
        [SerializeField] private Panel _finalMatchPanelDesktop;
        [SerializeField] private Panel _finalMatchPanelMobile;
        [SerializeField] private Panel _finishTutorialPanelDesktop;
        [SerializeField] private Panel _finishTutorialPanelMobile;
        [SerializeField] private ReverseButton _reverseButtonDesktop;
        [SerializeField] private ReverseButton _reverseButtonMobile;
        [SerializeField] private IconRewardedAdvertisement _rewardedIconDesktop;
        [SerializeField] private IconRewardedAdvertisement _rewardedIconMobile;
        [SerializeField] private ReverseButtonView _reverseButtonViewDesktop;
        [SerializeField] private ReverseButtonView _reverseButtonViewMobile;
        [Header("Game Objects")]
        [SerializeField] private MagicCell _magicCellDesktop;
        [SerializeField] private MagicCell _magicCellMobile;
        [SerializeField] private MagicCellsFactory _magicCellsFactory;
        [Header("Links")]
        [SerializeField] private VesselStateTracker _vesselsFulling;
        [SerializeField] private FinalGameSession _finalGameSession;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;

        public void Initialize()
        {
            ValidateRequiredDependencies();
            InitializeBase();
        }

        protected override void OnMobileSelected()
        {
            _magicCellsFactory.SetCellPrefab(_magicCellMobile);
            _mobileObjectsPosition.Initialize();
            _vesselsFulling.SetPanel(_finalMatchPanelMobile);
            _finalGameSession.SetPanel(_finalMatchPanelMobile);
            _finalGameSession.SetTutorialPanel(_finishTutorialPanelMobile);
            _clickModeSwitcher.SetButton(_reverseButtonViewMobile);
        }

        protected override void OnDesktopSelected()
        {
            _magicCellsFactory.SetCellPrefab(_magicCellDesktop);
            _desktopObjectsPosition.Initialize();
            _vesselsFulling.SetPanel(_finalMatchPanelDesktop);
            _finalGameSession.SetPanel(_finalMatchPanelDesktop);
            _finalGameSession.SetTutorialPanel(_finishTutorialPanelDesktop);
            _clickModeSwitcher.SetButton(_reverseButtonViewDesktop);
        }

        private void ValidateRequiredDependencies()
        {
            if (_desktopObjectsPosition == null)
                throw new ArgumentNullException(nameof(_desktopObjectsPosition));

            if (_mobileObjectsPosition == null)
                throw new ArgumentNullException(nameof(_mobileObjectsPosition));

            if (_finishTutorialPanelDesktop == null)
                throw new ArgumentNullException(nameof(_finishTutorialPanelDesktop));

            if (_finishTutorialPanelMobile == null)
                throw new ArgumentNullException(nameof(_finishTutorialPanelMobile));

            if (_finalMatchPanelDesktop == null)
                throw new ArgumentNullException(nameof(_finalMatchPanelDesktop));

            if (_finalMatchPanelMobile == null)
                throw new ArgumentNullException(nameof(_finalMatchPanelMobile));

            if (_vesselsFulling == null)
                throw new ArgumentNullException(nameof(_vesselsFulling));

            if (_finalGameSession == null)
                throw new ArgumentNullException(nameof(_finalGameSession));
        }
    }
}
