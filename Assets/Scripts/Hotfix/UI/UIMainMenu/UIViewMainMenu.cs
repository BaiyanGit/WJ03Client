using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewMainMenu : IUIView
    {
        public Image imgBg;
		public Toggle togTeach;
		public Toggle togFalutTrain;
		public Toggle togFaultExam;
        public Toggle togVirtualTrain;
        public Toggle togDigitalTwin;
        public TextMeshProUGUI tmptxtIntroduce;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			togTeach = handle.transform.Find("Img_Bg/ContentList/Tog_Teach").GetComponent<Toggle>();
			togFalutTrain = handle.transform.Find("Img_Bg/ContentList/Tog_FalutTrain").GetComponent<Toggle>();
			togFaultExam = handle.transform.Find("Img_Bg/ContentList/Tog_FaultExam").GetComponent<Toggle>();
            togVirtualTrain = handle.transform.Find("Img_Bg/ContentList/Tog_VirtualTrain").GetComponent<Toggle>();
            togDigitalTwin = handle.transform.Find("Img_Bg/ContentList/Tog_DigitalTwin").GetComponent<Toggle>();
            tmptxtIntroduce = handle.transform.Find("Img_Bg/TmpTxt_Introduce").GetComponent<TextMeshProUGUI>();
        }
    }
}