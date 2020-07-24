using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[ExecuteInEditMode]
public class GameFieldEditor : GameFieldBehaviour
{
    void Start()
    {
        Debug.Log("Eidtor start");
        base.init();
    }

    void Update()
    {
        Debug.Log("Eidtor update");
    }
}
