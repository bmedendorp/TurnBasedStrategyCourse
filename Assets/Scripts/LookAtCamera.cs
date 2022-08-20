using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;

    private Transform cameraTransform;

    private void Start() 
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate() 
    {
        if (invert)
        {
            Vector3 directionFromCamera = transform.position - cameraTransform.position;
            transform.LookAt(transform.position + directionFromCamera);
        }
        else
        {
            transform.LookAt(cameraTransform);
        }
    }
}
