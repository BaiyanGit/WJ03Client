using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewEntry : IUIView
    {
        public Image imgBg;
		public Button btnSimulate;
		public Button btnNet;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			btnSimulate = handle.transform.Find("Img_Bg/Btn_Simulate").GetComponent<Button>();
			btnNet = handle.transform.Find("Img_Bg/Btn_Net").GetComponent<Button>();
        }
    }
}