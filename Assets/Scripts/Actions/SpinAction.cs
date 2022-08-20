using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;

    public override string GetActionName()
    {
        return "Spin";
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAmount = 360 * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);
        totalSpinAmount += spinAmount;
        if (totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition targetGridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0f;

        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition gridPosition = unit.GetGridPosition();
        return new List<GridPosition>
        {
            gridPosition
        };
    }

    public override int GetActionPointCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }
}
