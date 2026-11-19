using Assets.Source.Scripts.Audio;
using Assets.Source.Scripts.Game_Modes;
using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.SceneManagement;
using YG;
using Zenject;

namespace Assets.Source.Scripts.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            YG2.saves.Initialize();
            YG2.saves.EnableAutoSaveOnExit();
            Container.Bind<IGameModeDataProvider>().To<GameModeDataProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerProgress>().AsSingle().NonLazy();
            Container.Bind<DifficultyState>().AsSingle().NonLazy();
            Container.Bind<Wallet>().AsSingle();
            Container.Bind<AudioSettingsData>().AsSingle();
            Container.Bind<SceneLoader>().AsSingle();
        }
    }
}