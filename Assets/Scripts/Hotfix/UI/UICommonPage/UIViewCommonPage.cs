using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewCommonPage : IUIView
    {
        public Image imgBg;
        public TMP_Text userNameText;
        public Button btnClose;
        public Button btnExit;
        public Button btnCallBack;
        public Button btnBackMain;
        public Toggle togMenu;
        public Toggle togSecond;
        public Toggle togThird;
        public Button btnSetting;

        public TMP_Text menuText;
        public TMP_Text secondText;
        public TMP_Text thirdText;

        public List<Toggle> ToggleList = new();
        public List<TMP_Text> ToggleTextList = new();

        public Transform bottomLine;

        public Button btnTaskComplete;

        public Toggle togIsAssessment;

        public Toggle togModelShowOrHide;
        public TMP_Text txtModelShowOrHide;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
            userNameText = handle.transform.Find("Img_Bg/Title/TitleMap/Tmp_Title").GetComponent<TMP_Text>();
            btnClose = handle.transform.Find("Img_Bg/Title/Btn_Close").GetComponent<Button>();
            btnExit = handle.transform.Find("Img_Bg/Title/Btn_Exit").GetComponent<Button>();
            btnCallBack = handle.transform.Find("Img_Bg/Btn_CallBack").GetComponent<Button>();
            btnBackMain = handle.transform.Find("Img_Bg/Btn_BackMain").GetComponent<Button>();

            btnTaskComplete = handle.transform.Find("Img_Bg/Btn_TaskComplete").GetComponent<Button>();

            togMenu = handle.transform.Find("Img_Bg/ShowList/Images/Tog_Menu").GetComponent<Toggle>();
            togSecond = handle.transform.Find("Img_Bg/ShowList/Images/Second/Tog_Second").GetComponent<Toggle>();
            togThird = handle.transform.Find("Img_Bg/ShowList/Images/Third/Tog_Third").GetComponent<Toggle>();

            btnSetting = handle.transform.Find("Img_Bg/Title/Btn_Setting").GetComponent<Button>();

            menuText = togMenu.transform.Find("Text").GetComponent<TMP_Text>();
            secondText = togSecond.transform.Find("Text").GetComponent<TMP_Text>();
            thirdText = togThird.transform.Find("Text").GetComponent<TMP_Text>();

            bottomLine = handle.transform.Find("Img_Bg/OtherBg/ImageDown");

            togIsAssessment = handle.transform.Find("Img_Bg/Tog_IsAssessment").GetComponent<Toggle>();

            togModelShowOrHide = handle.transform.Find("Img_Bg/Tog_ModelShowOrHide").GetComponent<Toggle>();
            txtModelShowOrHide = togModelShowOrHide.GetComponentInChildren<TMP_Text>();

            ToggleList.Add(togMenu);
            ToggleList.Add(togSecond);
            ToggleList.Add(togThird);
            ToggleTextList.Add(menuText);
            ToggleTextList.Add(secondText);
            ToggleTextList.Add(thirdText);

            togModelShowOrHide.isOn = false;
            // 启用Toggle点击事件 Owner: 王柏雁 2025-4-9 
            // togMenu.interactable = false;
            // togSecond.interactable = false;
            // togThird.interactable = false;
        }
    }
}