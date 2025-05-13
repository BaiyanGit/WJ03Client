using UnityEngine;

namespace GameMain.Runtime.Utility
{
    public class FirstCameraController : CameraControllerBase
    {
        private float _x = 0;
        private float _y = 0;
        public FirstCameraController(Camera camera, float x, float y, float xSpeed = 250f, float ySpeed = 125f, float yMinLimit = -10f, float yMaxLimit = 72f)
        {
            base.camera = camera;
            _x = x;
            _y = y;
            base.xSpeed = xSpeed;
            base.ySpeed = ySpeed;
            base.yMinLimit = yMinLimit;
            base.yMaxLimit = yMaxLimit;
        }


        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            cameraVec.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            _x += cameraVec.x * xSpeed * Time.deltaTime;
            _y -= cameraVec.y * ySpeed * Time.deltaTime;
            _y = ClampAngle(_y, yMinLimit, yMaxLimit);
            var rotation = Quaternion.Euler(_y, _x, 0);

            camera.transform.localRotation = rotation;
        }
    }
}
