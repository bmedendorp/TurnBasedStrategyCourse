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
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMovementVector();

        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector.normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        // Camera rotation
        Vector3 rotationVector = new Vector3(0, 0, 0);
        rotationVector.y = InputManager.Instance.GetCameraRotationAmount();

        float rotationSpeed = 100f;

        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime; 
    }

    private void HandleZoom()
    {
        // Camera zoom
        float zoomAmountScale = 1f;

        targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomAmountScale;
        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;

        cinemachineTransposer.m_FollowOffset.y = Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y, targetFollowOffset.y, Time.deltaTime * zoomSpeed);
    }
}
