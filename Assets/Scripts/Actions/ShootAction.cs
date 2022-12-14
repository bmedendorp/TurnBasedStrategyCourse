using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    [SerializeField] private LayerMask obstaclesLayerMask;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs 
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private int maxShootDistance = 7;

    private enum State 
    {
        Aiming,
        Shooting,
        Cooldown
    };
    private State state;
    private float timer;
    private Unit targetUnit;
    private bool canShootBullet;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        switch (state)
        {
            case State.Aiming:
                Quaternion aimDirection = Quaternion.LookRotation(targetUnit.GetWorldPosition() - unit.GetWorldPosition());
                float rotationSpeed = 10.0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, aimDirection, Time.deltaTime * rotationSpeed);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooldown:
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
            case State.Aiming:
                float shootingStateTime = .1f;
                timer = shootingStateTime;
                state = State.Shooting;
                break;
            case State.Shooting:
                float cooldownStateTime = .5f;
                timer = cooldownStateTime;
                state = State.Cooldown;
                break;
            case State.Cooldown:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs {
            targetUnit = targetUnit, 
            shootingUnit = unit
        });
        
        OnShoot?.Invoke(this, new OnShootEventArgs {
            targetUnit = targetUnit, 
            shootingUnit = unit
        });
        
        int damageAmt = 40;
        targetUnit.Damage(damageAmt);
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
    
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + gridOffset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                int testDistance = Math.Abs(x) + Math.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    // Target is out of range
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

                Vector3 shooterPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                Vector3 targetPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Vector3 shootDirection = (targetPosition - shooterPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(shooterPosition + Vector3.up * unitShoulderHeight, 
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
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        canShootBullet = true;

        float aimingStateTime = 1f;
        timer = aimingStateTime;
        state = State.Aiming;

        ActionStart(onActionComplete);
    }

    public Unit GetTargetUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100)
        };
    }    
}
