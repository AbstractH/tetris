using UnityEngine;

namespace Tetris
{
   public class LevelBehaviour : MonoBehaviour
   {
       private static readonly float _GAME_TICK_SLOWER = 0.5f;
       private int _level;
       private int _lines;
       private int _slowCounter;
       private float _gameTickDelay;
       private CubicTextMesh _mesh;
   
       public float GameTickDelay1 => _gameTickDelay;
   
       public void DecrementSpeed(int volume)
       {
           this._slowCounter += volume;
           CalculateGameTick();
       }
   
       private void Awake()
       {
           _mesh = GetComponent<CubicTextMesh>();
       }
   
       public void UpdateProgress()
       {
           this._lines++;
           this._level = (int) (_lines / 10.0f)+1;
           CalculateGameTick();
           _mesh.Text = _level.ToString();
           float s = 1f / _level.ToString().Length;
           this.transform.localScale = new Vector3(s,s,s);
       }
   
       private void CalculateGameTick()
       {
           float newTick = 2f - 1.5f / 10f * _level + _slowCounter*_GAME_TICK_SLOWER;
           this._gameTickDelay = (newTick>=0.5f)?newTick:0.5f;
       }
   
       public void Clear()
       {
           this._lines = 0;
           this._slowCounter = 0;
           this._level = 1;
           this._gameTickDelay = 1.5f / (float)_level;
           _mesh.Text = _level.ToString();
       }
       
   }
 
}
