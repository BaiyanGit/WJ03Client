using UnityEngine;

namespace GameMain.Runtime.Utility
{
    public class CameraControllerBase
    {
        protected Vector3 cameraVec;
        protected Camera camera;

        //X轴鼠标移动速度
        protected float xSpeed;
        //Y轴鼠标移动速度
        protected float ySpeed;
        //Y轴最小角度限制
        protected float yMinLimit;
        //Y轴最大角度限制
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
