using UnityEngine;

public class YAxisFaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // 获取主相机
    }

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0; // 投影到XZ平面，忽略Y轴差异

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation; // 应用旋转，仅绕Y轴
        }
    }
}