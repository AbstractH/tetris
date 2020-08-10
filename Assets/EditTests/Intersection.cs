using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class Intersection
    {
        public delegate R Request<R>();
        private class GameField
        {
            public Action<Figure> OnFigureFallen;
            public Request< Figure > OnFigureNeeded;

            public GameField()
            {
                
            }

            public void Run()
            {
                Figure f = new Figure("new");
                f=OnFigureNeeded();
                OnFigureFallen(f);
            }
        }

        private class Spawner
        {
            public GameField game;
            private string name;
            private Stack<Figure> next;

            public Spawner(GameField game, string name)
            {
                this.game = game;
                this.name = name;
                next = new Stack<Figure>();
                next.Push(new Figure(name+" S"));
                next.Push(new Figure(name+" I"));
                next.Push(new Figure(name+" R"));
                next.Push(new Figure(name+" T"));
                next.Push(new Figure(name+" E"));
                next.Push(new Figure(name+" T"));
                game.OnFigureFallen += GiveFigure;
                game.OnFigureNeeded += TakeFigure;
            }

            public Figure TakeFigure()
            {
                Debug.Log("Take figure from "+ this.name);
                return next.Pop();
            }

            public void GiveFigure(Figure f)
            {
                Debug.Log(f);
            }
        }

        private class Figure
        {
            private string name;

            public Figure(string name)
            {
                this.name = name;
            }

            public override string ToString()
            {
                return "Figure " + name;
            }
        }

        [Test]
        public void IntersectionSimplePasses()
        {
            GameField game = new GameField();
            Spawner spawner1 = new Spawner(game,"AAA");
            Spawner spawner2 = new Spawner(game,"BBB");
            game.Run();
        }
        
    }
}
