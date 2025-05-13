using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;
using UnityEngine.Video;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewLogin : IUIView
    {
        public Image imgBg;
		public Transform tsVideoPlayer;
		public TMP_InputField tmpinputAccount;
		public TMP_InputField tmpinputPassword;
		public Button btnLogin;
		public Button btnClose;
		public Button btnRegister;
		public Transform tsNotification;
		public Toggle togGuest;
		public VideoPlayer videoPlayer;
		public VideoClip clip;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			tsVideoPlayer = handle.transform.Find("Img_Bg/VideoPlay_Bg/Ts_VideoPlayer").GetComponent<Transform>();
			tmpinputAccount = handle.transform.Find("Img_Bg/View/TmpInput_Account").GetComponent<TMP_InputField>();
			tmpinputPassword = handle.transform.Find("Img_Bg/View/TmpInput_Password").GetComponent<TMP_InputField>();
			btnLogin = handle.transform.Find("Img_Bg/View/Btn_Login").GetComponent<Button>();
			btnClose = handle.transform.Find("Img_Bg/View/Btn_Close").GetComponent<Button>();
			btnRegister = handle.transform.Find("Img_Bg/View/Btn_Register").GetComponent<Button>();
			tsNotification = handle.transform.Find("Img_Bg/Ts_Notification").GetComponent<Transform>();
            togGuest = handle.transform.Find("Img_Bg/View/ToggleBasicText_Fade").GetComponent<Toggle>();

			videoPlayer = tsVideoPlayer.GetComponent<VideoPlayer>();
			clip = Resources.Load<VideoClip>("Video/Clip2");
        }
    }
}