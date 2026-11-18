using System;

namespace Assets.Source.Scripts.GameBehaviour
{
    public class TutorialGameHandler : BaseGameHandler
    {
        public event Action TutorialCompleted;
        public event Action<int> TutorialRoundStarted;

        public bool IsTutorialCompleted => IsLastRound;

        protected override void OnRoundChanged()
            => TutorialRoundStarted?.Invoke(Counter.RoundNumber);

        protected override void OnLastRoundReached()
            => TutorialCompleted?.Invoke();
    }
}