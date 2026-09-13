using Assets.Source.Scripts.UI.GamePanel;
using Assets.Source.Scripts.ActionsHandlers;
using Assets.Source.Scripts.GameBehaviour;
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
        [SerializeField] private Panel _finalMatchPanelDesktop;
        [SerializeField] private Panel _finalMatchPanelMobile;
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
            _vesselsFulling.ApplyPanel(_finalMatchPanelMobile);
            _finalGameSession.ApplyPanel(_finalMatchPanelMobile);
            _clickModeSwitcher.SetButton(_reverseButtonViewMobile);
            _seeker.SetButtonRewarded(_rewardedIconMobile, _reverseButtonMobile);
        }

        protected override void OnDesktopSelected()
        {
            _magicCellsFactory.SetCellPrefab(_magicCellDesktop);
            _desktopObjectsPosition.Initialize();
            _vesselsFulling.ApplyPanel(_finalMatchPanelDesktop);
            _finalGameSession.ApplyPanel(_finalMatchPanelDesktop);
            _clickModeSwitcher.SetButton(_reverseButtonViewDesktop);
            _seeker.SetButtonRewarded(_rewardedIconDesktop, _reverseButtonDesktop);
        }

        private void ValidateRequiredDependencies()
        {
            if (_desktopObjectsPosition == null)
                throw new ArgumentNullException(nameof(_desktopObjectsPosition));

            if (_mobileObjectsPosition == null)
                throw new ArgumentNullException(nameof(_mobileObjectsPosition));

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