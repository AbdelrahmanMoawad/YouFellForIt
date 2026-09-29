using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    // [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private Vector3 velocity;

    [Header("Air Movement")]
    [SerializeField][Range (1f,50f)] private float airAcceleration = 15f;
    private Vector3 currentHorizontalVelocity;
    
    [Header("Gravity Settings")]
    // [SerializeField] private float fallMultiplayer = 2.5f;
    [SerializeField] private float velocityCap = -50f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Ground Detection")]
    // [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private float groundCastDistance = 0.1f;
    [SerializeField] private float sphereCastRadius = 0.45f;
    [SerializeField] private bool isGrounded;
    

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
        //Calculate the exact bottom of the Character Controller
        Vector3 bottom = transform.position + controller.center - (Vector3.up * (controller.height / 2f));
    
        //Offset upward by the radius so the sphere starts securely inside the capsule
        Vector3 castOrigin = bottom + (Vector3.up * sphereCastRadius);
    
        //Cast downwards
        isGrounded = Physics.SphereCast(castOrigin, sphereCastRadius, Vector3.down, out RaycastHit hit, groundCastDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = GroundStickForce;
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        //Normalize the vector to prevent faster movement diagonally
        Vector3 moveDirection = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;
        Vector3 targetVelocity = moveDirection * moveSpeed;

        if (isGrounded)
        {
            // Instant start/stop on the ground
            currentHorizontalVelocity = targetVelocity;
        }
        else
        {
            // Gradual shift toward target velocity in the air
            currentHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, targetVelocity, airAcceleration * Time.deltaTime);
        }

        controller.Move(currentHorizontalVelocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Variable jump Height (Short Hop)
        if (Input.GetButtonUp("Jump") && velocity.y > 0)
        {
            //Immediately cut upward velocity in half when the button is released
            velocity.y *= 0.5f;
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;

        // Cap the dawnward speed to prevent clipping
        if(velocity.y < velocityCap)
        {
            velocity.y = velocityCap;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}