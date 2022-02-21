using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Transform debugHitPointTransform;  // puts a box at where the hookshot is activated
    [SerializeField] private Transform hookshotTransform;

    // declaring variables
    private CharacterController characterController;
    private float cameraVerticalAngle;
    private float characterVelocityY;
    private Vector3 characterVelocityMomentum;
    private Camera playerCamera;
    private State state;
    private Vector3 hookshotPosition;
    private float hookshotSize;

    private enum State
    {
        Normal,
        HookshotThrown,
        HookshotFlyingPlayer,
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked; // locks mouse in centre
        state = State.Normal;
        hookshotTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        // state engine to change the state of the character
        switch (state)
        {
            default:
            case State.Normal:
                HandleCharacterLook();
                HandleCharacterMovement();
                HandleHookshotStart();
                break;
            case State.HookshotThrown:
                HandleHookshotThrow();
                HandleCharacterLook();
                HandleCharacterMovement();
                break;
            case State.HookshotFlyingPlayer:
                HandleCharacterLook();
                HandleHookshotMovement();
                break;
        }
    }

    private void HandleCharacterLook()
    {
        // takes mouse input
        float lookX = Input.GetAxisRaw("Mouse X");
        float lookY = Input.GetAxisRaw("Mouse Y");

        // rotates camera based on mouse movement
        transform.Rotate(new Vector3(0f, lookX * mouseSensitivity, 0f), Space.Self);

        cameraVerticalAngle -= lookY * mouseSensitivity;

        // character camera vertical angle limiter
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

        // apply vertical angle as local rotation to camera so it pivots up and down
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }

    private void HandleCharacterMovement()
    {
        // takes keyboard input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        float moveSpeed = 20f;

        // velocity vector
        Vector3 characterVelocity = transform.right * moveX * moveSpeed + transform.forward * moveZ * moveSpeed;

        // resets character velocity Y when touching ground     
        if (characterController.isGrounded)
        {
            characterVelocityY = 0f;
            // jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float jumpSpeed = 30f;
                characterVelocityY = jumpSpeed;
            }
        }

        // apply gravity to jump
        float gravityDownForce = -60f;
        characterVelocityY += gravityDownForce * Time.deltaTime;

        // apply Y velocity to move vector
        characterVelocity.y = characterVelocityY;

        // apply momentum to character
        characterVelocity += characterVelocityMomentum;

        // move character controller 
        characterController.Move(characterVelocity * Time.deltaTime);

        // dampen momentum
        if (characterVelocityMomentum.magnitude >= 0f)
        { // checking if magnitude of momentum is > 0
            float momentumDrag = 3f; // define drag value
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime; // reducing our momentum by drag * time
            if (characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero; // when too small, set momentum to 0
            }
        }
    }

    private void ResetGravityEffect()
    {
        characterVelocityY = 0f;
    }

    private void HandleHookshotStart()
    {
        if (TestInputDownHookshot())
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit))
            {
                // hit something
                debugHitPointTransform.position = raycastHit.point;
                hookshotPosition = raycastHit.point;
                hookshotSize = 0f;
                hookshotTransform.gameObject.SetActive(true);
                hookshotTransform.localScale = Vector3.zero;
                state = State.HookshotThrown; // modifies state
            }
        }
    }

    private void HandleHookshotThrow()
    {
        hookshotTransform.LookAt(hookshotPosition); // our hookshot block will look towards our target position

        float hookshotThrowSpeed = 70f;
        hookshotSize += hookshotThrowSpeed * Time.deltaTime; // size will constantly increase
        hookshotTransform.localScale = new Vector3(1, 1, hookshotSize);
        if (hookshotSize >= Vector3.Distance(transform.position, hookshotPosition)) // check if hookshot has reached target position
        {
            state = State.HookshotFlyingPlayer; // change states
        }
    }

    private void HandleHookshotMovement()
    {
        hookshotTransform.LookAt(hookshotPosition);

        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized; // calculated direction towards hookshot position

        // locks velocity of hook in range 10 to 40
        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 40f;
        float hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 2f;

        characterController.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime); // moves character at a speed

        float reachHookshotPositionDistance = 1f;
        if (Vector3.Distance(transform.position, hookshotPosition) < reachHookshotPositionDistance)
        {
            // reached hookshot position
            StopHookshot();
        }

        if (TestInputDownHookshot())
        {
            // cancel hookshot
            StopHookshot();
        }

        if (TestInputJump())
        {
            // cancel grapple with jump
            float momentumExtraSpeed = 7f;
            characterVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
            float jumpSpeed = 40f;
            characterVelocityMomentum += Vector3.up * jumpSpeed;
            StopHookshot();
        }

    }

    // moving input key into seperate function
    // if player wants to change input, just change this function
    private bool TestInputDownHookshot()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    // function to cancel hookshot movement
    private void StopHookshot()
    {
        state = State.Normal;
        ResetGravityEffect();
        hookshotTransform.gameObject.SetActive(false);
    }

    private bool TestInputJump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
