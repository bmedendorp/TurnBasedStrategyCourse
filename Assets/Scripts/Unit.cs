using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int MAX_ACTION_POINTS = 2;

    public static event EventHandler OnAnyActionPointsChanged;

    private GridPosition gridPosition;

    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] baseActionArray;
    private int actionPoints = MAX_ACTION_POINTS;
    
    private void Awake() 
    {
        if (!TryGetComponent<MoveAction>(out moveAction))
        {
            Debug.LogError("Unit: Unable to find MoveAction component");
        }
        if (!TryGetComponent<SpinAction>(out spinAction))
        {
            Debug.LogError("Unit: Unable to find SpinAction component");
        }
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start() 
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
    }

    // Update is called once per frame
    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            // Unit changed grid positions
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

     public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public SpinAction GetSpinAction()
    {
        return spinAction;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TrySpendActionPointsOnAction(BaseAction action) 
    {
        if (CanSpendActionPointsOnAction(action))
        {
            SpendActionPoints(action.GetActionPointCost());
            return true;
        }
        else
        {
            return false;
        }
    } 

    public bool CanSpendActionPointsOnAction(BaseAction action) 
    {
        return actionPoints >= action.GetActionPointCost();
    }

    private void SpendActionPoints(int actionPointAmount)
    {
        actionPoints -= actionPointAmount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        actionPoints = MAX_ACTION_POINTS;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
}
