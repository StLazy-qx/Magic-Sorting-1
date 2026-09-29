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
        private const int WaitPointMechanicsPanelIndex = 1;
        private const int ReverseMechanicsPanelIndex = 2;
        private const int AmountCellActionToMainMechanics = 2;
        private const int ReverseMechanicsRoundNumber = 1;
        private const float MechanicsPanelDelay = 1.7f;

        private readonly AsyncTimer _asyncTimer = new AsyncTimer();

        [SerializeField] private TutorialGameHandler _gameHandler;
        [SerializeField] private TutorialLevelCounter _tutorialLevelCounter;
        [SerializeField] private MagicCellRouter _magicCellRouter;
        [SerializeField] private ReverseButton _reverseButton;
        [SerializeField] private Panel[] _panels;

        private int _cellActionCount;
        private bool _IsShowWaitPointMechanics;

        private void Awake()
        {
            Guard.NotNull(_gameHandler, nameof(_gameHandler));
            Guard.NotNull(_tutorialLevelCounter, nameof(_tutorialLevelCounter));
            Guard.NotNull(_magicCellRouter, nameof(_magicCellRouter));
            Guard.NotNull(_reverseButton, nameof(_reverseButton));
            Guard.NotNullOrEmpty(_panels, nameof(_panels));

            _cellActionCount = 0;
            _IsShowWaitPointMechanics = false;
        }

        private void Start()
        {
            ShowBeginPanel();
        }

        private void OnEnable()
        {
            _magicCellRouter.CellDeparturing += OnAddCellAction;
            _tutorialLevelCounter.RoundChanged += OnContinueTraining;
        }

        private void OnDisable()
        {
            _magicCellRouter.CellDeparturing -= OnAddCellAction;
            _tutorialLevelCounter.RoundChanged -= OnContinueTraining;
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

        public void ShowWaitPointMechanicsPanel()
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

        private void OnContinueTraining()
        {
            if (_tutorialLevelCounter.RoundNumber == ReverseMechanicsRoundNumber)
            {
                ShowReverseMechanicsPanel();
            }
        }

        private void OnAddCellAction()
        {
            _cellActionCount++;

            if (_cellActionCount >= AmountCellActionToMainMechanics
                && _IsShowWaitPointMechanics == false)
            {
                ShowWaitPointMechanicsPanel();

                _IsShowWaitPointMechanics = true;
            }
        }
    }
}
