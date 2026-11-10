using Assets.Source.Scripts.GameDifficulty;
using Assets.Source.Scripts.Player;
using Assets.Source.Scripts.Audio;
using Assets.Source.Scripts.EntryPoint;
using Assets.Source.Scripts.SceneManagement;
using Assets.Source.Scripts.GameBehaviour;
using Assets.Source.Scripts.Game_Progress;
using Zenject;
using UnityEngine;
using YG;

namespace Assets.Source.Scripts.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameModeData[] _gameModeDatas;

        public override void InstallBindings()
        {
            YG2.saves.Initialize();
            YG2.saves.EnableAutoSaveOnExit();
            Container.Bind<GameModeRuntimeProvider>()
            .FromInstance(new GameModeRuntimeProvider(_gameModeDatas))
            .AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerProgress>().AsSingle().NonLazy();
            Container.Bind<RoundSequenceProvider>().AsSingle();
            Container.Bind<SequenceDifficultyLevel>().AsSingle();
            Container.Bind<DifficultyState>().AsSingle().NonLazy();
            Container.Bind<Wallet>().AsSingle();
            Container.Bind<AudioSettingsData>().AsSingle();
            Container.Bind<SceneLoader>().AsSingle();
        }
    }
}