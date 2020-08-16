using System;
using UnityEngine;

namespace Tetris
{
    public class ScoreBehaviour : MonoBehaviour
    {
        private long _score;
        public ComboBehaviour comboBehaviour;
        private CubicTextMesh _mesh;

        private void Awake()
        {
            _mesh = GetComponent<CubicTextMesh>();
        }

        public void UpdateScore()
        {
            int baseScore = 2;
            int score = (int)Math.Pow(baseScore,comboBehaviour.Combo);
            _score+=score;
            comboBehaviour.AddCombo();
            _mesh.Text = this._score.ToString();
        }

        public void Clear()
        {
            this._score = 0;
            _mesh.Text = _score.ToString();
        }
    }

}
