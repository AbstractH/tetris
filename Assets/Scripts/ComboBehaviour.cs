using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboBehaviour : MonoBehaviour
{
    private int combo;
    public int Combo => combo;
    private CubicTextMesh mesh;

    private void Awake()
    {
        mesh = GetComponent<CubicTextMesh>();
    }

    public void AddCombo()
    {
        this.combo++;
        mesh.Text = combo.ToString();
        float s = 1f / combo.ToString().Length;
        this.transform.localScale = new Vector3(s,s,s);
        StopCoroutine("EraseCombo");
        StartCoroutine("EraseCombo");
    }
    
    IEnumerator EraseCombo()
    {
        float comboTime = 10.0f;
        yield return new WaitForSeconds(comboTime);
        Clear();
    }

    public void Clear()
    {
        this.combo = 1;
        mesh.Text = "";
    }
    
}
