using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;

    private GridObject gridObject;
    private TextMeshPro text;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    private void Update() 
    {
        textMeshPro.text = gridObject.ToString();
    }
}
