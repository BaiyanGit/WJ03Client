using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 注册界面
    /// </summary>
    public class UIViewRegister : IUIView
    {
        public Image imgBg;
		public TMP_InputField tmpinputAccount;
		public TMP_InputField tmpinputPassword;
		public TMP_InputField tmpinputConfirm;
		public Button btnRegister;
		public Button btnClose;
		public Transform tsErrorNotification;
		public Transform tsSuccessNotification;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			tmpinputAccount = handle.transform.Find("Img_Bg/View/TmpInput_Account").GetComponent<TMP_InputField>();
			tmpinputPassword = handle.transform.Find("Img_Bg/View/TmpInput_Password").GetComponent<TMP_InputField>();
			tmpinputConfirm = handle.transform.Find("Img_Bg/View/TmpInput_Confirm").GetComponent<TMP_InputField>();
			btnRegister = handle.transform.Find("Img_Bg/View/Btn_Register").GetComponent<Button>();
			btnClose = handle.transform.Find("Img_Bg/View/Btn_Close").GetComponent<Button>();
			tsErrorNotification = handle.transform.Find("Img_Bg/Ts_ErrorNotification").GetComponent<Transform>();
			tsSuccessNotification = handle.transform.Find("Img_Bg/Ts_SuccessNotification").GetComponent<Transform>();
        }
    }
}