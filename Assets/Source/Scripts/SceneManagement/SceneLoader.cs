using Assets.Source.Scripts.EntryPoint;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Source.Scripts.SceneManagement
{
    public class SceneLoader
    {
        private const int GameSceneIndex = 1;
        private const int MainMenuIndex = 0;

        private AsyncOperation _currentOperation;

        public void LoadMainMenu()
        {
            LoadScene(MainMenuIndex);
        }

        public void LoadGameScene()
        {
            LoadScene(GameSceneIndex);
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