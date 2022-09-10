using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen = false;

    private Action onInteractComplete;
    private Animator animator;
    private GridPosition gridPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractable(gridPosition, this);
        LevelGrid.Instance.SetWalkable(gridPosition, isOpen);

        animator.SetBool("IsOpen", isOpen);
    }


    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;

        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    public void AnimationComplete()
    {
            LevelGrid.Instance.SetWalkable(gridPosition, isOpen);
            onInteractComplete();
    }
}
