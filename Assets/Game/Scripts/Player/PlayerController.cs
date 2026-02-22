using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -9.8f;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] LayerMask groundLayer;


    // Camera Rotation
    private Transform cameraTransform;
    private float verticalRotation = 0f;

    // Ground Movement
    CharacterController characterController;
    private Vector3 velocity;
    float raycastDistance;

    // Utility
    private bool dead = false;
    private bool won = false;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        dead = false;
        won = false;
}

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float playerHeight = capsuleCollider.height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) + 0.2f;
    }

    void Update()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalMove + transform.right * horizontalMove;
        moveDirection.Normalize();

        if (!dead && !won)
        {
            RotateCamera();
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }

        /*if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !dead && !won)
        {
            velocity.y += jumpForce;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }*/

        if (!dead && !won)
        {
            if (IsGrounded() && horizontalMove == 0 && verticalMove == 0)
            {
                velocity = new Vector3(0, velocity.y, 0);
            }

            characterController.Move(velocity * Time.deltaTime);
        }
    }

    #region getters and setters
    public bool IsDead()
    {
        return dead;
    }

    public void Kill()
    {
        dead = true;
    }

    public void GameWon()
    {
        won = true;
    }

    public bool CheckWon()
    {
        return won;
    }

    #endregion

    #region Movement
    
    bool IsGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
    }

    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -40f, 60f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    #endregion
}
