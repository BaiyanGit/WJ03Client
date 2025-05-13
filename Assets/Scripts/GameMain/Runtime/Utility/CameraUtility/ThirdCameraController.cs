using UnityEngine;

namespace GameMain.Runtime.Utility
{
    public class ThirdCameraController : CameraControllerBase
    {
        //Ŀ������
        private readonly Transform _targetTran;
        //Ŀ������߶�
        private readonly float _targetHeight;
        //Ŀ��Xƫ��
        private readonly float _targetSide;
        //��Ŀ��λ�þ���
        private float _distance;
        //������
        private readonly float _maxDistance;
        //��С����
        private readonly float _minDistance;
        //������ͷ�ٶ�
        private readonly float _zoomRate;

        private float _x = 0;
        private float _y = 0;

        public ThirdCameraController(Camera camera, Transform targetTran, float x, float y, float targetHeight = 5f, float targetSide = 0.1f, float distance = 20, float maxDistance = 25f, float minDistance = 15f, float xSpeed = 250f, float ySpeed = 125f, float yMinLimit = -10f, float yMaxLimit = 72f, float zoomRate = 80f)
        {
            base.camera = camera;
            _targetTran = targetTran;
            _x = x;
            _y = y;
            _targetHeight = targetHeight;
            _targetSide = targetSide;
            _distance = distance;
            _maxDistance = maxDistance;
            _minDistance = minDistance;
            base.xSpeed = xSpeed;
            base.ySpeed = ySpeed;
            base.yMinLimit = yMinLimit;
            base.yMaxLimit = yMaxLimit;
            _zoomRate = zoomRate;
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            cameraVec.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse ScrollWheel"));

            _x += cameraVec.x * xSpeed * Time.deltaTime;
            _y -= cameraVec.y * ySpeed * Time.deltaTime;
            _y = ClampAngle(_y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(_y, _x, 0);

            camera.transform.rotation = rotation;

            _distance -= (cameraVec.z * Time.deltaTime) * _zoomRate * Mathf.Abs(_distance);
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
            camera.transform.localPosition = _targetTran.position + new Vector3(0, _targetHeight, 0) + rotation * (new Vector3(_targetSide, 0, -1) * _distance);
        }
    }
}

