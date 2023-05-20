using Unity.VisualScripting;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    private CharacterController characterController;
    private Camera playerCamera;

    private Vector3 move = Vector3.zero;

    private float mouseHorizontal = 3.0f;
    private float mouseVertical = 2.0f;
    private float minRotation = -65.0f;
    private float maxRotation = 60.0f;
    private float h_mouse, v_mouse;

    private void Awake()
    {
        characterController = U.GetOrAddComponent<CharacterController>(gameObject);

        Transform childTransform = transform.Find("PlayerCamera");
        if (childTransform != null)
        {
            playerCamera = U.GetOrAddComponent<Camera>(childTransform.gameObject);
        }
        else
        {
            Debug.LogError("Child transform with the name 'PlayerCamera' not found or does not have a Camera component attached.");
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Handle movement input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement vector
        Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput);

        // Normalize the movement vector to ensure consistent speed regardless of input magnitude
        movement.Normalize();

        // Apply movement speed based on input and player state
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        movement *= speed;

        // Handle jump input
        if (characterController.isGrounded && Input.GetButtonDown("Jump"))
        {
            movement.y = jumpSpeed;
        }

        // Apply gravity
        movement.y -= gravity * Time.deltaTime;

        // Move the character
        characterController.Move(transform.TransformDirection(movement) * Time.deltaTime);

        // Invoke the OnMove event
        if (movement != Vector3.zero)
        {
            OnMove?.Invoke(movement);
        }

        // Handle camera look input
        float mouseX = Input.GetAxis("Mouse X") * mouseHorizontal;
        float mouseY = Input.GetAxis("Mouse Y") * mouseVertical;

        // Rotate the player's camera vertically
        v_mouse -= mouseY;
        v_mouse = Mathf.Clamp(v_mouse, minRotation, maxRotation);
        playerCamera.transform.localEulerAngles = new Vector3(v_mouse, 0, 0);

        // Rotate the player's character horizontally
        transform.Rotate(0, mouseX, 0);

        // Invoke the OnLook event
        OnLook?.Invoke(new Vector2(mouseX, mouseY));
    }
    public delegate void MoveDelegate(Vector3 movement);
    public event MoveDelegate OnMove;

    public delegate void LookDelegate(Vector2 lookInput);
    public event LookDelegate OnLook;
}
