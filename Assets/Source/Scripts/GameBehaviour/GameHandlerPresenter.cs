using System;
using UnityEngine;

namespace Assets.Source.Scripts.GameBehaviour
{
    class GameHandlerPresenter : MonoBehaviour
    {
        [SerializeField] private IGetGameHandlable _gameObjects;

        private GameHandler _gameHandler;

        public void Initialize(GameHandler gameHandler)
        {
            _gameHandler = gameHandler ??
                throw new ArgumentNullException(nameof(gameHandler));
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }
    }
}
