using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraBehaviour : MonoBehaviour
{
    private bool isCameraMoving = false;
    private float startMoveTime;
    private Vector3[] positionCurve;
    private Vector3[] rotationCurve;
    
    public enum CameraPosition { GAME, MENU }

    private CameraPosition currentPossition = CameraPosition.MENU;
    
    private Vector3 MenuPosition = new Vector3(5f,-30f,-10f);
    private Vector3 GamePosition = new Vector3(0.5f,0.5f,-15f);
    private Vector3 MenuRotation = new Vector3(-115f, 0f, 0f);
    private Vector3 GameRotation = new Vector3(-25f, 15f, 0f);

    public Func<Boolean> onMoveComplete;

    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (isCameraMoving)
            MoveCamera();
        
    }
    
    public void StartCameraMoving(CameraPosition moveTo, Func<Boolean> cb)
    {
        isCameraMoving = true;
        startMoveTime = Time.time;;
        switch (moveTo)
        {
            case CameraPosition.GAME: 
                positionCurve = new Vector3[4];
                positionCurve[0] = MenuPosition;
                positionCurve[1] = new Vector3(3f,-18.0f,-20.0f);
                positionCurve[2] = new Vector3(1,15.0f,-50.0f);
                positionCurve[3] = GamePosition;
                
                rotationCurve = new Vector3[3];
                rotationCurve[0] = MenuRotation;
                rotationCurve[1] = new Vector3(-20f, 7f,0f);
                rotationCurve[2] = GameRotation;
                break;
            case CameraPosition.MENU:
                positionCurve = new Vector3[4];
                positionCurve[3] = MenuPosition;
                positionCurve[2] = new Vector3(3f,-18.0f,-20.0f);
                positionCurve[1] = new Vector3(1,15.0f,-50.0f);
                positionCurve[0] = GamePosition;
                
                rotationCurve = new Vector3[3];
                rotationCurve[2] = MenuRotation;
                rotationCurve[1] = new Vector3(-20f, 7f,0f);
                rotationCurve[0] = GameRotation;
                break;
        }

        onMoveComplete = cb;
    }

    private void MoveCamera()
    {
        float t = (Time.time - startMoveTime);
        
        transform.position = CalculateBezie(t, positionCurve);
        transform.rotation = Quaternion.Euler(CalculateBezie(t, rotationCurve));
        
        if (t > 1.0f)
        {
            isCameraMoving = false;
            transform.position = positionCurve[positionCurve.Length - 1];
            transform.rotation = Quaternion.Euler(rotationCurve[rotationCurve.Length - 1]);
            
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
        Vector3[] np = new Vector3[p.Length-1];
        for (int i = 0; i < np.Length; i++)
        {
            Vector3 v = p[i+1] - p[i];
            np[i] = p[i]+(v * t);
        }

        return CalculateBezie(t, np);
    }
}
