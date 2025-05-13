using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewTeachingDemo : IUIView
    {
        public Image imgBg;
		public Toggle togStructuralCognition;
		public Toggle togPrincipleLearning;
		public Toggle togEquipmentMonitoring;
		public TextMeshProUGUI tmptxtIntroduce;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			togStructuralCognition = handle.transform.Find("Img_Bg/ContentList/Tog_StructuralCognition").GetComponent<Toggle>();
			togPrincipleLearning = handle.transform.Find("Img_Bg/ContentList/Tog_PrincipleLearning").GetComponent<Toggle>();
			togEquipmentMonitoring = handle.transform.Find("Img_Bg/ContentList/Tog_EquipmentMonitoring").GetComponent<Toggle>();
			tmptxtIntroduce = handle.transform.Find("Img_Bg/TmpTxt_Introduce").GetComponent<TextMeshProUGUI>();
        }
    }
}