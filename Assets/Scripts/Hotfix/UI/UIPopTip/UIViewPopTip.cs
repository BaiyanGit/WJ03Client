using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 弹出提示界面
    /// </summary>
    public class UIViewPopTip : IUIView
    {
        public Image imgBg;
		public Transform tsWindow;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			tsWindow = handle.transform.Find("Img_Bg/Ts_Window").GetComponent<Transform>();
        }
    }
}