using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
    private static readonly float GAME_TICK_SLOWER = 0.5f;
    private int level;
    private int lines;
    private int slowCounter;
    private float GameTickDelay;
    private CubicTextMesh mesh;

    public float GameTickDelay1 => GameTickDelay;

    public void DecrementSpeed(int volume)
    {
        this.slowCounter += volume;
        CalculateGameTick();
    }

    private void Awake()
    {
        mesh = GetComponent<CubicTextMesh>();
    }

    public void UpdateProgress()
    {
        this.lines++;
        this.level = (int) (lines / 10.0f)+1;
        CalculateGameTick();
        mesh.Text = level.ToString();
        float s = 1f / level.ToString().Length;
        this.transform.localScale = new Vector3(s,s,s);
    }

    private void CalculateGameTick()
    {
        float newTick = 2f - 1.5f / 10f * level + slowCounter*GAME_TICK_SLOWER;
        this.GameTickDelay = (newTick>=0.5f)?newTick:0.5f;
    }

    public void Clear()
    {
        this.lines = 0;
        this.slowCounter = 0;
        this.level = 1;
        this.GameTickDelay = 1.5f / (float)level;
        mesh.Text = level.ToString();
    }
    
}
