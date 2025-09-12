using UnityEngine;
using UnityEngine.InputSystem;

public class MouseTargetFollower : MonoBehaviour, IUpdateListener
{
    [Header("Références")]
    [Tooltip("Đối tượng 3D sẽ di chuyển theo chuột (ví dụ: một Sphere nhỏ).")]
    [SerializeField] private Transform aimTargetObject;

    [Tooltip("Camera chính của người chơi.")]
    [SerializeField] private Camera mainCamera;

    [Header("Paramètres")]
    [Tooltip("Khoảng cách tối đa mà tia raycast có thể vươn tới.")]
    [SerializeField] private float maxDistance = 100f;

    [Tooltip("Các layer mà tia có thể va chạm (ví dụ: tường, đất, kẻ thù).")]
    [SerializeField] private LayerMask hittableLayers;

    private void OnEnable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.RegisterUpdateListener(this);
        }
    }

    private void OnDisable()
    {
        if (UpdateManager.Instance != null)
        {
            UpdateManager.Instance.UnregisterUpdateListener(this);
        }
    }

    public void OnUpdate(float delta)
    {
        // 1. Tạo một tia (Ray) từ camera đi qua vị trí của con trỏ chuột
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 2. Thực hiện Raycast
        // Physics.Raycast trả về true nếu tia va chạm với một vật thể nào đó
        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance, hittableLayers))
        {
            // Nếu có va chạm:
            // - Bật đối tượng mục tiêu lên (nếu nó đang bị ẩn)
            if (!aimTargetObject.gameObject.activeSelf)
            {
                aimTargetObject.gameObject.SetActive(true);
            }

            // - Di chuyển đối tượng mục tiêu đến chính xác vị trí va chạm
            aimTargetObject.position = hitInfo.point;
        }
        else
        {
            // Nếu không va chạm (ví dụ: chỉ vào bầu trời):
            // - Ẩn đối tượng mục tiêu đi
            if (aimTargetObject.gameObject.activeSelf)
            {
                aimTargetObject.gameObject.SetActive(false);
            }
        }
    }
}
