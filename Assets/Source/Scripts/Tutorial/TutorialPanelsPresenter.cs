using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.MagicCells;
using Assets.Source.Scripts.UI.Buttons;
using Assets.Source.Scripts.UI.GamePanel;
using UnityEngine;

namespace Assets.Source.Scripts.Tutorial
{
    public class TutorialPanelsPresenter : MonoBehaviour
    {
        private const int BeginPanelIndex = 0;
        private const int ColorMixMechanicsPanelIndex = 1;
        private const int WaitPointMechanicsPanelIndex = 2;
        private const int ReverseMechanicsPanelIndex = 3;
        private const int AmountCellActionColorMixMechanics = 2;
        private const int AmountCellActionWaitingMechanics = 4;
        private const int ReverseMechanicsRoundIndex = 1;
        private const float MechanicsPanelDelay = 1.7f;

        private readonly AsyncTimer _asyncTimer = new AsyncTimer();

        [SerializeField] private TutorialGameHandler _gameHandler;
        [SerializeField] private MagicCellRouter _magicCellRouter;
        [SerializeField] private ReverseButton _reverseButton;
        [SerializeField] private Panel[] _panels;

        private int _cellActionCount;
        private bool _IsShowWaitPointMechanics;
        private bool _IsShowColorMixMechanics;

        private void Awake()
        {
            Guard.NotNull(_gameHandler, nameof(_gameHandler));
            Guard.NotNull(_magicCellRouter, nameof(_magicCellRouter));
            Guard.NotNull(_reverseButton, nameof(_reverseButton));
            Guard.NotNullOrEmpty(_panels, nameof(_panels));

            _cellActionCount = 0;
            _IsShowWaitPointMechanics = false;
            _IsShowColorMixMechanics = false;
        }

        private void Start()
        {
            ShowBeginPanel();
        }

        private void OnEnable()
        {
            _magicCellRouter.CellDeparturing += OnAddCellAction;
            _magicCellRouter.WaitingPointUsing += OnWaitingPointPlaceUsed;
            _gameHandler.TutorialRoundStarted += OnTutorialRoundStarted;
        }

        private void OnDisable()
        {
            _magicCellRouter.CellDeparturing -= OnAddCellAction;
            _magicCellRouter.WaitingPointUsing -= OnWaitingPointPlaceUsed;
            _gameHandler.TutorialRoundStarted -= OnTutorialRoundStarted;
        }

        public void ShowBeginPanel()
        {
            _gameHandler.PauseGame();
            _panels[BeginPanelIndex].Open();
        }

        public void ShowReverseMechanicsPanel()
        {
            _gameHandler.PauseGame();
            _panels[ReverseMechanicsPanelIndex].Open();
            _reverseButton.gameObject.SetActive(true);
            _reverseButton.Enable();
        }

        private void OnWaitingPointPlaceUsed()
        {
            TryShowWaitingPointMechanicsPanel();
        }

        private void OnTutorialRoundStarted(int roundIndex)
        {
            if (roundIndex == ReverseMechanicsRoundIndex)
                ShowReverseMechanicsPanel();
        }

        private void OnAddCellAction()
        {
            _cellActionCount++;

            if (_cellActionCount >= AmountCellActionColorMixMechanics)
                TryShowColorMixMechanicsPanel();

            if (_cellActionCount >= AmountCellActionWaitingMechanics)
                TryShowWaitingPointMechanicsPanel();
        }

        private void TryShowWaitingPointMechanicsPanel()
        {
            if (_IsShowWaitPointMechanics)
                return;

            _IsShowWaitPointMechanics = true;

            ShowWaitPointMechanicsPanel();
        }

        private void TryShowColorMixMechanicsPanel()
        {
            if (_IsShowColorMixMechanics)
                return;

            _IsShowColorMixMechanics = true;

            ShowColorMixtMechanicsPanel();
        }

        private void ShowColorMixtMechanicsPanel()
        {
            _asyncTimer.StopTimer();
            _asyncTimer.StartTimer(
                MechanicsPanelDelay,
                () =>
                {
                    _gameHandler.PauseGame();
                    _panels[ColorMixMechanicsPanelIndex].Open();
                });
        }

        private void ShowWaitPointMechanicsPanel()
        {
            _asyncTimer.StopTimer();
            _asyncTimer.StartTimer(
                MechanicsPanelDelay,
                () =>
                {
                    _gameHandler.PauseGame();
                    _panels[WaitPointMechanicsPanelIndex].Open();
                });
        }
    }
}
