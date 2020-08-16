using UnityEngine;

namespace Tetris
{
   public class LivesBehaviour : MonoBehaviour
   {
       [SerializeField]
       private GameFieldBehaviour game;
       //[SerializeField]
       //private SpawnerBehaviour spawner;

       private CubicTextMesh _mesh;
       private int _lives;
   
       private void Awake()
       {
           _mesh = GetComponent<CubicTextMesh>();
           game.OnLifeRequested += GiveLife;
       }
   
       private void Start()
       {
           this._lives = 2;
           UpdateText();
       }
   
       public void AddLives(int livesToAdd)
       {
           this._lives += livesToAdd;
           UpdateText();
       }
   
       public bool GiveLife()
       {
           if (_lives > 1)
           {
               _lives--;
               UpdateText();
               return true;
           }
           else
               return false;
       }
   
       private void UpdateText()
       {
           if (_lives > 1)
           {
               _mesh.Text = "px" + this._lives;
               float s = 1f / _mesh.Text.Length;
               this.transform.localScale = new Vector3(s,s,s);
           }
           else
           {
               _mesh.Text = "";
           }
       }
       
   }
 
}
