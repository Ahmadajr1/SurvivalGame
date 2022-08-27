using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    //camera
    [SerializeField] private Transform cameraTarget;

    //mouse look/move
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private bool invertMouse = false;
    [SerializeField] private float lookUpConstraint = 60f;
    [SerializeField] private float lookDownConstraint = -60f;

    //movement
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float runSpeed = 15f;
    private Vector3 moveForward;
    private Vector3 moveRight;
    private float currentSpeed;
    

    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravityMod = 2.5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask movingPlatformLayer;

    //local GameObjects
    private CharacterController charController;
    private Transform mainCamera;
    private Vector3 platformVelocity = Vector3.zero;

    //local variables
    private float verticalRotationStore;
    private Vector3 movement;
    private bool isGrounded;
    void Start()
    {
        // To lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        charController = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;
    }

    void Update()
    {
        //mouse input
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        //rotate character based on x mouse movement
        Vector3 playerAngles = transform.rotation.eulerAngles;
        float yRotation = playerAngles.y + mouseInput.x;
        transform.rotation = Quaternion.Euler(playerAngles.x, yRotation, playerAngles.z);

        //mouse invert preference multiplier
        float invert = (!invertMouse) ? -1f : 1f;
        verticalRotationStore += (mouseInput.y * invert);

        //limit X rotation value based on input
        verticalRotationStore = Mathf.Clamp(verticalRotationStore, lookDownConstraint, lookUpConstraint);

        //apply rotation to camera target
        Vector3 cameraTargetAngles = cameraTarget.eulerAngles;
        cameraTarget.rotation = Quaternion.Euler(verticalRotationStore, cameraTargetAngles.y, cameraTargetAngles.z);

        //get user input for moving
        moveForward = transform.forward * Input.GetAxisRaw("Vertical");
        moveRight = transform.right * Input.GetAxisRaw("Horizontal");

        //set movement speed based on if the player is holding down left shift
        currentSpeed = (Input.GetKey(KeyCode.LeftShift)) ? runSpeed : moveSpeed;

        //do our own check for jumping
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.3f, groundLayer);

        //if player is pushing the jump button
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }
    }


    private void LateUpdate()
    {
        //change main camera position and rotation based on cameraTarget
        mainCamera.position = cameraTarget.position;
        mainCamera.rotation = cameraTarget.rotation;
    }

    private void FixedUpdate()
    {
        //To normalize forward and right movement without affecting Y movement
        float yVelocity = movement.y;
        movement = (moveForward + moveRight).normalized;
        movement *= currentSpeed;
        movement.y = yVelocity;

        //apply movement to the player
        charController.Move(movement * Time.fixedDeltaTime);

        //apply gravity to the player
        movement.y += Physics.gravity.y * Time.fixedDeltaTime * gravityMod;
        
        //check if Character Controller is grounded
        if (charController.isGrounded)
        {
            movement.y = 0f;
        }
    }
}
