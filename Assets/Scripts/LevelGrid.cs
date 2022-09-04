using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    public event EventHandler OnUnitMoved;
    public event EventHandler OnGridResized;

    public class GridResizeArgs : EventArgs
    {
        public int minX { get; private set; }
        public int maxX { get; private set; }
        public int minZ { get; private set; }
        public int maxZ { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public int cellSize { get; private set; }
        public Vector3 position { get; private set; }

        public GridResizeArgs(int minX, int maxX, int minZ, int maxZ, int cellSize)
        {
            Set(minX, maxX, minZ, maxZ, cellSize);
        }

        public void Set(int minX, int maxX, int minZ, int maxZ, int cellSize)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minZ = minZ;
            this.maxZ = maxZ;
            this.cellSize = cellSize;

            CalculateParameters();
        }

        private void CalculateParameters()
        {
            width = maxX - minX;
            height = maxZ - minZ;

            position = new Vector3(width / 2f, 0f, height / 2f) + new Vector3(minX, 0f, minZ);
        }

        public override string ToString()
        {
            string outString = "(" + minX + ", " + maxX + ", " + minZ + ", " + maxZ + ")";
            outString += " : " + cellSize;
            outString += " -- " + position;
            return outString;
        }
    }

    [SerializeField] int cellSize;
    [SerializeField] private Transform gridDebugObjectPrefab;

    private int minX = int.MaxValue;
    private int maxX = int.MinValue;
    private int minZ = int.MaxValue;
    private int maxZ = int.MinValue;

    private GridSystem<GridObject> gridSystem;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LevelGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition previousGridPosition, GridPosition newGridPosition)
    {
        RemoveUnitAtGridPosition(previousGridPosition, unit);
        AddUnitAtGridPosition(newGridPosition, unit);

        OnUnitMoved?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();
    
    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public void AddRoom(int minX, int maxX, int minZ, int maxZ)
    {
        bool updated = false;
        
        if (this.minX > minX)
        {
            this.minX = minX;
            updated = true;
        }
        if (this.maxX < maxX)
        {
            this.maxX = maxX;
            updated = true;
        }
        if (this.minZ > minZ)
        {
            this.minZ = minZ;
            updated = true;
        }
        if (this.maxZ < maxZ)
        {
            this.maxZ = maxZ;
            updated = true;
        }

        if (updated)
        {
            if (gridSystem != null)
            {
                gridSystem.DestroyDebugObjects();
            }

            CreateGridSystem();
            OnGridResized?.Invoke(this, new GridResizeArgs(this.minX, this.maxX, this.minZ, this.maxZ, this.cellSize));
        }
    }

    private void CreateGridSystem()
    {
        int width = (maxX - minX) / cellSize;
        int height = (maxZ - minZ) / cellSize;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize, new Vector3(minX + 1f, 0f, minZ + 1f),
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
        // gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
}
