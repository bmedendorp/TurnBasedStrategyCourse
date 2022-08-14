using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
    [SerializeField] private Animator moveAnimator;
    [SerializeField] private int maxMoveDistance = 4;

    private Unit unit;
    
    private Vector3 targetPosition;

    private void Awake() 
    {
        if (!TryGetComponent<Unit>(out unit))
        {
            Debug.LogError("Cannot get Unit component in MoveAction");
        }
        targetPosition = transform.position;
    }

    private void Update()
    {
        float stoppingDistance = 0.1f;
        if (Vector3.Distance(targetPosition, transform.position) > stoppingDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float velocity = 4f;
            transform.position += direction * velocity * Time.deltaTime;

            float rotationSpeed = 10.0f;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * rotationSpeed);
            moveAnimator.SetBool("isWalking", true);
        }
        else
        {
            moveAnimator.SetBool("isWalking", false);
        }        
    }

    public bool IsValidMovePosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public void Move(GridPosition targetGridPosition)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition gridOffset = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + gridOffset;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    // Position is out of bounds
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    // Our unit is already at this position
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitAtGridPosition(testGridPosition))
                {
                    // There is another unit already at this position
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }
}
