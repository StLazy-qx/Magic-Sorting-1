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
        private const int RatingSceneIndex = 3;
        private const float MaxAsyncProgress = 0.9f;

        private AsyncOperation _currentOperation;

        public bool IsLoading => 
            _currentOperation != null && _currentOperation.isDone == false;
        public float Progress => _currentOperation == null
            ? 0f
            : Mathf.Clamp01(_currentOperation.progress / MaxAsyncProgress);

        public void LoadMainMenuScene()
            => LoadScene(MainMenuIndex);

        public void LoadGameScene()
            => LoadScene(GameSceneIndex);

        public void LoadTutorialScene()
            => LoadScene(TutorialScene);
            
         public void LoadRatingScene()
            => LoadScene(RatingSceneIndex);

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