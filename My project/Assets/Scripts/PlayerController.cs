using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    public float moveSpeed;
    public float rotateSpeed;
    private Vector2 moveDirection;
    private Vector2 lookDirection;
    public InputActionReference move;
    public InputActionReference look;
    public InputActionReference attack;
    public CharacterController characterController;

    public Camera cam;
    public Transform camTarget;
    public float lookSpeed = 2f; // camera pitch speed
    public float minPitch = -30f;
    public float maxPitch = 60f;
    private float currentPitch = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
        StartCoroutine(activateControls());

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MoveRelativeToCamera();
    }

    void MoveRelativeToCamera()
    {
        moveDirection = move.action.ReadValue<Vector2>();
        lookDirection = look.action.ReadValue<Vector2>();

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 relativeForward = cameraForward * moveDirection.y;
        Vector3 relativeRight = cameraRight * moveDirection.x;

        Vector3 relativeMove = relativeForward + relativeRight;

        //animator.SetFloat(animID_running, relativeMove.magnitude);

        characterController.Move(relativeMove * Time.deltaTime * moveSpeed);

        //if (moveDirection != Vector2.zero)
        //{
        //    Quaternion rotateTo = Quaternion.LookRotation(relativeMove, Vector3.up);
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotateSpeed * Time.deltaTime);
        //}

        float yaw = lookDirection.x * lookSpeed;
        transform.Rotate(0f, yaw, 0f);

        currentPitch -= lookDirection.y * lookSpeed;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        camTarget.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);

        cam.transform.position = camTarget.position - camTarget.forward * 3f + Vector3.up * 1.5f; // Adjust distance/height
        cam.transform.LookAt(camTarget);

    }

    IEnumerator activateControls()
    {
        yield return null;
        playerInput.enabled = true;
    }
}
