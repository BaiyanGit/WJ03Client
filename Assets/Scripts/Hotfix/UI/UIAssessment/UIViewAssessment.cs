using TMPro;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewAssessment : IUIView
    {
        public Image imgBg;
        public Toggle togCustomItem;
        public Toggle togTeacherItem;
        public Toggle togTrainItem;
        public Toggle togTaskItem;
        public TextMeshProUGUI tmptxtIntroduce;

        public VideoPlayer videoPlayer;
        public VideoClip clip;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
            togCustomItem = handle.transform.Find("Img_Bg/ContentList/Tog_Self").GetComponent<Toggle>();
            togTeacherItem = handle.transform.Find("Img_Bg/ContentList/Tog_Teacher").GetComponent<Toggle>();
            togTrainItem = handle.transform.Find("Img_Bg/ContentList/Tog_Train").GetComponent<Toggle>();
            togTaskItem = handle.transform.Find("Img_Bg/ContentList/Tog_Task").GetComponent<Toggle>();
            tmptxtIntroduce = handle.transform.Find("Img_Bg/TmpTxt_Introduce").GetComponent<TextMeshProUGUI>();

            videoPlayer = handle.transform.GetComponentInChildren<VideoPlayer>();
            clip = Resources.Load<VideoClip>("Video/Clip2");
        }
    }
}