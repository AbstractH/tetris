using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesBehaviour : MonoBehaviour
{
    
    private CubicTextMesh mesh;
    public GameFieldBehaviour game;
    public SpawnerBehaviour spawner;
    private int lives;

    private void Awake()
    {
        mesh = GetComponent<CubicTextMesh>();
        game.OnLifeRequested = GiveLife;
    }

    private void Start()
    {
        this.lives = 2;
        UpdateText();
    }

    public void AddLives(int livesToAdd)
    {
        this.lives += livesToAdd;
        UpdateText();
    }

    public bool GiveLife()
    {
        if (lives > 1)
        {
            lives--;
            UpdateText();
            return true;
        }
        else
            return false;
    }

    private void UpdateText()
    {
        if (lives > 1)
        {
            mesh.Text = "px" + this.lives;
            float s = 1f / mesh.Text.Length;
            this.transform.localScale = new Vector3(s,s,s);
        }
        else
        {
            mesh.Text = "";
        }
    }
    
    void Update()
    {
        
    }
}
