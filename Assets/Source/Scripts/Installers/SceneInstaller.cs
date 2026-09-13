using Zenject;
using Assets.Source.Scripts.UI.GameDifficultyView;

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