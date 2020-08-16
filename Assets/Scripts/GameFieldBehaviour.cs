using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class GameFieldBehaviour : MonoBehaviour
{
    public delegate RESULT Request<RESULT>();
    
    public int height;
    public int width;
    public GameObject cellPrefab;
    public Material filledMaterial;
    public Material emptyMaterial;
    public Material figureMaterial;
    private Cell[,] cells;
    private Dictionary<Cell, Renderer> screenCells;
    private Dictionary<Cell, ParticleSystem> particles;
    private Figure current;
    private bool isGameStarted;
    private CubicTextMesh gg;
    
    public ScoreBehaviour score;
    public LevelBehaviour level;
    public ComboBehaviour combo;
    public Action OnGameOver;
    public Request<Figure> OnFigureNeeed;
    public Action<Figure> OnFigureFallen;
    public Request<bool> OnLifeRequested;

    public enum Direction
    {
        LEFT, RIGHT, DOWN
    }

    private void Awake()
    {
        this.gg = GetComponentInChildren<CubicTextMesh>();
    }

    void Start()
    {
        init();
    }
    
    void Update()
    {
        if (isGameStarted) HandleControls();    
        Display();
    }

    private void HandleControls()
    {
        if (Input.GetKeyDown("a"))
            StartCoroutine("MoveLeft");
        if(Input.GetKeyUp("a"))
            StopCoroutine("MoveLeft");
        if (Input.GetKeyDown("d"))
            StartCoroutine("MoveRight");
        if (Input.GetKeyUp("d"))
            StopCoroutine("MoveRight");
        if (Input.GetKeyDown("s"))
            StartCoroutine("MoveDown");
        if (Input.GetKeyUp("s"))
            StopCoroutine("MoveDown");

        if (Input.GetKeyDown("space"))
        {
            while (CheckIfPositionAllowed(current, Direction.DOWN))
            {
                current.Move(Direction.DOWN);
            }
            HandleFallenFigure();
        }

        if (Input.GetKeyDown("q"))
            Rotate(Direction.LEFT);
        if (Input.GetKeyDown("e"))
            Rotate(Direction.RIGHT);
    }

    private void Rotate(Direction direction)
    {
        if (CheckIfRotationAllowed(current, direction))
        {
            current.Rotate(direction);
        }
        else
        {
            if (CheckIfRotationAllowed(current.Clone().Move(Direction.LEFT), direction))
            {
                current.Move(Direction.LEFT).Rotate(direction);
            }
            else if(CheckIfRotationAllowed(current.Clone().Move(Direction.RIGHT), direction))
            {
                current.Move(Direction.RIGHT).Rotate(direction);
            }
        }
    }

    IEnumerator MoveLeft()
    {
        float delay = 0.2f;

        while (true)
        {
            if(CheckIfPositionAllowed(current, Direction.LEFT))
                current.Move(Direction.LEFT);
            yield return new WaitForSeconds(delay);
            if (delay > 0.01f)
                delay /= 4f;
        }
    }
    
    IEnumerator MoveRight()
    {
        float delay = 0.2f;

        while (true)
        {
            if(CheckIfPositionAllowed(current, Direction.RIGHT))
                current.Move(Direction.RIGHT);
            yield return new WaitForSeconds(delay);
            if (delay > 0.01f)
                delay /= 4f;
        }
    }
    
    IEnumerator MoveDown()
    {
        float delay = 0.3f;

        while (true)
        {
            if(CheckIfPositionAllowed(current, Direction.DOWN))
                current.Move(Direction.DOWN);
            yield return new WaitForSeconds(delay);
            if (delay > 0.1f)
                delay /= 2;
        }
    }

    IEnumerator GameOver()
    {
        StopCoroutine("Game");
        isGameStarted = false;
        this.gg.Text = "C";
        yield return new WaitForSeconds(2f);
        OnGameOver?.Invoke();
        yield return new WaitForSeconds(2f);
        PrepareForNewGame();
    }

    private void Display()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Renderer screenCell = screenCells[cells[i, j]];
                if (cells[i, j].IsFilled)
                {
                    screenCell.material = filledMaterial;
                    screenCell.transform.localScale = new Vector3(1f,1f,1f);
                }
                else if (DoesCellContainFigure(cells[i,j]))
                {
                    screenCell.material = figureMaterial;
                    screenCell.transform.localScale = new Vector3(0.7f,0.7f,0.7f);

                }
                else
                {
                    screenCell.material = emptyMaterial;
                    screenCell.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
                }
                    
            }
        }
    }

    private bool DoesCellContainFigure(Cell cell)
    {
        if (current == null) return false;
        foreach (Cell figureCell in current.GetCells())
        {
            if (figureCell.IsOnSamePosition(cell))
                return true;
        }
        return false;
    }

    IEnumerator Game()
    {
        while (true)
        {
            try
            {
                Fall();
            }
            catch (CantFallException e)
            {
                HandleFallenFigure();
            }
            
            yield return new WaitForSeconds(level.GameTickDelay1);   
        }
    }

    private void HandleFallenFigure()
    {
        OnFigureFallen?.Invoke(current);
        Fill();
        ClearLines();
        Figure next = OnFigureNeeed?.Invoke();
        if (CheckIfPositionAllowed(next, next.Position))
            current = next;
        else
            if(!SpendLife())
                StartCoroutine("GameOver");
    }

    private bool SpendLife()
    {
        if (OnLifeRequested())
        {
            ClearField();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ClearField()
    {
        foreach (Cell c in cells)
        {
            c.IsFilled = false;
        }
    }
    
    class CantFallException : Exception{};

    private void Fall() 
    {
        Figure clone = current.Clone();
        clone.Move(Direction.DOWN);
        if (CheckIfPositionAllowed(clone, clone.Position))
            current.Move(Direction.DOWN);
        else
            throw new CantFallException();
    }
    private void Fill()
    {
        foreach (Cell fieldCell in cells)
        {
            foreach (Cell figureCell in current.GetCells())
            {
                if (figureCell.IsOnSamePosition(fieldCell))
                {
                    fieldCell.Fill();
                }
            }
        }
    }

    private void ClearLines()
    {
        for (int i = 0; i < height-1; i++)
        {
            bool lineIsFilled = true;
            for (int j = 0; j < width; j++)
            {
                lineIsFilled &= cells[i, j].IsFilled ;
            }

            if (lineIsFilled)
            {
                for (int j = 0; j < width; j++)
                {
                    ParticleSystem p = particles[cells[i, j]];
                    p.Play();
                }
                for (int h = i; h < height - 1; h++)
                for (int j = 0; j < width; j++)
                    cells[h, j].IsFilled = cells[h + 1, j].IsFilled;
                i--;
                score.UpdateScore();
                level.UpdateProgress();
            }
        }
    }
    private bool CheckIfRotationAllowed(Figure figure, Direction direction)
    {
        List<Cell> cells =  figure.Clone().Rotate(direction).GetCells();
        return CheckCells(cells);
    }
    private bool CheckIfPositionAllowed(Figure figure, Vector2 position)
    {
        List<Cell> cells =  figure.Clone().Move(position).GetCells();
        return CheckCells(cells);
    }
    private bool CheckIfPositionAllowed(Figure figure, Direction direction)
    {
        List<Cell> cells =  figure.Clone().Move(direction).GetCells();
        return CheckCells(cells);
    }

    private bool CheckCells(List<Cell> cells)
    {
        foreach (Cell figureCell in cells)
        {
            bool found = false;
            foreach (Cell fieldCell in this.cells)
            {
                if (figureCell.IsOnSamePosition(fieldCell))
                {
                    found = true;
                    if (fieldCell.IsFilled) return false;
                }
            }

            if (!found) return false;
        }
        return true;
    }

    protected void init()
    {
        cells = new Cell[height,width];
        screenCells = new Dictionary<Cell, Renderer>();
        particles = new Dictionary<Cell, ParticleSystem>();
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                cells[i,j] = new Cell(new Vector2(j,i));
                GameObject screenCell = Instantiate(
                    cellPrefab,
                    new Vector3(j, i, 0), 
                    Quaternion.identity);
                Renderer cellRenderer = screenCell.GetComponent<Renderer>();
                ParticleSystem cellParticle = screenCell.GetComponent<ParticleSystem>();
                screenCell.transform.parent = transform;
                screenCells.Add(cells[i,j],cellRenderer);
                particles.Add(cells[i,j],cellParticle);
            }
        }
        
    }

    private void PrepareForNewGame()
    {
        StopAllCoroutines();
        this.gg.Text = "";
        foreach (Cell c in cells)
            c.Clear();
        
        level.Clear();
        combo.Clear();
        score.Clear();
    }
    
    public void NewGame()
    {
        isGameStarted = true;
        PrepareForNewGame();
        if(current==null) 
            current = OnFigureNeeed?.Invoke();
        StartCoroutine("Game");
    }

    
}
