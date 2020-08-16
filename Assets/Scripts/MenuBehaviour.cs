using System;
using UnityEngine;

namespace Tetris
{
    public class MenuBehaviour : MonoBehaviour
    {

        public CameraBehaviour camera;
        private bool isActiveMenu = true;
        public GameFieldBehaviour gameField;

        void Start()
        {
            gameField.OnGameOver += HanleGameOver;
        }
    
        void Update()
        {
            if (Input.GetKeyDown("space") && isActiveMenu)
            {
                camera.StartCameraMoving(CameraBehaviour.CameraPosition.Game, StartNewGame);
                isActiveMenu = false;
            }
        }

        private Boolean StartNewGame()
        {
            gameField.NewGame();
            return true;
        }

        private void HanleGameOver()
        {
            camera.StartCameraMoving(CameraBehaviour.CameraPosition.Menu, OnMenuShown);
        }

        private Boolean OnMenuShown()
        {
            isActiveMenu = true;
            return true;
        }
    
   
    }

}
