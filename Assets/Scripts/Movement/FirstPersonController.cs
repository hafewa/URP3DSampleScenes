using UnityEngine;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    private float m_MouseSensitivity = 100f;
    [SerializeField]
    private float m_MovementSpeed = 5f;
    [SerializeField]
    private Transform m_PlayerCamera = null;
    [SerializeField]
    private bool m_MoveWithMouse = true;

    [SerializeField] private float m_Gravity = -9.81f;

    private CharacterController m_CharacterController;
    private float m_XRotation = 0f;
    private Vector3 m_Velocity;

    void Start()
    {
#if ENABLE_INPUT_SYSTEM
        Debug.Log("The FirstPersonController uses the legacy input system. Please set it in Project Settings");
        m_MoveWithMouse = false;
#endif
        if (m_MoveWithMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Look();
        Move();
        m_Velocity.y += m_Gravity * Time.deltaTime;
    }

    private void Look()
    {
        Vector2 lookInput = GetLookInput();

        m_XRotation -= lookInput.y;
        m_XRotation = Mathf.Clamp(m_XRotation, -90f, 90f);

        m_PlayerCamera.localRotation = Quaternion.Euler(m_XRotation, 0, 0);
        transform.Rotate(Vector3.up * lookInput.x, Space.World);
    }

    private void Move()
    {
        Vector3 movementInput = GetMovementInput();

        Vector3 move = transform.right * movementInput.x + transform.forward * movementInput.z;

        m_CharacterController.Move(move * m_MovementSpeed * Time.deltaTime);
        m_CharacterController.Move(-Vector3.down * m_Gravity * Time.deltaTime);
    }

    private Vector2 GetLookInput()
    {
        float mouseX = 0;
        float mouseY = 0;
        if (m_MoveWithMouse)
        {
            mouseX = Input.GetAxis("Mouse X") * m_MouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * m_MouseSensitivity * Time.deltaTime;
        }
        return new Vector2(mouseX, mouseY);
    }

    private Vector3 GetMovementInput()
    {
        float x = 0;
        float z = 0;
        if (m_MoveWithMouse)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        return new Vector3(x, 0, z);
    }
}
