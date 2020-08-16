using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tetris
{
   public class SpawnerBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameFieldBehaviour game;
    [SerializeField]
    private LivesBehaviour lives;
    [SerializeField]
    private LevelBehaviour level;
    
    private Vector2 _spawnPosition;
    private Vector3 _globalPosition;
    private Queue<Figure> _next;
    private Queue<Task> _tasks;
    private FigureMesh _mesh;

    private void Awake()
    {
        _mesh = GetComponent<FigureMesh>();
        _globalPosition = this.transform.position;
        game.OnFigureNeeded += PopNext;
        game.OnFigureFallen += OnFigureFallen;
    }

    private void Start()
    {
        Init(new Vector2((int)(game.Width/2), (int)(game.Height)-2));
    }

    private void OnFigureFallen(Figure f)
    {
        //check for task
        if (_tasks.Count>0)
        {
            Task task = _tasks.Dequeue();
            if (task == null) return;
            task.UpdateState(f);
            try
            {
                if (!task.DoesItNeedMoreData()) return;
            }
            catch (HeartReward r)
            {
                lives.AddLives(r.Volume);
            }
            catch (SpeedDownReward r)
            {
                level.DecrementSpeed(r.Volume);
            }
        }
    }

    private void Init(Vector2 position)
    {
        this._spawnPosition = position;
        _next = new Queue<Figure>();
        _tasks = new Queue<Task>();
        FillQueue();
        _mesh.SetFigure(PeekNext());
        transform.position = _globalPosition;
    }

    private void FillQueue()
    {
        switch (UnityEngine.Random.Range(1,20))
        {
            case 1:
                GenerateHeartTask();
                break;
            case 2:
            case 3:
            case 4:
                GenerateSpeedTask();
                break;
            default: 
                _next.Enqueue(Figure.GenerateRandom()); 
                _tasks.Enqueue(new EmptyTask());
                break;
        }
    }

    private void GenerateSpeedTask()
    {
        Task newTask = new SpeedDownTask();
        foreach (Figure f in newTask.Figures)
        {
            _next.Enqueue(f);
            _tasks.Enqueue(newTask);
        }
    }
    private void GenerateHeartTask()
    {
        Task newTask = new HeartTask();
        foreach (Figure f in newTask.Figures)
        {
            _next.Enqueue(f);
            _tasks.Enqueue(newTask);
        }
    }
    
    public Figure PeekNext()
    {
        return _next.Peek();
    }

    private void ShowFigure()
    {
        try
        {
            List<Figure> res;
            Task currentTask = null;
            Task nextTask = null;
            currentTask = _tasks.Peek();
            if (!(currentTask is EmptyTask))
            {
                res = currentTask.Goal;
                _mesh.SetFigure(res);
            }
            else if (!(
                (nextTask=_tasks.AsEnumerable().ElementAt(1)) 
                    is 
                EmptyTask))
            {
                res = nextTask.Goal;
                _mesh.SetFigure(res);
            }else
                throw new Exception();
        }
        catch(Exception)
        {
            _mesh.SetFigure(PeekNext());
        }
        
    }
    public Figure PopNext()
    {
        Figure res = _next.Dequeue();
        if (_next.Count == 0)
        {
            FillQueue();
        }
        ShowFigure();
        transform.position = _globalPosition;
        res.Move(_spawnPosition);
        return res;
    }

    private class SpeedDownTask : Task
    {
        public SpeedDownTask() : base()
        {
            Figure f1 = Figure.GenerateTFigure();
            Figure f2 = Figure.GenerateTFigure();
            Figures.Add(f1);
            figures.Add(f2);
            goal.Add(f1
                .Clone()
                .Move(Vector2.zero)
                .Rotate(GameFieldBehaviour.Direction.Left)
            );
            goal.Add(f2
                .Clone()
                .Move(Vector2.left*2)
                .Rotate(GameFieldBehaviour.Direction.Left)
            );
        }

        protected override Reward OnTaskCompleted()
        {
            return new SpeedDownReward(1);
        }
    }

    private class HeartTask : Task
    {
        
        public HeartTask() : base()
        {
            Figure f1 = Figure.GenerateTFigure();
            Figure f2 = Figure.GenerateTFigure();
            Figure f3 = Figure.GenerateNFigure();
            Figure f4 = Figure.GenerateИFigure();
            figures.Add(f1);
            figures.Add(f2);
            figures.Add(f3);
            figures.Add(f4);
            goal.Add(f1
                .Clone()
                .Move(Vector2.zero)
                .Rotate(GameFieldBehaviour.Direction.Left)
                .Rotate(GameFieldBehaviour.Direction.Left)
            );
            goal.Add(f2
                .Clone()
                .Move(Vector2.up)
            );
            goal.Add(f3
                .Clone()
                .Move(Vector2.up*2+Vector2.left*2)
                .Rotate(GameFieldBehaviour.Direction.Right)
            );
            goal.Add(f4
                .Clone()
                .Move(Vector2.up*2 + Vector2.right)
                .Rotate(GameFieldBehaviour.Direction.Right)
            );
            
        }

        protected override Reward OnTaskCompleted()
        {
            return new HeartReward(2);
        }
    }

    public class HeartReward : Reward
    {
        public HeartReward(int hearts) : base(hearts) { }
    }
    public class SpeedDownReward : Reward
    {
        public SpeedDownReward(int speedDown) : base(speedDown) { }
    }

    public class Reward : Exception
    {
        private int _volume;

        public int Volume
        {
            get { return _volume; }
            private set {}
        }

        public Reward(int volume)
        {
            this._volume = volume;
        }
    }
    
    private abstract class Task
    {
        protected List<Figure> state;
        protected List<Figure> goal;
        protected List<Figure> figures;
        public List<Figure> Figures
        {
            get { return figures; }
            set { figures = value; }
        }

        public List<Figure> Goal => goal;

        protected Task()
        {
            state = new List<Figure>();
            goal = new List<Figure>();
            figures = new List<Figure>();
        }
        public virtual void UpdateState(Figure f)
        {
            state.Add(f);
        }

        /// <summary>
        /// Throws reward if goal archived
        /// </summary>
        /// <returns>true if need more figures to make analise, false if analysing one</returns>
        public virtual bool DoesItNeedMoreData()
        {
            if (state.Count == goal.Count)
            {
                if (IsGoalArchived())
                {
                    throw OnTaskCompleted();
                }

                return false;
            }

            return true;
        }
        
        public bool IsGoalArchived()
        {
            return Figure.AreSame(goal,state);
        }

        protected abstract Reward OnTaskCompleted();
    }

    private class EmptyTask : Task
    {
        public override void UpdateState(Figure f)
        {
        }

        public override bool DoesItNeedMoreData()
        {
            return false;
        }

        protected override Reward OnTaskCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
 
}
