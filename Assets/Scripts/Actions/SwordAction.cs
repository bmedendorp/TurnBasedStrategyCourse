using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    private enum State
    {
        SlashBeforeHit,
        SlashAfterHit
    }

    public static event EventHandler OnAnySlashStart;
    public event EventHandler OnSlashStart;
    public event EventHandler OnSlashComplete;

    private int maxSwordDistance = 1;
    private State state;
    private float timer;
    private Unit targetUnit;

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition gridPosition = unit.GetGridPosition();
    
        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + gridOffset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitAtGridPosition(testGridPosition))
                {
                    // There is no unit at this position
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // Unit is the same type
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition targetGridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);

        float slashBeforeHitTime = .7f;
        timer = slashBeforeHitTime;
        state = State.SlashBeforeHit;

        ActionStart(onActionComplete);
        OnSlashStart?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        switch (state)
        {
            case State.SlashBeforeHit:
                Quaternion aimDirection = Quaternion.LookRotation(targetUnit.GetWorldPosition() - unit.GetWorldPosition());
                float rotationSpeed = 10.0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, aimDirection, Time.deltaTime * rotationSpeed);
                break;
            case State.SlashAfterHit:
                break;
        }

        timer -= Time.deltaTime;
        if ( timer < 0)
        {
            SwitchState();
        }
    }

    private void SwitchState()
    {
        switch (state)
        {
            case State.SlashBeforeHit:
                float slashAfterHitTime = .2f;
                timer = slashAfterHitTime;
                state = State.SlashAfterHit;
                SlashTarget();
                break;
            case State.SlashAfterHit:
                OnSlashComplete?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    private void SlashTarget()
    {
                int damageAmt = 100;
                targetUnit.Damage(damageAmt);

                OnAnySlashStart?.Invoke(this, EventArgs.Empty);
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }
}
