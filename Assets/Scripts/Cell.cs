using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private Vector2 position ;
    private bool isFilled;

    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }

    public bool IsFilled
    {
        get { return isFilled; }
        set { isFilled = value; }
    }

    public Cell(Vector2 position)
    {
        this.position = position;
        this.isFilled = false;
    }

    public void Fill()
    {
        isFilled = true;
    }

    public void Clear()
    {
        isFilled = false;
    }

    public bool IsOnSamePosition(Cell cell)
    {
        return this.position.Equals(cell.position);
    } 
}
