using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    const float MIN_FOLLOW_Y_OFFSET = 2f;
    const float MAX_FOLLOW_Y_OFFSET = 12f;

    Vector2 targetFollowOffset;
    CinemachineTransposer cinemachineTransposer;

    private void Start() 
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        // Camera lateral movement
        Vector3 inputMoveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z = 1f;
        }        
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z = -1f;
        }        
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }        
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = 1f;
        }  

        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        transform.position += moveVector.normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        // Camera rotation
        Vector3 rotationVector = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = 1f;
        }        
        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y = -1f;
        } 

        float rotationSpeed = 100f;

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime; 
    }

    private void HandleZoom()
    {
        // Camera zoom
        float zoomAmount = 1f;

        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y,MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;

        cinemachineTransposer.m_FollowOffset.y = Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y, targetFollowOffset.y, Time.deltaTime * zoomSpeed);
    }
}
