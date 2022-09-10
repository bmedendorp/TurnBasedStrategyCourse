using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int maxInteractDistance = 1;

    public override string GetActionName()
    {
        return "Interact";
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
    
        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + gridOffset;

                if (testGridPosition == gridPosition)
                {
                    // Can't be standing ON the interactable
                    continue;
                }

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                if (!LevelGrid.Instance.IsInteractable(testGridPosition))
                {
                    // Position is not interactable
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition targetGridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        IInteractable interactable = LevelGrid.Instance.GetInteractable(targetGridPosition);
        interactable.Interact(OnActionComplete);
    }

    private void OnActionComplete()
    {
        ActionComplete();
    }
}
