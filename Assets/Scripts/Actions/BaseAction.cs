using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit unit;
    protected bool isActive = false;
    protected Action onActionComplete;

    protected virtual void Awake() 
    {
        if (!TryGetComponent<Unit>(out unit))
        {
            Debug.LogError("Cannot get Unit component");
        }
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition targetGridPosition, Action onActionComplete);

    public virtual bool IsValidActionPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointCost()
    {
        return 1;
    }

    public void ActionStart(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        isActive = true;
    }

    public void ActionComplete()
    {
        isActive = false;
        onActionComplete();
    }
}
