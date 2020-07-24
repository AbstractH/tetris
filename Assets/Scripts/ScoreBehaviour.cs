using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBehaviour : MonoBehaviour
{
    private long score;
    public ComboBehaviour CombpBehaviour;
    private CubicTextMesh mesh;

    private void Awake()
    {
        mesh = GetComponent<CubicTextMesh>();
    }

    public void UpdateScore()
    {
        int baseScore = 2;
        int score = (int)Math.Pow(baseScore,CombpBehaviour.Combo);
        this.score+=score;
        CombpBehaviour.AddCombo();
        mesh.Text = this.score.ToString();
    }

    public void Clear()
    {
        this.score = 0;
        mesh.Text = score.ToString();
    }
}
