using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float MoveSpeed = 5f;
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float upMultiplier = 2f;
    [SerializeField] float downMultiplier = 2.5f;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] LayerMask groundLayer;


    // Camera Rotation
    private Transform cameraTransform;
    private float verticalRotation = 0f;

    // Ground Movement
    private float moveHorizontal;
    private float moveForward;

    // Jumping
    private Rigidbody rb;
    private bool isGrounded = true;
    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;
    private float playerHeight;
    private float raycastDistance;

    // Utility
    private bool dead;

    private void Awake()
    {
        dead = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerHeight = capsuleCollider.height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) + 0.2f;
    }

    void Update()
    {
        if (!dead)
        {
            moveHorizontal = Input.GetAxisRaw("Horizontal");
            moveForward = Input.GetAxisRaw("Vertical");

            RotateCamera();

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }

            if (!isGrounded && groundCheckTimer <= 0f)
            {
                Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
                isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
            }
            else
            {
                groundCheckTimer -= Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (!dead)
        {
            MovePlayer();
        }
    }

    public bool IsDead()
    {
        return dead;
    }

    public void Kill()
    {
        dead = true;
    }

    #region Movement
    void MovePlayer()
    {

        Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        Vector3 targetVelocity = movement * MoveSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;

        if (isGrounded && moveHorizontal == 0 && moveForward == 0)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }

        // jump gravity multipliers
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * downMultiplier * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * upMultiplier * Time.fixedDeltaTime;
        }
    }

    void RotateCamera()
    {
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, horizontalRotation, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void Jump()
    {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    #endregion
}
