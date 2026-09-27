using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Pool;
using Assets.Source.Scripts.UI.GamePanel;
using UnityEngine;

namespace Assets.Source.Scripts.Tutorial
{
    public class TutorialPanelsPresenter : MonoBehaviour
    {
        private const int BeginPanelIndex = 0;
        //private const int MainMechanicsPanelIndex = 1;
        private const int WaitPointMechanicsPanelIndex = 2;
        private const int ReverseMechanicsPanelIndex = 3;
        private const int EndTutorialPanelIndex = 4;
        private const int AmountCellActionToMainMechanics = 2;

        [SerializeField] private TutorialGameHandler _gameHandler;
        [SerializeField] private StackMagicCells _stackMagicCells;
        [SerializeField] private Panel[] _panels;

        private int _cellActionCount;

        private void OnEnable()
        {
            _cellActionCount = 0;
            _stackMagicCells.CellSented += OnAddCellAction;
        }

        private void OnDisable()
        {
            _stackMagicCells.CellSented -= OnAddCellAction;
        }

        private void Start()
        {
            ShowBeginPanel();
        }

        public void ShowBeginPanel()
        {
            _gameHandler.PauseGame();
            _panels[BeginPanelIndex].Open();
        }

        public void ShowWaitPointMechanicsPanel()
        {
            _gameHandler.PauseGame();
            _panels[WaitPointMechanicsPanelIndex].Open();
        }

        private void OnAddCellAction()
        {
            _cellActionCount++;

            Debug.Log($"добавление счетчика {_cellActionCount}");

            if (_cellActionCount >= AmountCellActionToMainMechanics)
            {
                ShowWaitPointMechanicsPanel();
            }
        }
    }
}
