using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    private PlayerInput _inputActions;

    private void Awake()
    {
        _inputActions = new PlayerInput();

        // Đăng ký sự kiện: Khi Action "Pause" được thực hiện, gọi hàm ToggleCursor
        _inputActions.Player.Pause.performed += ToggleCursor;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    void Start()
    {
        // Ban đầu, khóa và ẩn chuột
        LockCursor();
    }

    // Hàm này sẽ được gọi TỰ ĐỘNG khi bạn nhấn phím Escape
    private void ToggleCursor(InputAction.CallbackContext context)
    {
        // Nếu chuột đang bị khóa, hãy mở khóa
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            UnlockCursor();
        }
        // Ngược lại, nếu chuột đang tự do, hãy khóa lại
        else
        {
            LockCursor();
        }
    }

    // Tách ra thành các hàm nhỏ để dễ quản lý
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Hàm Update() không còn cần thiết cho việc này nữa
    // void Update() { }
}
