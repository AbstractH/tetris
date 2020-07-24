using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerBehaviour : MonoBehaviour
{
    private Vector2 spawnPosition;
    private Vector3 globalPosition;
    private Stack<Figure> next;
    private FigureMesh mesh;

    private void Awake()
    {
        mesh = GetComponent<FigureMesh>();
        globalPosition = this.transform.position;
    }

    public void init(Vector2 position)
    {
        this.spawnPosition = position;
        next = new Stack<Figure>();
        FillStack();
        mesh.SetFigure(PeekNext());
        transform.position = globalPosition;
    }

    private void FillStack()
    {
        next.Push(Figure.GenerateRandom());
    }

    
    public Figure PeekNext()
    {
        return next.Peek();
    }
        
    public Figure PopNext()
    {
        Figure res = next.Pop();
        if (next.Count == 0)
        {
            FillStack();
        }
        mesh.SetFigure(PeekNext());
        transform.position = globalPosition;
        res.Move(spawnPosition);
        return res;
    }

}
