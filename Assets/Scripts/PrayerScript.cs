
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 1.9f;
    public float sprintSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;
    public CharacterController controller;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;
    private float currentSpeed;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Handle movement
        HandleMovementInput();
        HandleJump();
        ApplyGravity();

        // Apply final movement
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMovementInput()
    {
        // Get input values
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Calculate movement direction
        Vector3 moveDirection = CalculateMovementDirection(horizontal, vertical);

        // Determine speed
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Rotate character to face movement direction
        if (moveDirection != Vector3.zero)
        {
            RotateCharacter(moveDirection);
        }

        // Move the character
        if (moveDirection != Vector3.zero || isSprinting)
        {
            Vector3 moveVector = moveDirection * currentSpeed;
            velocity.x = moveVector.x;
            velocity.z = moveVector.z;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        // Update animations
        UpdateAnimations(moveDirection);
    }

    private Vector3 CalculateMovementDirection(float horizontal, float vertical)
    {
        Vector3 direction = Vector3.zero;

        // If there's WASD input, use that direction
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            direction = (cameraTransform.right * horizontal + cameraTransform.forward * vertical).normalized;
            direction.y = 0;
        }
        // If no WASD but sprinting, use camera forward direction
        else if (isSprinting)
        {
            direction = cameraTransform.forward;
            direction.y = 0;
            direction.Normalize();
        }

        return direction;
    }

    private void RotateCharacter(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void UpdateAnimations(Vector3 moveDirection)
    {
        bool isMoving = moveDirection != Vector3.zero;

        animator.SetBool("walk", isMoving && !isSprinting);
        animator.SetBool("running", isSprinting);
        animator.SetBool("idle", !isMoving && !isSprinting);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetTrigger("jump");
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

}