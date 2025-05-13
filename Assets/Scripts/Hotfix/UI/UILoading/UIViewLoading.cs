using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 加载界面
    /// </summary>
    public class UIViewLoading : IUIView
    {
        public Image imgBg;
		public Transform tsProgressBarCircularLoop;
		public Transform tsProgressBarBubble;
		public Transform tsWindow;
		public TextMeshProUGUI tmptxtWindowTitle;
		public TextMeshProUGUI tmptxtContent;
		public Button btnCancel;
		public TextMeshProUGUI tmptxtCancel;
		public Button btnConfirm;
		public TextMeshProUGUI tmptxtConfirm;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			tsProgressBarCircularLoop = handle.transform.Find("Img_Bg/Ts_ProgressBarCircularLoop").GetComponent<Transform>();
			tsProgressBarBubble = handle.transform.Find("Img_Bg/Ts_ProgressBarBubble").GetComponent<Transform>();
			tsWindow = handle.transform.Find("Img_Bg/Ts_Window").GetComponent<Transform>();
			tmptxtWindowTitle = handle.transform.Find("Img_Bg/Ts_Window/TitleBar/TmpTxt_WindowTitle").GetComponent<TextMeshProUGUI>();
			tmptxtContent = handle.transform.Find("Img_Bg/Ts_Window/TmpTxt_Content").GetComponent<TextMeshProUGUI>();
			btnCancel = handle.transform.Find("Img_Bg/Ts_Window/ButtonBar/Btn_Cancel").GetComponent<Button>();
			tmptxtCancel = handle.transform.Find("Img_Bg/Ts_Window/ButtonBar/Btn_Cancel/Background/TmpTxt_Cancel").GetComponent<TextMeshProUGUI>();
			btnConfirm = handle.transform.Find("Img_Bg/Ts_Window/ButtonBar/Btn_Confirm").GetComponent<Button>();
			tmptxtConfirm = handle.transform.Find("Img_Bg/Ts_Window/ButtonBar/Btn_Confirm/Background/TmpTxt_Confirm").GetComponent<TextMeshProUGUI>();
        }
    }
}