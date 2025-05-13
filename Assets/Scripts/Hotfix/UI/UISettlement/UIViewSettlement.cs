using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;

namespace Hotfix.UI
{
    /// <summary>
    /// 结算界面
    /// </summary>
    public class UIViewSettlement : IUIView
    {
        public Image imgBg;
        public Transform tsSettlementContent;
        public TextMeshProUGUI tmptxtModelName;
        public TextMeshProUGUI tmptxtErrorCount;
        public TextMeshProUGUI tmptxtTime;
        public TextMeshProUGUI tmptxtScore;
        public Button btnClose;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
            tsSettlementContent = handle.transform.Find("Img_Bg/Content/ScrollView/Viewport/Ts_SettlementContent").GetComponent<Transform>();
            //tmptxtModelName = handle.transform.Find("Img_Bg/Content/ScrollView/Viewport/Ts_SettlementContent/ModelName/Swap1/TmpTxt_ModelName").GetComponent<TextMeshProUGUI>();
            //tmptxtErrorCount = handle.transform.Find("Img_Bg/Content/ScrollView/Viewport/Ts_SettlementContent/ErrorCount/Swap1/TmpTxt_ErrorCount").GetComponent<TextMeshProUGUI>();
            tmptxtTime = handle.transform.Find("Img_Bg/Time/TmpTxt_Time").GetComponent<TextMeshProUGUI>();
            tmptxtScore = handle.transform.Find("Img_Bg/Score/TmpTxt_Score").GetComponent<TextMeshProUGUI>();
            btnClose = handle.transform.Find("Img_Bg/Btn_Close").GetComponent<Button>();
        }
    }
}