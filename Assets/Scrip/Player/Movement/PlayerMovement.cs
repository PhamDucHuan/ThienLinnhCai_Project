using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IFixedUpdateListener
{
    [Header("Références")]
    [SerializeField] private Transform cam; // Transform của camera chính

    [Header("Paramètres de Mouvement")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 6.0f;
    // Biến turnSmoothTime không còn cần thiết nữa
    [SerializeField] private float gravity = -19.62f;

    // BIẾN MỚI CHO CHỨC NĂNG NHẢY
    [Header("Paramètres de Saut")]
    [Tooltip("Độ cao của cú nhảy.")]
    [SerializeField] private float jumpHeight = 1.5f;

    // Biến nội bộ
    private CharacterController _controller;
    private PlayerInput _inputActions; // Class được tạo tự động từ Input Actions asset
    private Vector2 _moveInput;
    private Vector3 _velocity; // Vector vận tốc, chủ yếu để xử lý trọng lực
    private bool _isSprinting = false;
    // Biến _turnSmoothVelocity không còn cần thiết nữa

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputActions = new PlayerInput(); // Khởi tạo Input Actions

        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        //Khich hoat chay nhanh
        _inputActions.Player.Sprint.performed += ctx => _isSprinting = true;
        _inputActions.Player.Sprint.canceled += ctx => _isSprinting = false;

        // === ĐĂNG KÝ SỰ KIỆN CHO ACTION "JUMP" ===
        _inputActions.Player.Jump.performed += OnJump;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.RegisterFixedUpdateListener(this);
        }
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.UnregisterFixedUpdateListener(this);
        }
    }

    public void OnFixedUpdate(float deltaTime)
    {
        SyncRotationWithCamera();

        HandleMovement();
        ApplyGravity();
    }
    //private void FixedUpdate()
    //{
        
    //}

    private void SyncRotationWithCamera()
    {
        // Lấy góc quay hiện tại của camera theo trục Y (quay ngang)
        float cameraAngleY = cam.transform.eulerAngles.y;

        // Áp dụng góc quay đó cho nhân vật (PlayerBody)
        // Chúng ta chỉ muốn xoay ngang, nên trục X và Z giữ nguyên là 0
        transform.rotation = Quaternion.Euler(0f, cameraAngleY, 0f);
    }

    private void HandleMovement()
    {
        // Lấy hướng input ngang và dọc (x, y)
        Vector3 direction = new Vector3(_moveInput.x, 0f, _moveInput.y);

        // Chỉ di chuyển khi có input
        if (direction.magnitude >= 0.1f)
        {
            // === TÍNH HƯỚNG DI CHUYỂN MỚI, KHÔNG CẦN XOAY ===
            // Kết hợp hướng phải và hướng trước của camera để tạo ra vector di chuyển cuối cùng.
            // Điều này cho phép nhân vật di chuyển tương đối với hướng camera mà không cần xoay người.
            Vector3 moveDir = cam.right * direction.x + cam.forward * direction.z;
            moveDir.y = 0; // Đảm bảo nhân vật không bay lên khi camera nhìn lên/xuống

            // DI CHUYỂN NHÂN VẬT
            float currentSpeed = _isSprinting ? runSpeed : walkSpeed;
            _controller.Move(moveDir.normalized * (currentSpeed * Time.deltaTime));
        }
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Chỉ cho phép nhảy khi nhân vật đang đứng trên mặt đất
        if (_controller.isGrounded)
        {
            // Công thức vật lý để tính vận tốc cần thiết để nhảy tới độ cao mong muốn
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
