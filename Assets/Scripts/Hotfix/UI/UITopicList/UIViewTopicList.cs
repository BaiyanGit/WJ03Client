using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewTopicList : IUIView
    {
        public Image imgBg;
		public RawImage rawModel;
		public Transform tsContentList;
		public Button btnPageUp;
		public Button btnNextPage;
        public Transform toggleItem;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
			rawModel = handle.transform.Find("Img_Bg/ModelShowContent/Raw_Model").GetComponent<RawImage>();
			tsContentList = handle.transform.Find("Img_Bg/Ts_ContentList").GetComponent<Transform>();
			btnPageUp = handle.transform.Find("Img_Bg/PageControl/Btn_PageUp").GetComponent<Button>();
			btnNextPage = handle.transform.Find("Img_Bg/PageControl/Btn_NextPage").GetComponent<Button>();

            toggleItem = GameEntry.Resource.BuildInResource.Load<Transform>(AppConst.AssetPathConst.TopicItem);
        }
    }
}