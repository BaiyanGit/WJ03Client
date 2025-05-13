using TMPro;

using UnityEngine;

namespace Hotfix.UI
{
    /// <summary>
    /// 结算条目
    /// </summary>
    public class SettlementItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI settlementTitle;
        [SerializeField] private TextMeshProUGUI settlementContent;
        [SerializeField] private TextMeshProUGUI settlementScore;

        public void InitData(ResponseRecordExamInfo recordExamInfo)
        {
            settlementTitle.text = recordExamInfo.PointID.ToString();
            settlementContent.text = recordExamInfo.CreateTime.ToString();
            settlementScore.text = recordExamInfo.Score.ToString();
        }

        public void InitData(string title, string content = null, string score = null)
        {
            settlementTitle.text = title;
            settlementContent.text = content;
            settlementScore.text = score;
        }
    }
}