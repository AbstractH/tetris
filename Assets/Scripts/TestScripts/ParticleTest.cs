using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    public CellBehaviour[] cells;
    void Start()
    {
        
    }

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
