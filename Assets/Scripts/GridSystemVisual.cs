using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        Move,
        Spin,
        Shoot,
        ShootObstructed,
        Grenade,
        GrenadeObstructed
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;

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
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnUnitMoved += LevelGrid_OnUnitMoved;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;

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

        UpdateGridPositions();       
    }

    public void HideAllGridPositions()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void ShowGridPositionRange(GridPosition position, int range, GridVisualType gridVisualType)
    {
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = position + gridOffset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                int testDistance = Math.Abs(x) + Math.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridVisualSingleArray[testGridPosition.x, testGridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
            }
        }
    }

    public void ShowGridPositionRangeSquare(GridPosition position, int range, GridVisualType gridVisualType)
    {
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = position + gridOffset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                gridVisualSingleArray[testGridPosition.x, testGridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
            }
        }
    }

    public void UpdateGridPositions()
    {
        BaseAction baseAction = UnitActionSystem.Instance.GetSelectedAction();
        HideAllGridPositions();

        GridVisualType gridVisualType;
        switch (baseAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.Move;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Spin;
                break;
            case ShootAction shootAction:
                ShowGridPositionRange(shootAction.GetUnit().GetGridPosition(), shootAction.GetMaxShootDistance(), GridVisualType.ShootObstructed);
                gridVisualType = GridVisualType.Shoot;
                break;
            case GrenadeAction grenadeAction:
                ShowGridPositionRange(grenadeAction.GetUnit().GetGridPosition(), grenadeAction.GetMaxThrowDistance(), GridVisualType.GrenadeObstructed);
                gridVisualType = GridVisualType.Grenade;
                break;
            case SwordAction swordAction:
                ShowGridPositionRangeSquare(swordAction.GetUnit().GetGridPosition(), swordAction.GetMaxSwordDistance(), GridVisualType.ShootObstructed);
                gridVisualType = GridVisualType.Shoot;
                break;
        }

        ShowGridPositionList(baseAction.GetValidActionGridPositionList(), gridVisualType);

    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) 
    {
        UpdateGridPositions();
    }

    private void LevelGrid_OnUnitMoved(object sender, EventArgs e) 
    {
        UpdateGridPositions();
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e) 
    {
        UpdateGridPositions();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }
}
