using System;
using UnityEngine;

namespace Assets.Source.Scripts.EntryPoint
{
    public class ObjectsBeginPositionSetter : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _chair;
        [SerializeField] private Transform _columnsPoints;
        [Header("Positions & Rotations to objects")]
        [SerializeField] private Vector3 _cameraPosition;
        [SerializeField] private Vector3 _cameraRotation;
        [SerializeField] private Vector3 _playerPosition;
        [SerializeField] private Vector3 _playerRotation;
        [SerializeField] private Vector3 _chairPosition;
        [SerializeField] private Vector3 _columnsPointsPosition;

        public void Initialize()
        {
            ValidateRequiredObjects();
            PositionObjects();
        }

        private void ValidateRequiredObjects()
        {
            if (_mainCamera == null)
                throw new ArgumentNullException(nameof(_mainCamera));

            if (_player == null)
                throw new ArgumentNullException(nameof(_player));

            if (_chair == null)
                throw new ArgumentNullException(nameof(_chair));

            if (_columnsPoints == null)
                throw new ArgumentNullException(nameof(_columnsPoints));
        }

        private void PositionObjects()
        {
            _mainCamera.transform.SetPositionAndRotation(
                _cameraPosition, Quaternion.Euler(_cameraRotation));
            _player.transform.SetPositionAndRotation(
                _playerPosition, Quaternion.Euler(_playerRotation));
            _chair.transform.SetPositionAndRotation(
                _chairPosition, Quaternion.identity);
            _columnsPoints.transform.SetPositionAndRotation(
                _columnsPointsPosition, Quaternion.identity);
        }
    }
}