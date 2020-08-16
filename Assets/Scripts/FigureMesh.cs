using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class FigureMesh : MonoBehaviour
    {
        public GameObject cube;
        private List<GameObject> _cubes;
        private Rigidbody _rb;
    
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _cubes = new List<GameObject>();
        }
    
        public void SetFigure(Figure f)
        {
            foreach (GameObject c in _cubes)
            {
                Destroy(c);
            }
            _cubes.Clear();
            foreach (Cell c in f.GetCells())
            {
                GameObject a = Instantiate(cube, this.transform);
                a.transform.localPosition = Vector3.zero + new Vector3(c.Position.x,c.Position.y,0f);
                _cubes.Add(a);
            }
            _rb.angularVelocity = new Vector3(0f,0f,4f);
        }
    
        public void SetFigure(List<Figure> fl)
        {
            foreach (GameObject c in _cubes)
            {
                Destroy(c);
            }
            _cubes.Clear();
            foreach (Figure f in fl)
            {
                foreach (Cell c in f.GetCells())
                {
                    GameObject a = Instantiate(cube, this.transform);
                    a.transform.localPosition = Vector3.zero + new Vector3(c.Position.x,c.Position.y,0f);
                    _cubes.Add(a);
                }   
            }
            _rb.angularVelocity = new Vector3(0f,0f,0f);
            transform.rotation = Quaternion.identity;
        }
    
    }

}
