using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.SceneManagement
{
    public class SceneLoader
    {
        private const int MainMenuIndex = 0;
        private const int GameSceneIndex = 1;
        private const int TutorialScene = 2;

        private AsyncOperation _currentOperation;

        public void LoadMainMenuScene()
        {
            LoadScene(MainMenuIndex);
        }

        public void LoadGameScene()
        {
            LoadScene(GameSceneIndex);
        }

        public void LoadTutorialScene()
        {
            LoadScene(TutorialScene);
        }

        private  void LoadScene(int sceneIndex)
        {
            if (sceneIndex < 0 || 
                sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                throw new ArgumentOutOfRangeException(nameof(sceneIndex));
            }

            _currentOperation = SceneManager.LoadSceneAsync(sceneIndex);
            _currentOperation.completed += _
                => {};
        }
    }
}