using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 考核课题选择界面
    /// </summary>
    public class UIViewAssessmentSubject : IUIView
    {
        public Image imgBg;
		public TextMeshProUGUI tmptxtTitle;
		public Transform tsScrollView;
		public Transform tsContent;
		public TextMeshProUGUI tmptxtDescribe;
		public Button btnClose;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			tmptxtTitle = handle.transform.Find("Img_Bg/TitleMap/Background/TmpTxt_Title").GetComponent<TextMeshProUGUI>();
			tsScrollView = handle.transform.Find("Img_Bg/Ts_ScrollView").GetComponent<Transform>();
			tsContent = handle.transform.Find("Img_Bg/Ts_ScrollView/Viewport/Ts_Content").GetComponent<Transform>();
			tmptxtDescribe = handle.transform.Find("Img_Bg/Describe/TmpTxt_Describe").GetComponent<TextMeshProUGUI>();
			btnClose = handle.transform.Find("Img_Bg/Btn_Close").GetComponent<Button>();
        }
    }
}