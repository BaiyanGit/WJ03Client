using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 训练监测
    /// </summary>
    public class UIViewTrainMonitoring : IUIView
    {
        public Image imgBg;
		public RawImage rawModel;
		public Transform tsCheckContent;
		public Transform tsLeftToggleList;
		public Transform tsCheckItemList;
		public Button btnClose;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			rawModel = handle.transform.Find("Img_Bg/ModelShowContent/Raw_Model").GetComponent<RawImage>();
			tsCheckContent = handle.transform.Find("Img_Bg/TipList/Viewport/Ts_CheckContent").GetComponent<Transform>();
			tsLeftToggleList = handle.transform.Find("Img_Bg/LeftToggleList/Viewport/Ts_LeftToggleList").GetComponent<Transform>();
			tsCheckItemList = handle.transform.Find("Img_Bg/CheckItemList/Viewport/Ts_CheckItemList").GetComponent<Transform>();
			btnClose = handle.transform.Find("Img_Bg/Btn_Close").GetComponent<Button>();
        }
    }
}