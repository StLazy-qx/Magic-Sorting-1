using Zenject;
using Assets.Source.Scripts.UI.GameDifficultyView;
using Assets.Source.Scripts.GameDifficulty;

namespace Assets.Source.Scripts.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LevelDifficultyViewer>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}