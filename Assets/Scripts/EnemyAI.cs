using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State 
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private State state;
    private float timer;

    private void Awake() 
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                if ((timer -= Time.deltaTime) <= 0)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        // No more enemies can take an action
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) 
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            timer = 2f;
            state = State.TakingTurn;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        BaseAction[] baseActionArray = enemyUnit.GetBaseActionArray();
        BaseAction bestAction = null;
        EnemyAIAction bestEnemyAIAction = null;

        foreach (BaseAction baseAction in baseActionArray)
        {
            if (!enemyUnit.CanSpendActionPointsOnAction(baseAction))
            {
                // Unit can't afford this action
                continue;
            }

            EnemyAIAction enemyAIAction = baseAction.GetBestEnemyAIAction();
            if (bestEnemyAIAction == null)
            {
                bestAction = baseAction;
                bestEnemyAIAction = enemyAIAction;
            }
            else
            {
                if (enemyAIAction != null && enemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestAction = baseAction;
                    bestEnemyAIAction = enemyAIAction;
                }
            }
        }

        if (bestAction == null)
        {
            return false;
        }

        if (!enemyUnit.TrySpendActionPointsOnAction(bestAction))
        {
            return false;
        }

        bestAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
        return true;
    }
}
