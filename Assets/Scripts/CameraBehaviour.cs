using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tetris
{
    public class CameraBehaviour : MonoBehaviour
    {
        private bool _isCameraMoving = false;
        private float _startMoveTime;
        private Vector3[] _positionCurve;
        private Vector3[] _rotationCurve;

        public enum CameraPosition
        {
            Game,
            Menu
        }

        //todo: make it serializeable
        private Vector3 _menuPosition = new Vector3(5f, -30f, -10f);
        private Vector3 _gamePosition = new Vector3(0.5f, 0.5f, -15f);
        private Vector3 _menuRotation = new Vector3(-115f, 0f, 0f);
        private Vector3 _gameRotation = new Vector3(-25f, 15f, 0f);

        public Func<Boolean> onMoveComplete;

        void Update()
        {
            if (_isCameraMoving)
                MoveCamera();
        }

        public void StartCameraMoving(CameraPosition moveTo, Func<Boolean> cb)
        {
            _isCameraMoving = true;
            _startMoveTime = Time.time;
            ;
            switch (moveTo)
            {
                case CameraPosition.Game:
                    _positionCurve = new Vector3[4];
                    _positionCurve[0] = _menuPosition;
                    _positionCurve[1] = new Vector3(3f, -18.0f, -20.0f);
                    _positionCurve[2] = new Vector3(1, 15.0f, -50.0f);
                    _positionCurve[3] = _gamePosition;

                    _rotationCurve = new Vector3[3];
                    _rotationCurve[0] = _menuRotation;
                    _rotationCurve[1] = new Vector3(-20f, 7f, 0f);
                    _rotationCurve[2] = _gameRotation;
                    break;
                case CameraPosition.Menu:
                    _positionCurve = new Vector3[4];
                    _positionCurve[3] = _menuPosition;
                    _positionCurve[2] = new Vector3(3f, -18.0f, -20.0f);
                    _positionCurve[1] = new Vector3(1, 15.0f, -50.0f);
                    _positionCurve[0] = _gamePosition;

                    _rotationCurve = new Vector3[3];
                    _rotationCurve[2] = _menuRotation;
                    _rotationCurve[1] = new Vector3(-20f, 7f, 0f);
                    _rotationCurve[0] = _gameRotation;
                    break;
            }

            onMoveComplete = cb;
        }

        private void MoveCamera()
        {
            float t = (Time.time - _startMoveTime);

            transform.position = CalculateBezie(t, _positionCurve);
            transform.rotation = Quaternion.Euler(CalculateBezie(t, _rotationCurve));

            if (t > 1.0f)
            {
                _isCameraMoving = false;
                transform.position = _positionCurve[_positionCurve.Length - 1];
                transform.rotation = Quaternion.Euler(_rotationCurve[_rotationCurve.Length - 1]);

                onMoveComplete?.Invoke();
                return;
            }
        }

        private Vector3 CalculateBezie(float t, Vector3[] p)
        {
            if (p.Length == 0)
            {
                return Vector3.zero;
            }

            if (p.Length == 1)
            {
                return p[0];
            }

            Vector3[] np = new Vector3[p.Length - 1];
            for (int i = 0; i < np.Length; i++)
            {
                Vector3 v = p[i + 1] - p[i];
                np[i] = p[i] + (v * t);
            }

            return CalculateBezie(t, np);
        }
    }
}