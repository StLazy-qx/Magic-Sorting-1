using System;
using Assets.Source.Scripts.SceneManagement;
using Zenject;

namespace Assets.Source.Scripts.SceneManagement
{
    public class SceneNavigator
    {
        private readonly SceneLoader _sceneLoader;

        [Inject]
        public SceneNavigator(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        }

        public void OpenMainMenu()
        {
            _sceneLoader.LoadMainMenuScene();
        }

        public void OpenTutorialScene()
        {
            _sceneLoader.LoadTutorialScene();
        }

        public void OpenGameSession()
        {
            _sceneLoader.LoadGameScene();
        }
    }
}