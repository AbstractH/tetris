﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = GameFieldBehaviour.Direction;
using Random = UnityEngine.Random;

public class Figure
    {
        public Vector2 Position => position;
        
        private List<Cell> cells;
        private Vector2 position;

        public Figure()
        {
            cells = new List<Cell>();
        }

        public Figure Rotate(Direction direction)
        {
            switch (direction)
            {
                case Direction.LEFT: Rotate(1); break;
                case Direction.RIGHT: Rotate(3); break;
            }
            return this;
        }

        private void Rotate(int multiplier)
        {
            foreach (Cell cell in this.cells)
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
                case Direction.DOWN: this.position+=Vector2.down; break;
                case Direction.LEFT: this.position+=Vector2.left; break;
                case Direction.RIGHT: this.position+=Vector2.right; break;
            }
            return this;
        }

        public Figure Move(Vector2 position)
        {
            this.position = position;
            return this;
        }
        
        public List<Cell> GetCells()
        {
            List<Cell> res = CopyCells();
            foreach (Cell c in res)
            {
                c.Position += this.position;
            }

            return res;
        }
        public Figure Clone()
        {
            Figure res = new Figure();
            res.cells = CopyCells();
            res.position = Vector2.zero+this.position;
            return res;
        }
        private List<Cell> CopyCells()
        {
            List<Cell> res = new List<Cell>();
            foreach (Cell cell in cells)
            {
                res.Add(new Cell(cell.Position));
            }
            return res;
        }

        public static Figure GenerateRandom()
        {
            switch (Random.Range(0,7))
            {
                case 0: return GenerateIFigure();
                case 1: return GenerateTFigure();
                case 2: return GenerateUFigure();
                case 3: return GenerateZFigure();
                case 4: return GenerateИFigure();
                case 5: return GenerateГFigure();
                case 6: return GenerateLFigure();
                default: return GenerateOFigure();
            }
        }
        public static Figure GenerateTFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            res.cells.Add(new Cell(Vector2.left));
            res.cells.Add(new Cell(Vector2.right));
            res.cells.Add(new Cell(Vector2.up));
            return res;
        }
        public static Figure GenerateIFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            res.cells.Add(new Cell(Vector2.left));
            res.cells.Add(new Cell(Vector2.left+Vector2.left));
            res.cells.Add(new Cell(Vector2.right));
            return res;
        }
        public static Figure GenerateZFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            res.cells.Add(new Cell(Vector2.up));
            res.cells.Add(new Cell(Vector2.up+Vector2.left));
            res.cells.Add(new Cell(Vector2.right));
            return res;
        }
        public static Figure GenerateИFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            res.cells.Add(new Cell(Vector2.up));
            res.cells.Add(new Cell(Vector2.up+Vector2.right));
            res.cells.Add(new Cell(Vector2.left));
            return res;
        }
        public static Figure GenerateUFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            res.cells.Add(new Cell(Vector2.left));
            res.cells.Add(new Cell(Vector2.up));
            res.cells.Add(new Cell(Vector2.up+Vector2.left));
            return res;
        }
        public static Figure GenerateLFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            res.cells.Add(new Cell(Vector2.left));
            res.cells.Add(new Cell(Vector2.right));
            res.cells.Add(new Cell(Vector2.right+Vector2.up));
            return res;
        }
        /*
         * 0 1 1 1 1 1 1 1 0
         * 1 0 1 1 0 1 1 1 1
         * 1 0 0 1 0 0 1 1 1
         * 1 1 0 0 1 1 1 1 1
         * 0 1 1 1 1 1 0 0
         * 0 0 0 0 0 0 0 0
         * 0 1 1 1 1 0 ` 0
         */

        public static Figure GenerateГFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            res.cells.Add(new Cell(Vector2.left));
            res.cells.Add(new Cell(Vector2.right));
            res.cells.Add(new Cell(Vector2.left+Vector2.up));
            return res;
        }
        public static Figure GenerateOFigure()
        {
            Figure res = new Figure();
            res.cells.Add(new Cell(Vector2.zero));
            return res;
        }
        
    }
