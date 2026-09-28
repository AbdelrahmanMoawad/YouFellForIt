using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = -19.62f; 

    [Header("Ground Detection")]
    [SerializeField] private float groundDistance = 0.2f;

    private Vector3 velocity;
    private bool isGrounded;
    private const float GroundStickForce = -2f; 

    private void Start()
    {
       
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGroundedState();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    private void CheckGroundedState()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = GroundStickForce;
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}