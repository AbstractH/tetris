using System;
using System.Collections.Generic;
using UnityEngine;
using Direction = Tetris.GameFieldBehaviour.Direction;

namespace Tetris
{
    public class Figure
    {
        public Vector2 Position => _position;
        public int Rotation => _rotation;
        
        
        private List<Cell> _cells;
        private Vector2 _position;
        private int _rotation;

        public Figure()
        {
            _cells = new List<Cell>();
            _position = Vector2.zero;
            _rotation = 0;
        }

        public Figure Rotate(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left: Rotate(1); break;
                case Direction.Right: Rotate(3); break;
            }
            return this;
        }

        private void Rotate(int multiplier)
        {
            _rotation = (_rotation+multiplier)%4;
            foreach (Cell cell in this._cells)
            {
                cell.Position = RotateVector(cell.Position, 90 * multiplier);
            }
        }
        
        private static Vector2 RotateVector(Vector2 v, float degrees) {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);
         
            float tx = v.x;
            float ty = v.y;
 
            return new Vector2(
                (float)Math.Round(cos * tx - sin * ty), 
                (float)Math.Round(sin * tx + cos * ty)
                );
        }
        
        public Figure Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down: _position+=Vector2.down; break;
                case Direction.Left: _position+=Vector2.left; break;
                case Direction.Right: _position+=Vector2.right; break;
            }
            return this;
        }

        public Figure Move(Vector2 position)
        {
            this._position = position;
            return this;
        }
        
        public List<Cell> GetCells()
        {
            List<Cell> res = CopyCells();
            foreach (Cell c in res)
            {
                c.Position += _position;
            }

            return res;
        }
        public Figure Clone()
        {
            Figure res = new Figure
            {
                _cells = CopyCells(), 
                _position = Vector2.zero + _position
            };
            return res;
        }
        private List<Cell> CopyCells()
        {
            List<Cell> res = new List<Cell>();
            foreach (Cell cell in _cells)
            {
                res.Add(new Cell(cell.Position));
            }
            return res;
        }

        public static Figure GenerateRandom()
        {
            switch (UnityEngine.Random.Range(0,7))
            {
                case 0: return GenerateIFigure();
                case 1: return GenerateTFigure();
                case 2: return GenerateUFigure();
                case 3: return GenerateNFigure();
                case 4: return GenerateИFigure();
                case 5: return GenerateГFigure();
                case 6: return GenerateLFigure();
                default: return GenerateOFigure();
            }
        }
        public static Figure GenerateTFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            res._cells.Add(new Cell(Vector2.left));
            res._cells.Add(new Cell(Vector2.right));
            res._cells.Add(new Cell(Vector2.up));
            return res;
        }
        public static Figure GenerateIFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            res._cells.Add(new Cell(Vector2.left));
            res._cells.Add(new Cell(Vector2.left+Vector2.left));
            res._cells.Add(new Cell(Vector2.right));
            return res;
        }
        public static Figure GenerateNFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            res._cells.Add(new Cell(Vector2.up));
            res._cells.Add(new Cell(Vector2.up+Vector2.left));
            res._cells.Add(new Cell(Vector2.right));
            return res;
        }
        public static Figure GenerateИFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            res._cells.Add(new Cell(Vector2.up));
            res._cells.Add(new Cell(Vector2.up+Vector2.right));
            res._cells.Add(new Cell(Vector2.left));
            return res;
        }
        public static Figure GenerateUFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            res._cells.Add(new Cell(Vector2.left));
            res._cells.Add(new Cell(Vector2.up));
            res._cells.Add(new Cell(Vector2.up+Vector2.left));
            return res;
        }
        public static Figure GenerateLFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            res._cells.Add(new Cell(Vector2.left));
            res._cells.Add(new Cell(Vector2.right));
            res._cells.Add(new Cell(Vector2.right+Vector2.up));
            return res;
        }
        /*
         * 0 1 1 1 1 1 1 1 0
         * 1 0 1 1 0 1 1 1 1
         * 1 0 0 1 0 0 1 1 1
         * 1 1 0 0 1 1 1 1 1
         * 0 1 1 1 1 1 0 0
         * 0 0 0 0 0 0 0 0
         * 0 1 1 1 1 0 0 0
         */

        public static Figure GenerateГFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            res._cells.Add(new Cell(Vector2.left));
            res._cells.Add(new Cell(Vector2.right));
            res._cells.Add(new Cell(Vector2.left+Vector2.up));
            return res;
        }
        public static Figure GenerateOFigure()
        {
            Figure res = new Figure();
            res._cells.Add(new Cell(Vector2.zero));
            return res;
        }

        public static bool AreSame(List<Figure> l1, List<Figure> l2)
        {
            if (l1.Count != l2.Count) return false;
            bool dpf = false;
            Vector2 dp = Vector2.zero;
            for (int i = 0; i < l1.Count; i++)
            {
                Figure f1 = l1[i];
                Figure f2 = l2[i];
                if (!dpf)
                {
                    dp = f1.Position - f2.Position;
                    dpf = true;
                }
                if (f1.Position != f2.Position + dp)
                    return false;
                if (f1.Rotation != f2.Rotation)
                    return false;
            }
            return true;
        }

        
    }

}

