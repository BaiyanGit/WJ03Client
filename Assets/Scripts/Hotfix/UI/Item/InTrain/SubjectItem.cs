using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix.UI
{
    public class SubjectItem : MonoBehaviour
    {
        [SerializeField] private Button subjectBtn;
        [SerializeField] private TextMeshProUGUI subjectNameText;

        private SubjectData _subjectData;
        private EnumMonitorMode _monitorMode;

        private void Awake()
        {
            subjectBtn.onClick.AddListener(OnSubjectHandle);
        }

        private void OnSubjectHandle()
        {
            switch (_monitorMode)
            {
                case EnumMonitorMode.Train:
                    var uiTrainMonitoringData = new UITrainMonitoringData()
                    {
                        subjectId = _subjectData.subjectId,
                        subjectName = _subjectData.name
                    };
                    GameEntry.UI.OpenUIFormAsync<UITrainMonitoring>(userData:uiTrainMonitoringData).Forget();
                    break;
                case EnumMonitorMode.Assessment:
                    var uiAssessmentMonitoringData = new UIAssessmentMonitoringData()
                    {
                        subjectId = _subjectData.subjectId,
                        subjectName = _subjectData.name
                    };
                    GameEntry.UI.OpenUIFormAsync<UIAssessmentMonitoring>(userData:uiAssessmentMonitoringData).Forget();
                    break;
                default:
                    break;
            }
        }

        public void InitData(SubjectData subjectData,EnumMonitorMode monitorMode)
        {
            _subjectData = subjectData;
            _monitorMode = monitorMode;
            subjectNameText.text = _subjectData.name;
        }
    }
}