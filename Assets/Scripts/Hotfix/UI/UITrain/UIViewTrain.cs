using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewTrain : IUIView
    {
        public Image imgBg;
		public RawImage rawModel;
		public Transform tsContentList;
		public Button btnPageUp;
		public Button btnNextPage;
		public Transform tsShowItem;
		public TextMeshProUGUI tmptxtTitle;
		public Toggle togCheckPointItem;
		public Transform toggleItem;

		public Transform _pointTip;

		public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			rawModel = handle.transform.Find("Img_Bg/ModelShowContent/Raw_Model").GetComponent<RawImage>();
			tsContentList = handle.transform.Find("Img_Bg/Ts_ContentList").GetComponent<Transform>();
			btnPageUp = handle.transform.Find("Img_Bg/PageControl/Btn_PageUp").GetComponent<Button>();
			btnNextPage = handle.transform.Find("Img_Bg/PageControl/Btn_NextPage").GetComponent<Button>();
			tsShowItem = handle.transform.Find("Img_Bg/Ts_ShowItem").GetComponent<Transform>();
			tmptxtTitle = handle.transform.Find("Img_Bg/Ts_ShowItem/TmpTxt_Title").GetComponent<TextMeshProUGUI>();
			togCheckPointItem = handle.transform.Find("Img_Bg/Ts_ShowItem/CheckPointItemList/Tog_CheckPointItem").GetComponent<Toggle>();
            toggleItem = GameEntry.Resource.BuildInResource.Load<Transform>(AppConst.AssetPathConst.TrainItem);

			_pointTip = Resources.Load<Transform>("PointTip");
		}
    }
}