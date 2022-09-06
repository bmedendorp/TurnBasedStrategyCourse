using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MoveAction : BaseAction
{
    [SerializeField] private int maxMoveDistance = 4;

    public event EventHandler OnMoveStart;
    public event EventHandler OnMoveStop;
    
    private List<Vector3> positionList;
    private int currentPositionIndex;
    private Seeker seeker;
    private List<GridPosition> validGridPositionList;
    private MultiTargetPath pathDistanceCheck;
    private CustomBlockManager.TraversalProvider traversalProvider;


    protected override void Awake() 
    {
        base.Awake();

        if (!TryGetComponent<Seeker>(out seeker)) 
        {
            Debug.LogError("Cannot get Seeker component");
        }
    }

    protected void Start() 
    {
        traversalProvider = new CustomBlockManager.TraversalProvider(CustomBlockManager.Instance, CustomBlockManager.BlockMode.AllExceptSelector, null);

        BuildValidGridPositionList(LevelGrid.Instance.GetGridPosition(transform.position));
    }

    public override string GetActionName()
    {
        return "Move";
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Quaternion targetDirection = Quaternion.LookRotation(targetPosition - transform.position);
        float rotationSpeed = 10.0f;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetDirection, Time.deltaTime * rotationSpeed);

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float stoppingDistance = 0.1f;
        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
        {
            float velocity = 4f;
            transform.position += moveDirection * velocity * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnMoveStop?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
            else
            {
                BuildValidGridPositionList(LevelGrid.Instance.GetGridPosition(positionList[currentPositionIndex]));
            }
        }  
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Path path = ABPath.Construct(transform.position, LevelGrid.Instance.GetWorldPosition(gridPosition), null);
        path.traversalProvider = traversalProvider;

        seeker.StartPath(path, OnPathComplete);

        OnMoveStart?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
        isActive = false;   // Don't want to start updating until we have our completed path
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        // Wait for valid grid positions to finish calculating if needed
        if (pathDistanceCheck.PipelineState != PathState.Returned)
        {
            pathDistanceCheck.BlockUntilCalculated();
        }

        List<GridPosition> validGridPositionListCopy = new List<GridPosition>(this.validGridPositionList);

        foreach (GridPosition gridPosition in validGridPositionList)
        {
            if (LevelGrid.Instance.HasAnyUnitAtGridPosition(gridPosition))
            {
                // There is another unit already at this position
                validGridPositionListCopy.Remove(gridPosition);
            }
            
            GraphNode graphNode = AstarPath.active.GetNearest(LevelGrid.Instance.GetWorldPosition(gridPosition)).node;
            if (CustomBlockManager.Instance.NodeContainsAnyBlocker(graphNode))
            {
                // There is a destructable obstacle at this position
                validGridPositionListCopy.Remove(gridPosition);
            }
        }

        return validGridPositionListCopy;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        EnemyAIAction enemyAIAction = new EnemyAIAction();
        ShootAction shootAction = unit.GetAction<ShootAction>();
        int enemiesInShootingRange = shootAction.GetValidActionGridPositionList(gridPosition).Count;

        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = enemiesInShootingRange * 10
        };
    }

    public void OnPathComplete(Path path) 
    {
        if (path.error)
        {
            Debug.Log("Error creating path!");
            return;
        }

        positionList = path.vectorPath;
        currentPositionIndex = 0;
        isActive = true;    // Start updating
        BuildValidGridPositionList(LevelGrid.Instance.GetGridPosition(positionList[0]));
    }

    public void OnMultiTargetPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.Log("Error creating path for path lengh checks!");
        }

        validGridPositionList = new List<GridPosition>();
        List<GraphNode>[] paths = pathDistanceCheck.nodePaths;

        const int MOVE_DISTANCE_MULTIPLIER = 10;
        foreach (List<GraphNode> path in paths)
        {
            // Validate Distance and add to validGridPositionList if it is short enough
            if (getPathDistance(path) <= maxMoveDistance * MOVE_DISTANCE_MULTIPLIER)
            {
                validGridPositionList.Add(LevelGrid.Instance.GetGridPosition((Vector3)path[path.Count - 1].position));
            }
        }
    }

    private void BuildValidGridPositionList(GridPosition unitGridPosition)
    {
        List<Vector3> validGridWorldPositionList = new List<Vector3>();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + gridOffset;
                NNInfo testNodeInfo = AstarPath.active.GetNearest(LevelGrid.Instance.GetWorldPosition(testGridPosition));
                NNInfo unitNodeInfo = AstarPath.active.GetNearest(LevelGrid.Instance.GetWorldPosition(unitGridPosition));

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                // if (unitGridPosition == testGridPosition)
                // {
                //     // Our unit is already at this position
                //     continue;
                // }

                // if (!testNodeInfo.node.Walkable) 
                // {
                //     // Position is obstructed by an obstacle
                //     continue;
                // }

                if (!PathUtilities.IsPathPossible(unitNodeInfo.node, testNodeInfo.node))
                {
                    // No valid path to position
                    continue;
                }

                validGridWorldPositionList.Add(LevelGrid.Instance.GetWorldPosition(testGridPosition));
            }
        }

        // Find path to all valid grid positions to check for path length
        pathDistanceCheck = seeker.StartMultiTargetPath(LevelGrid.Instance.GetWorldPosition(unitGridPosition),
                                                        validGridWorldPositionList.ToArray(),
                                                        true,
                                                        OnMultiTargetPathComplete);
    }

    public int getPathDistance(List<GraphNode> path) 
    {
        const int STRAIGHT_MOVE_COST = 10;
        const int DIAGONAL_MOVE_COST = 14;

        int length = 0;
        for (int x = 0; x < path.Count - 1; x++)
        {
            GraphNode node = path[x];
            GraphNode nextNode = path[x + 1];

            if (node.position.x == nextNode.position.x || node.position.z == nextNode.position.z)
            {
                length += STRAIGHT_MOVE_COST;
            }
            else
            {
                length += DIAGONAL_MOVE_COST;
            }
        }
        return length;
    }
}
