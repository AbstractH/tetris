using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Tetris
{
   public class GameFieldBehaviour : MonoBehaviour
{
    public delegate TResult Request<out TResult>();
    
    [SerializeField]
    private int height;
    [SerializeField]
    private int width;
    public int Height => height;
    public int Width => width;
    public GameObject cellPrefab;
    public Material filledMaterial;
    public Material emptyMaterial;
    public Material figureMaterial;
    private Cell[,] _cells;
    private Dictionary<Cell, Renderer> _screenCells;
    private Figure _current;
    private bool _isGameStarted;
    private CubicTextMesh _gameOverText;
    
    public ScoreBehaviour score;
    public LevelBehaviour level;
    public ComboBehaviour combo;
    public event Action OnGameOver = delegate { };
    public event Request<Figure> OnFigureNeeded = () => null;
    public event Action<Figure> OnFigureFallen = delegate {  };
    public event Request<bool> OnLifeRequested = () => false;

    public enum Direction
    {
        Left, Right, Down
    }

    private void Awake()
    {
        _gameOverText = GetComponentInChildren<CubicTextMesh>();
    }

    void Start()
    {
        Init();
    }
    
    void Update()
    {
        if (_isGameStarted) HandleControls();    
        Display();
    }

    private void HandleControls()
    {
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(nameof(MoveLeft));
        if(Input.GetKeyUp(KeyCode.A))
            StopCoroutine(nameof(MoveLeft));
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(nameof(MoveRight));
        if (Input.GetKeyUp(KeyCode.D))
            StopCoroutine(nameof(MoveRight));
        if (Input.GetKeyDown(KeyCode.S))
            StartCoroutine(nameof(MoveDown));
        if (Input.GetKeyUp(KeyCode.S))
            StopCoroutine(nameof(MoveDown));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            while (CheckIfPositionAllowed(_current, Direction.Down))
            {
                _current.Move(Direction.Down);
            }
            HandleFallenFigure();
        }

        if (Input.GetKeyDown(KeyCode.Q))
            Rotate(Direction.Left);
        if (Input.GetKeyDown(KeyCode.E))
            Rotate(Direction.Right);
    }

    private void Rotate(Direction direction)
    {
        if (CheckIfRotationAllowed(_current, direction))
        {
            _current.Rotate(direction);
        }
        else
        {
            if (CheckIfRotationAllowed(_current.Clone().Move(Direction.Left), direction))
            {
                _current.Move(Direction.Left).Rotate(direction);
            }
            else if(CheckIfRotationAllowed(_current.Clone().Move(Direction.Right), direction))
            {
                _current.Move(Direction.Right).Rotate(direction);
            }
        }
    }

    IEnumerator MoveLeft()
    {
        float delay = 0.2f;

        while (true)
        {
            if(CheckIfPositionAllowed(_current, Direction.Left))
                _current.Move(Direction.Left);
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
            if(CheckIfPositionAllowed(_current, Direction.Right))
                _current.Move(Direction.Right);
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
            if(CheckIfPositionAllowed(_current, Direction.Down))
                _current.Move(Direction.Down);
            yield return new WaitForSeconds(delay);
            if (delay > 0.1f)
                delay /= 2;
        }
    }

    IEnumerator GameOver()
    {
        StopCoroutine(nameof(Game));
        _isGameStarted = false;
        this._gameOverText.Text = "C";
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
                Renderer screenCell = _screenCells[_cells[i, j]];
                if (_cells[i, j].IsFilled)
                {
                    screenCell.material = filledMaterial;
                    screenCell.transform.localScale = new Vector3(1f,1f,1f);
                }
                else if (DoesCellContainFigure(_cells[i,j]))
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
        if (_current == null) return false;
        foreach (Cell figureCell in _current.GetCells())
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
            catch (CantFallException)
            {
                HandleFallenFigure();
            }
            
            yield return new WaitForSeconds(level.GameTickDelay1);   
        }
    }

    private void HandleFallenFigure()
    {
        OnFigureFallen?.Invoke(_current);
        Fill();
        ClearLines();
        Figure next = OnFigureNeeded?.Invoke();
        if (next != null && CheckIfPositionAllowed(next, next.Position))
            _current = next;
        else
            if(!SpendLife())
                StartCoroutine(nameof(GameOver));
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
        foreach (Cell c in _cells)
        {
            c.IsFilled = false;
        }
    }
    
    class CantFallException : Exception{};

    private void Fall() 
    {
        Figure clone = _current.Clone();
        clone.Move(Direction.Down);
        if (CheckIfPositionAllowed(clone, clone.Position))
            _current.Move(Direction.Down);
        else
            throw new CantFallException();
    }
    private void Fill()
    {
        foreach (Cell fieldCell in _cells)
        {
            foreach (Cell figureCell in _current.GetCells())
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
                lineIsFilled &= _cells[i, j].IsFilled ;
            }

            if (lineIsFilled)
            {
                for (int h = i; h < height - 1; h++)
                for (int j = 0; j < width; j++)
                    _cells[h, j].IsFilled = _cells[h + 1, j].IsFilled;
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
            foreach (Cell fieldCell in this._cells)
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

    protected void Init()
    {
        _cells = new Cell[height,width];
        _screenCells = new Dictionary<Cell, Renderer>();
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                _cells[i,j] = new Cell(new Vector2(j,i));
                GameObject screenCell = Instantiate(
                    cellPrefab,
                    new Vector3(j, i, 0), 
                    Quaternion.identity);
                Renderer cellRenderer = screenCell.GetComponent<Renderer>();
                ParticleSystem cellParticle = screenCell.GetComponent<ParticleSystem>();
                screenCell.transform.parent = transform;
                _screenCells.Add(_cells[i,j],cellRenderer);
            }
        }
        
    }

    private void PrepareForNewGame()
    {
        StopAllCoroutines();
        _gameOverText.Text = "";
        foreach (Cell c in _cells)
            c.Clear();
        
        level.Clear();
        combo.Clear();
        score.Clear();
    }
    
    public void NewGame()
    {
        _isGameStarted = true;
        PrepareForNewGame();
        if(_current==null) 
            _current = OnFigureNeeded?.Invoke();
        StartCoroutine(nameof(Game));
    }

    
}
 
}
