using UnityEngine;

namespace Hotfix
{
    public partial class AppConst
    {
        public class Constant
        {
            public static Vector3 CameraDefaultPos = new(6.599965f, 2.546934f, -5.698928f);
            public static Quaternion CameraDefaultRot = Quaternion.Euler(11.245f, -50f, 0f);
            /// <summary>
            /// 大屏布局的最大索引
            /// </summary>
            public static int BigScreenLayoutIndexMax = 15;
        }
    }
}