using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarGridSizer : MonoBehaviour
{
    private void Start()
    {
        LevelGrid.Instance.OnGridResized += LevelGrid_OnGridResized;
    }

   private void LevelGrid_OnGridResized(object sender,  EventArgs args)
    {
        LevelGrid.GridResizeArgs resizeArgs = args as LevelGrid.GridResizeArgs;

        GridGraph gridGraph = AstarPath.active.data.gridGraph;
        int width = resizeArgs.width / resizeArgs.cellSize;
        int depth = resizeArgs.height / resizeArgs.cellSize;
        gridGraph.SetDimensions(width, depth, resizeArgs.cellSize);
        gridGraph.center = resizeArgs.position;


       AstarPath.active.Scan();
    }
}
