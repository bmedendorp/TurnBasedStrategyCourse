using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlane : MonoBehaviour
{
    private void Start()
    {
        LevelGrid.Instance.OnGridResized += LevelGrid_OnGridResized;
    }

    private void LevelGrid_OnGridResized(object sender, EventArgs e)
    {
        LevelGrid.GridResizeArgs resizeArgs = e as LevelGrid.GridResizeArgs;

        transform.localScale = new Vector3(Mathf.Ceil(resizeArgs.width / 10f), 1f, Mathf.Ceil(resizeArgs.height / 10f));
        transform.position = resizeArgs.position;
    }
}
