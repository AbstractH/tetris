using System.Collections;
using UnityEngine;

namespace Tetris
{
    public class ComboBehaviour : MonoBehaviour
    {
        private int _combo;
        public int Combo => _combo;
        private CubicTextMesh _mesh;

        private void Awake()
        {
            _mesh = GetComponent<CubicTextMesh>();
        }

        public void AddCombo()
        {
            this._combo++;
            _mesh.Text = _combo.ToString();
            float s = 1f / _combo.ToString().Length;
            this.transform.localScale = new Vector3(s,s,s);
            StopCoroutine(nameof(EraseCombo));
            StartCoroutine(nameof(EraseCombo));
        }
    
        IEnumerator EraseCombo()
        {
            float comboTime = 10.0f;
            yield return new WaitForSeconds(comboTime);
            Clear();
        }

        public void Clear()
        {
            this._combo = 1;
            _mesh.Text = "";
        }
    
    }

}
