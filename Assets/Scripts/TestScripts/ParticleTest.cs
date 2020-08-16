
using UnityEngine;

namespace Tetris
{
    public class ParticleTest : MonoBehaviour
    {
        public CellBehaviour[] cells;
        void Update()
        {
            if (Input.GetKeyDown("space"))
            {
                foreach (CellBehaviour cell in cells)
                {
                    cell.Explode();
                }
            }
        }
    }

}
