using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerBehaviour : MonoBehaviour
{
    public GameFieldBehaviour game;
    public LivesBehaviour lives;
    public LevelBehaviour level;
    
    private Vector2 spawnPosition;
    private Vector3 globalPosition;
    private Queue<Figure> next;
    private Queue<Task> tasks;
    private FigureMesh mesh;

    private void Awake()
    {
        mesh = GetComponent<FigureMesh>();
        globalPosition = this.transform.position;
        game.OnFigureNeeed = PopNext;
        game.OnFigureFallen += OnFigureFallen;
    }

    private void Start()
    {
        init(new Vector2((int)(game.width/2), (int)(game.height)-2));
    }

    private void OnFigureFallen(Figure f)
    {
        //check for task
        if (tasks.Count>0)
        {
            Task task = tasks.Dequeue();
            if (task == null) return;
            task.UpdateState(f);
            try
            {
                if (!task.NeedsMoreData()) return;
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

    public void init(Vector2 position)
    {
        this.spawnPosition = position;
        next = new Queue<Figure>();
        tasks = new Queue<Task>();
        FillQueue();
        mesh.SetFigure(PeekNext());
        transform.position = globalPosition;
    }

    private void FillQueue()
    {
        // GenerateHeartTask();
        // return;
        switch (Random.Range(1,20))
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
                next.Enqueue(Figure.GenerateRandom()); 
                tasks.Enqueue(new EmptyTask());
                break;
        }
    }

    private void GenerateSpeedTask()
    {
        Task newTask = new SpeedDownTask();
        foreach (Figure f in newTask.Figures)
        {
            next.Enqueue(f);
            tasks.Enqueue(newTask);
        }
    }
    private void GenerateHeartTask()
    {
        Task newTask = new HeartTask();
        foreach (Figure f in newTask.Figures)
        {
            next.Enqueue(f);
            tasks.Enqueue(newTask);
        }
    }
    
    public Figure PeekNext()
    {
        return next.Peek();
    }

    private void ShowFigure()
    {
        try
        {
            List<Figure> res;
            Task currentTask = null;
            Task nextTask = null;
            currentTask = tasks.Peek();
            if (!(currentTask is EmptyTask))
            {
                res = currentTask.Goal;
                mesh.SetFigure(res);
            }
            else if (!(
                (nextTask=tasks.AsEnumerable().ElementAt(1)) 
                    is 
                EmptyTask))
            {
                res = nextTask.Goal;
                mesh.SetFigure(res);
            }else
                throw new Exception();
        }
        catch(Exception e)
        {
            mesh.SetFigure(PeekNext());
        }
        
    }
    public Figure PopNext()
    {
        Figure res = next.Dequeue();
        if (next.Count == 0)
        {
            FillQueue();
        }
        ShowFigure();
        transform.position = globalPosition;
        res.Move(spawnPosition);
        return res;
    }

    private class SpeedDownTask : Task
    {
        public SpeedDownTask() : base()
        {
            Figure f1 = Figure.GenerateTFigure();
            Figure f2 = Figure.GenerateTFigure();
            figures.Add(f1);
            figures.Add(f2);
            goal.Add(f1
                .Clone()
                .Move(Vector2.zero)
                .Rotate(GameFieldBehaviour.Direction.LEFT)
            );
            goal.Add(f2
                .Clone()
                .Move(Vector2.left*2)
                .Rotate(GameFieldBehaviour.Direction.LEFT)
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
                .Rotate(GameFieldBehaviour.Direction.LEFT)
                .Rotate(GameFieldBehaviour.Direction.LEFT)
            );
            goal.Add(f2
                .Clone()
                .Move(Vector2.up)
            );
            goal.Add(f3
                .Clone()
                .Move(Vector2.up*2+Vector2.left*2)
                .Rotate(GameFieldBehaviour.Direction.RIGHT)
            );
            goal.Add(f4
                .Clone()
                .Move(Vector2.up*2 + Vector2.right)
                .Rotate(GameFieldBehaviour.Direction.RIGHT)
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
        private int volume;

        public int Volume
        {
            get { return volume; }
            private set {}
        }

        public Reward(int volume)
        {
            this.volume = volume;
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

        public Task()
        {
            state = new List<Figure>();
            goal = new List<Figure>();
            figures = new List<Figure>();
        }

        /// <summary>
        /// Throws reward if goal archived
        /// </summary>
        /// <returns>true if need more figures to make analise</returns>
        public virtual void UpdateState(Figure f)
        {
            state.Add(f);
        }

        public virtual bool NeedsMoreData()
        {
            if (state.Count == goal.Count)
            {
                if (IsGoalArchived())
                {
                    throw OnTaskCompleted();
                }
                else
                {
                    return false;
                }
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
            return;
        }

        public override bool NeedsMoreData()
        {
            return false;
        }

        protected override Reward OnTaskCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
