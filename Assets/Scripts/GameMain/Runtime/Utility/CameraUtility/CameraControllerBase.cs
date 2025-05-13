using UnityEngine;

namespace GameMain.Runtime.Utility
{
    public class CameraControllerBase
    {
        protected Vector3 cameraVec;
        protected Camera camera;

        //X������ƶ��ٶ�
        protected float xSpeed;
        //Y������ƶ��ٶ�
        protected float ySpeed;
        //Y����С�Ƕ�����
        protected float yMinLimit;
        //Y�����Ƕ�����
        protected float yMaxLimit;

        public virtual void OnLateUpdate() { }

        protected float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}
