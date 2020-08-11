using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureMesh : MonoBehaviour
{
    public GameObject cube;
    private List<GameObject> cubes;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cubes = new List<GameObject>();
    }

    public void SetFigure(Figure f)
    {
        foreach (GameObject c in cubes)
        {
            Destroy(c);
        }
        cubes.Clear();
        foreach (Cell c in f.GetCells())
        {
            GameObject a = Instantiate(cube, this.transform);
            a.transform.localPosition = Vector3.zero + new Vector3(c.Position.x,c.Position.y,0f);
            cubes.Add(a);
        }
        rb.angularVelocity = new Vector3(0f,0f,4f);
    }

    public void SetFigure(List<Figure> fl)
    {
        foreach (GameObject c in cubes)
        {
            Destroy(c);
        }
        cubes.Clear();
        foreach (Figure f in fl)
        {
            foreach (Cell c in f.GetCells())
            {
                GameObject a = Instantiate(cube, this.transform);
                a.transform.localPosition = Vector3.zero + new Vector3(c.Position.x,c.Position.y,0f);
                cubes.Add(a);
            }   
        }
        rb.angularVelocity = new Vector3(0f,0f,0f);
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        
    }
}
