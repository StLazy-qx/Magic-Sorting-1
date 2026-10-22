using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Player
{
    public class PlayerProgress
    {
        private const int EasyEndNumberLevel   = 5;
        private const int MediumEndNumberLevel = 11;
        private const int HardEndNumberLevel   = 17;

        private int _currentRoundNumber;

        public bool IsInitialized { get; private set; }
        public PlayerProgressStatus Current { get; private set; }
        public GameMode CurrentMode { get; private set; }
        public int CurrentRoundNumber => _currentRoundNumber;
        public bool IsEasyCompleted   { get; private set; }
        public bool IsMediumCompleted { get; private set; }
        public bool IsHardCompleted   { get; private set; }

        public bool IsFreeModeAvailable =>
            IsEasyCompleted && IsMediumCompleted && IsHardCompleted;

        public void Initialize()
        {
            _currentRoundNumber = YG2.saves.GetRoundNumber();

            //
            Debug.Log($"Номер уровня {_currentRoundNumber}");

            Evaluate();

            Current = new PlayerProgressStatus(CurrentMode, _currentRoundNumber);
            IsInitialized = true;

            //
            Debug.Log($"PlayerProgress [инициализирован]");
        }

        private void Evaluate()
        {
            IsEasyCompleted   = _currentRoundNumber > EasyEndNumberLevel;
            IsMediumCompleted = _currentRoundNumber > MediumEndNumberLevel;
            IsHardCompleted   = _currentRoundNumber > HardEndNumberLevel;
            CurrentMode = DetermineCurrentMode(_currentRoundNumber);
        }

        private GameMode DetermineCurrentMode(int roundNumber)
        {
            if (roundNumber <= EasyEndNumberLevel)   
				return GameMode.Easy;
				
            if (roundNumber <= MediumEndNumberLevel) 
				return GameMode.Medium;
				
            if (roundNumber <= HardEndNumberLevel)   
				return GameMode.Hard;
            
            return GameMode.FreeMode;
        }

        public bool IsModeAvailable(GameMode mode) => mode switch
        {
            GameMode.Tutorial => true,
            GameMode.Easy     => true,
            GameMode.Medium   => IsEasyCompleted,
            GameMode.Hard     => IsMediumCompleted,
            GameMode.FreeMode => IsFreeModeAvailable,
            _                 => false
        };

        public bool IsModeCompleted(GameMode mode) => mode switch
        {
            GameMode.Easy   => IsEasyCompleted,
            GameMode.Medium => IsMediumCompleted,
            GameMode.Hard   => IsHardCompleted,
            _               => false
        };
    }
}