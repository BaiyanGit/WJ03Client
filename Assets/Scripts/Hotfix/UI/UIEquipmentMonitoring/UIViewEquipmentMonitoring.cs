using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewEquipmentMonitoring : IUIView
    {
        public Image imgBg;
		public RawImage rawModel;
		public Button btnPageUp;
		public Button btnNextPage;
		public Transform tsContentList;
		public TextMeshProUGUI tmptxtIntroduce;
		public Transform tsShowItem;
		public Toggle togItem;
		public Transform tsShowTips;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			rawModel = handle.transform.Find("Img_Bg/ModelShowContent/Raw_Model").GetComponent<RawImage>();
			btnPageUp = handle.transform.Find("Img_Bg/LeftToggleList/PageControl/Btn_PageUp").GetComponent<Button>();
			btnNextPage = handle.transform.Find("Img_Bg/LeftToggleList/PageControl/Btn_NextPage").GetComponent<Button>();
			tsContentList = handle.transform.Find("Img_Bg/LeftToggleList/Ts_ContentList").GetComponent<Transform>();
			tmptxtIntroduce = handle.transform.Find("Img_Bg/TmpTxt_Introduce").GetComponent<TextMeshProUGUI>();
			tsShowItem = handle.transform.Find("Img_Bg/Ts_ShowItem").GetComponent<Transform>();
			togItem = handle.transform.Find("Img_Bg/Ts_ShowItem/ItemList/Tog_Item").GetComponent<Toggle>();
			tsShowTips = handle.transform.Find("Img_Bg/Ts_ShowTips").GetComponent<Transform>();
        }
    }
}