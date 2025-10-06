using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;          //Move speed

    [Header("Jump (no hold needed)")]
    public float jumpHeight = 3.5f;       //Jump height (m)
    public float coyoteTime = 0.10f;      //Grace after leaving ground
    public float jumpBufferTime = 0.12f;  //Grace after tap before landing
    public float groundedStick = -2f;     

    [Header("Gravity")]
    public float gravity = -9.81f;        
    public float gravityScale = 2.0f;     

    [Header("Grounding Check")]
    public LayerMask groundMask = ~0;     //Layers considered ground/platform
    public float sphereGroundExtra = 0.25f; //How far below feet we check

    [Header("Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 250f;

    private CharacterController controller;
    private PlayerRideOnPlatforms rider;   
    private Vector3 velocity;            
    private float xRotation = 0f;

    //Jump helpers
    private float lastGroundedTime = -999f;
    private float lastJumpPressedTime = -999f;
    private bool  jumpConsumed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        rider = GetComponent<PlayerRideOnPlatforms>();

        //Preserve existing camera init
        float startX = cameraTransform.localEulerAngles.x;
        if (startX > 180f) startX -= 360f;
        xRotation = Mathf.Clamp(startX, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        //Capture jump tap
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            lastJumpPressedTime = Time.time;

        //Ground detection
        bool grounded = controller.isGrounded;
        if (!grounded)
        {
            grounded = SphereGround(out _); //Catch moving platform
            if (!grounded && rider != null && rider.supportedThisFrame)
                grounded = true;            //Rider says we're supported
        }

        //Ground Handling
        if (grounded)
        {
            lastGroundedTime = Time.time;
            jumpConsumed = false;
            if (velocity.y < 0f) velocity.y = groundedStick;
        }

        // 4) Buffered jump + coyote time (no hold required)
        float effectiveGravity = gravity * gravityScale; // negative
        bool coyoteOk = (Time.time - lastGroundedTime)  <= coyoteTime;
        bool bufferOk = (Time.time - lastJumpPressedTime) <= jumpBufferTime;
        if (!jumpConsumed && coyoteOk && bufferOk)
        {
            // v0 = sqrt(2 * g * h), with g as positive magnitude
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * effectiveGravity);
            jumpConsumed = true;
            lastJumpPressedTime = -999f; // clear buffer
        }

        //Gravity
        velocity.y += effectiveGravity * Time.deltaTime;

        //Horizontal movement
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 moveInput = (transform.right * x + transform.forward * z).normalized;
        Vector3 horiz = moveInput * moveSpeed;

        //Horizontal and Vertical Movement
        Vector3 totalVelocity = horiz + Vector3.up * velocity.y;
        controller.Move(totalVelocity * Time.deltaTime);
    }

    //Ground detection function
    bool SphereGround(out RaycastHit hit)
    {
        // Sphere cast right under the feet—far more stable on moving/uneven platforms
        Vector3 feet = transform.position + Vector3.up * 0.1f;
        float radius = Mathf.Max(controller.radius * 0.95f, 0.2f);
        return Physics.SphereCast(
            feet,
            radius,
            Vector3.down,
            out hit,
            sphereGroundExtra,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void ResetLook(float pitch = 0f)
    {
        xRotation = Mathf.Clamp(pitch, -90f, 90f);
        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
