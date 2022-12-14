using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;
    private List<Transform> debugObjectList;
    private Vector3 position;

    public GridSystem(int width, int height, float cellSize, Vector3 position, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.position = position;
        gridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridObjectArray[x, z] = createGridObject(this, new GridPosition(x, z));
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize + position;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        worldPosition -= position;
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        debugObjectList = new List<Transform>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform transform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                debugObjectList.Add(transform);
                if (transform.TryGetComponent<GridDebugObject>(out GridDebugObject gridDebugObject))
                {
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                }
                else
                {
                    Debug.LogError("GridDebugObject component not found on Grid Debug Object Prefab");
                }
            }
        }
    }

    public void DestroyDebugObjects()
    {
        if (debugObjectList != null)
        {
            foreach (Transform transform in debugObjectList)
            {
                GameObject.Destroy(transform.gameObject);
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && 
               gridPosition.z >= 0 && 
               gridPosition.x < width && 
               gridPosition.z < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
