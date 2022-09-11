using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSphere : MonoBehaviour, IInteractable
{
    [SerializeField] Transform sphere;
    [SerializeField] Material redMaterial;
    [SerializeField] Material greenMaterial;

    private Action onInteractComplete;
    private Renderer sphereRenderer;
    private bool isGreen = false;

    private void Awake()
    {
        sphereRenderer = sphere.GetComponent<Renderer>();
    }

    private void Start()
    {
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractable(gridPosition, this);
        LevelGrid.Instance.SetWalkable(gridPosition, false);

        if (isGreen)
        {
            SetGreen();
        }
        else
        {
            SetRed();
        }
    }

    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;

        if (isGreen)
        {
            SetRed();
        }
        else
        {
            SetGreen();
        }

        onInteractComplete();
    }

    private void SetGreen()
    {
        isGreen = true;
        sphereRenderer.material = greenMaterial;
    }

    private void SetRed()
    {
        isGreen = false;
        sphereRenderer.material = redMaterial;
    }
}
