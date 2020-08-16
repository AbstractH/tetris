using UnityEngine;

namespace Tetris
{
    
    [ExecuteInEditMode]
    public class GameFieldEditor : GameFieldBehaviour
    {
        void Start()
        {
            Debug.Log("Editor start");
            base.Init();
        }

        void Update()
        {
            Debug.Log("Editor update");
        }
    }

}
