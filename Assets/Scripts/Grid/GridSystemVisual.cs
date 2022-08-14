using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance;

    [SerializeField] private Transform gridSystemVisualSinglePrefab;

    GridSystemVisualSingle[ , ] gridVisualSingleArray;

    private void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        gridVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridVisualSingle = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridVisualSingleArray[x, z] = gridVisualSingle.GetComponent<GridSystemVisualSingle>();
            }
        }        
    }

    private void Update() 
    {
        UpdateGridPositions();
    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridVisualSingleArray[gridPosition.x, gridPosition.z].Show();
        }
    }

    public void UpdateGridPositions()
    {
        Unit unit = UnitActionController.Instance.GetSelectedUnit();
        HideAllGridPosition();
        ShowGridPositionList(unit.GetMoveAction().GetValidActionGridPositionList());

    }
}
