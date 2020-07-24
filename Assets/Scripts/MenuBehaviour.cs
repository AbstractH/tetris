using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            camera.StartCameraMoving(CameraBehaviour.CameraPosition.GAME, StartNewGame);
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
        camera.StartCameraMoving(CameraBehaviour.CameraPosition.MENU, OnMenuShown);
    }

    private Boolean OnMenuShown()
    {
        isActiveMenu = true;
        return true;
    }
    
   
}
