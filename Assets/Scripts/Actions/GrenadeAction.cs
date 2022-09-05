using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] Transform grenadeProjectilePrefab;
    [SerializeField] LayerMask obstaclesLayerMask;

    private int maxThrowDistance = 7;

    public int GetMaxThrowDistance()
    {
        return maxThrowDistance;
    }
    
    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition gridPosition = unit.GetGridPosition();
    
        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + gridOffset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                int testDistance = Math.Abs(x) + Math.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    // Target is out of range
                    continue;
                }

                Vector3 shooterPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                Vector3 targetPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Vector3 shootDirection = (targetPosition - shooterPosition).normalized;
                if (Physics.Raycast(shooterPosition, 
                        shootDirection,
                        out RaycastHit hitInfo,
                        Mathf.Abs(Vector3.Distance(shooterPosition, targetPosition)), 
                        obstaclesLayerMask))
                {
                    // Line of sight is obstructed
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition targetGridPosition, Action onActionComplete)
    {
        Transform grenadeTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(targetGridPosition, OnGrenadeActionComplete);

        ActionStart(onActionComplete);
    }

    private void OnGrenadeActionComplete()
    {
        ActionComplete();
    }
}
