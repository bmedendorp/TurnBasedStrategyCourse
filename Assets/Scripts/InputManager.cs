using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance {get; private set;}

    private void Update()
    {
        if (Instance != null)
        {
            Debug.Log("There is more than one InputManager!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Vector3 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }

    public bool IsMouseButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public Vector2 GetCameraMovementVector()
    {
        Vector2 inputMoveDir = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y = 1f;
        }        
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y = -1f;
        }        
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }        
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = 1f;
        }

        return inputMoveDir; 
    }

    public float GetCameraRotationAmount()
    {
        float rotationAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotationAmount = 1f;
        }        
        if (Input.GetKey(KeyCode.E))
        {
            rotationAmount = -1f;
        } 

        return rotationAmount;
    }

    public float GetCameraZoomAmount()
    {
        float zoomAmount = 0f;

        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = 1f;
        }

        return zoomAmount;
    }
}
