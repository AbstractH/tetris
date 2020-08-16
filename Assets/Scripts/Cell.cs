using UnityEngine;

namespace Tetris
{
    public class Cell
    {
        private Vector2 _position ;
        private bool _isFilled;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set { _isFilled = value; }
        }

        public Cell(Vector2 position)
        {
            _position = position;
            _isFilled = false;
        }

        public void Fill()
        {
            _isFilled = true;
        }

        public void Clear()
        {
            _isFilled = false;
        }

        public bool IsOnSamePosition(Cell cell)
        {
            return this._position.Equals(cell._position);
        } 
    }

}
