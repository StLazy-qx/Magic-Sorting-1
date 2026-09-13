using Assets.Source.Scripts.UI.GameDifficultyView;
using UnityEngine;
using Zenject;

namespace Assets.Source.Scripts.GameDifficulty
{
    public class DifficultyInstaller : MonoInstaller
    {
        [SerializeField] private DifficultyDatabase _difficultyDatabase;

        public override void InstallBindings()
        {
            Container.Bind<DifficultyState>().AsSingle().NonLazy();

            if (_difficultyDatabase != null)
            {
                Container.BindInstance(_difficultyDatabase).
                    WhenInjectedInto<LevelDifficultyViewer>();
            }
        }
    }
}