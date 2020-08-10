using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerBehaviour : MonoBehaviour
{
    public GameFieldBehaviour game;
    public LivesBehaviour lives;
    
    private Vector2 spawnPosition;
    private Vector3 globalPosition;
    private Queue<Figure> next;
    private Queue<Analizeable> analiers;
    private FigureMesh mesh;
    private Queue<HeartTask> tasks;

    private void Awake()
    {
        mesh = GetComponent<FigureMesh>();
        globalPosition = this.transform.position;
        tasks = new Queue<HeartTask>();
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
        if (analiers.Count>0)
        {
            Analizeable analizer = analiers.Dequeue();
            if (analizer == null) return;
            analizer.UpdateState(f);
            try
            {
                if (!analizer.NeedsMoreData()) return;
                // tasks.Dequeue();
            }
            catch (HeartReward r)
            {  
                lives.AddLives(r.Volume);
                //tasks.Dequeue();
            }
        }
    }

    public void init(Vector2 position)
    {
        this.spawnPosition = position;
        next = new Queue<Figure>();
        analiers = new Queue<Analizeable>();
        FillQueue();
        mesh.SetFigure(PeekNext());
        transform.position = globalPosition;
    }

    private void FillQueue()
    {
        switch (Random.Range(1,5))
        {
            case 1:
                GenerateHeartTask();
                break;
            default: 
                next.Enqueue(Figure.GenerateRandom()); 
                analiers.Enqueue(new EmptyTask());
                break;
        }
    }

    private void GenerateHeartTask()
    {
        HeartTask newTask = new HeartTask();
        tasks.Enqueue(newTask);
        foreach (Figure f in newTask.Figures)
        {
            next.Enqueue(f);
            analiers.Enqueue(newTask);
        }
    }
    
    public Figure PeekNext()
    {
        return next.Peek();
    }
        
    public Figure PopNext()
    {
        Figure res = next.Dequeue();
        if (next.Count == 0)
        {
            FillQueue();
        }
        mesh.SetFigure(PeekNext());
        transform.position = globalPosition;
        res.Move(spawnPosition);
        return res;
    }

    public class HeartTask : Analizeable
    {
        private List<Figure> state;
        private List<Figure> goal;
        private List<Figure> figures;

        public List<Figure> Figures
        {
            get { return figures; }
            set { figures = value; }
        }

        public HeartTask()
        {
            state = new List<Figure>();
            goal = new List<Figure>();
            figures = new List<Figure>();
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
                .Move(Vector2.up*2 + Vector2.right*2)
                .Rotate(GameFieldBehaviour.Direction.RIGHT)
            );
            
        }

        public void UpdateState(Figure f)
        {
            state.Add(f);
        }

        /// <summary>
        /// Trows reward
        /// </summary>
        /// <returns>true if need more figures to make analise</returns>
        public bool NeedsMoreData()
        {
            if (state.Count == goal.Count)
            {
                if (IsGoalArchived())
                {
                    throw new HeartReward(2);
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
    }

    public class HeartReward : Reward
    {
        public HeartReward(int hearts) : base(hearts) { }
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
    
    private interface Analizeable
    {
        void UpdateState(Figure f);
        bool NeedsMoreData();
    }

    private class EmptyTask : Analizeable
    {
        public void UpdateState(Figure f)
        {
            return;
        }

        public bool NeedsMoreData()
        {
            return false;
        }
    }
}
