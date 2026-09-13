using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.Audio;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.SceneManagement;
using Zenject;

namespace Assets.Source.Scripts.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SequenceDifficultyLevel>().AsSingle();
            Container.Bind<DifficultyState>().AsSingle().NonLazy();
            Container.Bind<Wallet>().AsSingle();
            Container.Bind<AudioSettingsData>().AsSingle();
            Container.Bind<SceneLoader>().AsSingle();
        }
    }
}