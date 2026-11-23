using Assets.Source.Scripts.UI.GamePanel;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.Vessels;
using Assets.Source.Scripts.UI.GameModeView;
using Assets.Source.Scripts.UI.Buttons;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.Factory;
using Assets.Source.Scripts.Tutorial;
using UnityEngine;
using System;

namespace Assets.Source.Scripts.EntryPoint
{
    public class PlatformGameAdapter : BasePlatformAdapter
    {
        [Header("Begin Objects Position")]
        [SerializeField] private ObjectsBeginPositionSetter _desktopObjectsPosition;
        [SerializeField] private ObjectsBeginPositionSetter _mobileObjectsPosition;
        [Header("UI Elements")]
        [SerializeField] private Panel _desktopEndRoundPanel;
        [SerializeField] private Panel _desktopEndModePanel;
        [SerializeField] private Panel _mobileEndRoundPanel;
        [SerializeField] private Panel _mobileEndModePanel;
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
        [SerializeField] private VesselRoundController _vesselRoundController;
        [SerializeField] private ClickModeSwitcher _clickModeSwitcher;
        [SerializeField] private TutorialMatchHighlighter _seeker;

        public void Initialize()
        {
            ValidateRequiredDependencies();
            InitializeBase();
        }

        protected override void OnMobileSelected()
        {
            _magicCellsFactory.SetCellPrefab(_magicCellMobile);
            _mobileObjectsPosition.Initialize();
            _vesselRoundController.SetFinalPanels(_mobileEndRoundPanel, _mobileEndModePanel);
            _clickModeSwitcher.SetModeButtons(_reverseButtonViewMobile);
            _seeker.SetButtonRewarded(_rewardedIconMobile, _reverseButtonMobile);
        }

        protected override void OnDesktopSelected()
        {
            _magicCellsFactory.SetCellPrefab(_magicCellDesktop);
            _desktopObjectsPosition.Initialize();
            _vesselRoundController.SetFinalPanels(_desktopEndRoundPanel, _desktopEndModePanel);
            _clickModeSwitcher.SetModeButtons(_reverseButtonViewDesktop);
            _seeker.SetButtonRewarded(_rewardedIconDesktop, _reverseButtonDesktop);
        }

        private void ValidateRequiredDependencies()
        {
            if (_desktopObjectsPosition == null)
                throw new ArgumentNullException(nameof(_desktopObjectsPosition));

            if (_mobileObjectsPosition == null)
                throw new ArgumentNullException(nameof(_mobileObjectsPosition));

            if (_mobileEndRoundPanel == null)
                throw new ArgumentNullException(nameof(_mobileEndRoundPanel));

            if (_mobileEndModePanel == null)
                throw new ArgumentNullException(nameof(_mobileEndModePanel));

            if (_desktopEndRoundPanel == null)
                throw new ArgumentNullException(nameof(_desktopEndRoundPanel));

            if (_desktopEndModePanel == null)
                throw new ArgumentNullException(nameof(_desktopEndModePanel));

            if (_vesselRoundController == null)
                throw new ArgumentNullException(nameof(_vesselRoundController));
        }
    }
}