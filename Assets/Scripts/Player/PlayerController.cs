using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump (no hold needed)")]
    public float jumpHeight = 3.5f;
    public float coyoteTime = 0.10f;
    public float jumpBufferTime = 0.12f;
    public float groundedStick = -2f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float gravityScale = 2.0f;

    [Header("Grounding Check")]
    public LayerMask groundMask = ~0;
    public float sphereGroundExtra = 0.25f;

    [Header("Air Control")]
    [Range(0f, 1f)] public float airControlFactor = 0.7f;// 0 = no air control, 1 = full control
    public float landingSmoothTime = 0.1f;// time to smooth horizontal velocity when landing

    [Header("Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 200f;

    [Header("Double Jump")]
    public int maxAirJumps = 1;  
    private int airJumpCount = 0;


    private CharacterController controller;
    private PlayerRideOnPlatforms rider;
    private Vector3 velocity;
    private Vector3 currentHorizontalVelocity;
    private float xRotation = 0f;

    private float lastGroundedTime = -999f;
    private float lastJumpPressedTime = -999f;
    private bool jumpConsumed;
    private bool wasGroundedLastFrame;

    void Awake()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        rider = GetComponent<PlayerRideOnPlatforms>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            lastJumpPressedTime = Time.time;

        bool grounded = controller.isGrounded;
        if (!grounded)
        {
            grounded = SphereGround(out _);
            if (!grounded && rider != null && rider.supportedThisFrame)
                grounded = true;
        }

        if (grounded)
        {
            lastGroundedTime = Time.time;
            jumpConsumed = false;
            airJumpCount = 0;
            if (velocity.y < 0f) velocity.y = groundedStick;
        }

        float effectiveGravity = gravity * gravityScale;
        bool coyoteOk = (Time.time - lastGroundedTime) <= coyoteTime;
        bool bufferOk = (Time.time - lastJumpPressedTime) <= jumpBufferTime;

        // Check if on upward-moving platform for jump boost
        bool onUpwardPlatform = rider != null && rider.supportedThisFrame && rider.current != null;
        bool platformMovingUp = false;
        if (onUpwardPlatform)
        {
            UpAndDownPlatform upDownPlatform = rider.current.GetComponent<UpAndDownPlatform>();
            if (upDownPlatform != null)
            {
                platformMovingUp = upDownPlatform.IsMovingUp;
            }
            else
            {
                // Fallback to checking delta
                Vector3 platformDelta = rider.current.WorldDeltaThisFrame;
                platformMovingUp = platformDelta.y > 0.001f;
            }
        }
        
        // Boost jump height when on upward-moving platform
        float currentJumpHeight = jumpHeight;
        if (onUpwardPlatform && platformMovingUp && grounded)
        {
            currentJumpHeight = jumpHeight * 2.2f; // 20% jump boost - slightly higher than normal
        }

        if (!jumpConsumed && coyoteOk && bufferOk)
        {
            velocity.y = Mathf.Sqrt(currentJumpHeight * -2f * effectiveGravity);
            jumpConsumed = true;
            lastJumpPressedTime = -999f;
        }else if (!grounded && bufferOk && airJumpCount < maxAirJumps)// double jump
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * effectiveGravity);
            airJumpCount++;
            lastJumpPressedTime = -999f;
        }

        // If on a platform that's moving upward, cancel gravity to prevent jitter
        // Reuse the platformMovingUp variable already calculated above
        bool onMovingPlatform = onUpwardPlatform; // Same check as above
        
        // Only cancel gravity if on upward platform, grounded, and NOT jumping
        bool shouldCancelGravity = onMovingPlatform && platformMovingUp && grounded && velocity.y <= 0f;
        
        if (shouldCancelGravity)
        {
            // Platform is moving up and player is passively riding it - cancel gravity to prevent jitter
            // Platform movement in LateUpdate will handle the vertical movement
            velocity.y = 0f;
        }
        else
        {
            // Normal gravity application (needed for jump arcs and falling)
            velocity.y += effectiveGravity * Time.deltaTime;
        }

        // 获取移动输入
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 moveInput = (transform.right * x + transform.forward * z).normalized;
        Vector3 targetHorizontal = moveInput * moveSpeed;

        // Smooth horizontal velocity
        if (grounded)
        {
            // switching from air to ground
            if (!wasGroundedLastFrame)
                currentHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, targetHorizontal, Time.deltaTime / landingSmoothTime);
            else
                currentHorizontalVelocity = targetHorizontal;
        }
        else
        {
            // air control
            currentHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, targetHorizontal * airControlFactor, 0.1f);
        }

        // Apply movement
        Vector3 totalVelocity = currentHorizontalVelocity + Vector3.up * velocity.y;
        controller.Move(totalVelocity * Time.deltaTime);

        wasGroundedLastFrame = grounded;
    }

    bool SphereGround(out RaycastHit hit)
    {
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
        if (Time.timeScale == 0f) return;
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
