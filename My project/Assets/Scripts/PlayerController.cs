using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

//COntrolls how the player character functions.
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput; //Used to access the controls

    public float moveSpeed = 5.0f; //How fast the player moves
    public float rotateSpeed = 100.0f; //How fast the player rotates
    private Vector2 moveDirection; //Two vectors for controling the camera and player movement
    private Vector2 lookDirection;
    public InputActionReference move; //Three Input actions to allow access to parts of the control scheme. MUST be set in the inspector
    public InputActionReference look;
    public InputActionReference attack;
    public CharacterController characterController; //Allows the player to move using Unity prebuilt collision detection. MUST be set in the inspector

    public bool controlCamera = true; //A debug option that turns off the camera controls for testing purposes
    public Camera cam; //Allows access to the main camera so the player can use it.
    public Transform camTarget; //An external target used to help adjust camera aim.
    public float lookSpeed = 2f; // Four floats for adjusting how the camera moves.
    public float minPitch = -30f;
    public float maxPitch = 60f;
    private float currentPitch = 0f;

    //Combat variables
    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _HP;
    [SerializeField] private float clapRange = 10.0f; //The range at which enemies can hear the "clap" noise the player emits
    [SerializeField] private float knifeRange = 3.0f; //The range at which the enemy detects knife swings.
    [SerializeField] private LayerMask enemyLayer = 1 << 6; //Used to make raycasts that only detect guards.
    public float HP { get => _HP; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
        StartCoroutine(activateControls());

        Cursor.lockState = CursorLockMode.Locked;
        _HP = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        MoveRelativeToCamera();
    }

    //Controls player movement, including moving the camera and moving relative to the camera's positon
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
        if (controlCamera)
        {
            float yaw = lookDirection.x * lookSpeed;
            transform.Rotate(0f, yaw, 0f);

            currentPitch -= lookDirection.y * lookSpeed;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            camTarget.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);

            cam.transform.position = camTarget.position - camTarget.forward * 3f + Vector3.up * 1.5f; // Adjust distance/height
            cam.transform.LookAt(camTarget);
        }

    }

    //Very stupid hack to make unity input actiosn behave. Basically toggles them back on after one frame. Used only on startup.
    IEnumerator activateControls()
    {
        yield return null;
        playerInput.enabled = true;
    }

    //Executes when the "Clap" button is pressed and makes a noise
    private void OnClap(InputValue value)
    {
        Debug.Log("Clap!");
        NoiseHandler.InvokeNoise(NoiseHandler.NoiseID.Clap, this.transform, clapRange);
    }

    //Used when the Knife button is pressed and processes the player's side of a melee attack. Vurrently only backstabs.
    private void OnKnife(InputValue value)
    {
        Debug.Log("Knife!");

        if(Physics.Raycast(this.transform.position, this.transform.forward,out RaycastHit hit, knifeRange, enemyLayer))
        {
            Debug.Log("Hit " + hit.collider.name);
            
            var enemy = hit.collider.GetComponent<EnemyStateMachineController>();
            if (enemy != null)
            {
                if(Vector3.Dot(enemy.Trans.forward, (this.transform.position - enemy.Trans.position).normalized) < 0f)
                {
                    enemy.getBackstabbed();
                }
                else
                {
                    Debug.Log("Not a Backstab!");
                }
            }
        }
        // Draw ray in Scene view
        Debug.DrawRay(this.transform.position, this.transform.forward * knifeRange, Color.red, 100f);
    }

    //Function for taking damage
    public void Damage(float damage)
    {
        _HP -= damage;
    }
}
