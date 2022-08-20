using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private const int MAX_ACTION_POINTS = 2;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitDied;

    [SerializeField] private bool isEnemy;
    private GridPosition gridPosition;

    private MoveAction moveAction;
    private SpinAction spinAction;
    private HealthSystem healthSystem;
    private BaseAction[] baseActionArray;
    private int actionPoints = MAX_ACTION_POINTS;
    
    private void Awake() 
    {
        if (!TryGetComponent<MoveAction>(out moveAction))
        {
            Debug.LogError("Unit: Unable to find MoveAction component - " + transform);
        }

        if (!TryGetComponent<SpinAction>(out spinAction))
        {
            Debug.LogError("Unit: Unable to find SpinAction component - " + transform);
        }

        if (!TryGetComponent<HealthSystem>(out healthSystem))
        {
            Debug.LogError("Unit: Unable to find HealthSystem component - " + transform);
        }
        
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start() 
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

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
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
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

    public Vector3 GetWorldPosition()
    {
        return transform.position;
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
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            actionPoints = MAX_ACTION_POINTS;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmt)
    {
        healthSystem.Damage(damageAmt);
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDied?.Invoke(this, EventArgs.Empty);
    }
}
