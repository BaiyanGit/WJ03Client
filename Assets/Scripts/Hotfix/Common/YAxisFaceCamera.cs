using UnityEngine;

public class YAxisFaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // ��ȡ�����
    }

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

        Vector3 direction = mainCamera.transform.position - transform.position;
        direction.y = 0; // ͶӰ��XZƽ�棬����Y�����

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation; // Ӧ����ת������Y��
        }
    }
}