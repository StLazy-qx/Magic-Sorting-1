using Assets.Source.Scripts.Enums;
using Assets.Source.Scripts.Extensions;
using Assets.Source.Scripts.Game_Modes;
using Assets.Source.Scripts.Game_Progress;
using Assets.Source.Scripts.Player;
using System;
using UnityEngine;
using YG;
using Zenject;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class LevelCounter : MonoBehaviour
    {
        private const int FirstRoundNumber = 1;

        private int _roundIndex;
        private GameModeData _data;
        private IGameModeDataProvider _dataProvider;

        public event Action RoundChanged;
        public event Action RoundRestarted;
        public event Action LastRoundReached;

        public bool IsInitialized => _data != null;
        public bool IsEndless => Data.IsEndless;
        public bool IsLastRound => Data.IsEndless == false && _roundIndex >= Data.RoundsCount - 1;
        public int RoundsCount => Data.RoundsCount;
        public int RoundIndex => _roundIndex;
        public int RoundNumber => _roundIndex + 1;
        public GameMode Mode => Data.Mode;
        public DifficultyLevel CurrentDifficulty => Data.GetDifficulty(_roundIndex);

        private GameModeData Data
        {
            get
            {
                Guard.IsTrue(_data != null, 
                    "[LevelCounter] Initialize(mode) ещё не вызывался.");

                return _data;
            }
        }

        [Inject]
        private void Construct(IGameModeDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public void Initialize(GameMode mode)
        {
            _data = _dataProvider.Get(mode);

            _roundIndex = _data.IsEndless
                ? Mathf.Max(FirstRoundNumber, YG2.saves.Round.GetNumber()) - 1
                : 0;

            Announce();
        }

        public void RestartCurrentRound()
        {
            Guard.IsTrue(IsInitialized, "[LevelCounter] Initialize(mode) ещё не вызывался.");

            RoundRestarted?.Invoke();
        }

        public bool TryAdvance()
        {
            if (IsLastRound)
                return false;

            _roundIndex++;

            if (Data.IsEndless)
                YG2.saves.Round.SaveNumber(RoundNumber);

            Announce();

            return true;
        }

        private void Announce()
        {
            RoundChanged?.Invoke();

            if (IsLastRound)
                LastRoundReached?.Invoke();
        }
    }
}