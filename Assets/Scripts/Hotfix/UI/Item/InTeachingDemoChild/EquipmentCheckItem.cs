using UnityEngine;
using Hotfix.ExcelData;
using TMPro;
using System.Collections.Generic;
using XCharts.Runtime;
using UnityEngine.UI;
using System;

namespace Hotfix.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class EquipmentCheckItem : MonoBehaviour
    {
        public TextMeshProUGUI titleTmp;
        public TextMeshProUGUI targetValue;
        public TextMeshProUGUI currentValue;

        private Transform labelTip;
        private Transform _labelTip;

        //public TextMeshProUGUI lineReferenceValue;
        public TextMeshProUGUI lineCurrentValue;

        public LineChart lineChart;

        public Canvas ui_Canvas;
        public Camera _camera;

        public Transform tipParent;

        public RectTransform pointImage;
        public RectTransform targetPoint;
        public LineRenderer line;
        Toggle currentToggle;

        private void Awake()
        {
            _labelTip = Resources.Load<Transform>("Prefab/OtherModels/LabelTip");
            ui_Canvas = GameObject.Find("UI Form Instance").GetComponent<Canvas>();
            currentToggle =this.GetComponent<Toggle>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="config2nd"></param>
        public void Init(EquipmentCheckConfig1st itemData, EquipmentCheckConfig2nd config2nd, Transform parent = null)
        {
            titleTmp.text = config2nd.Title;
            tipParent = parent;

            //targetValue.text = itemData.TargetValues.Length > 1 ? itemData.TargetValues[0] + "-" + itemData.TargetValues[1] : itemData.TargetValues[0].ToString();

            InitLabelTip();

            labelTip = parent == null ? Instantiate(_labelTip, transform) : Instantiate(_labelTip, parent);
            labelTip.localPosition = Vector3.zero;
            labelTip.gameObject.SetActive(false);

            labelTip.Find("Content/ReferenceValue/Tmp_ReferenceValue").GetComponent<TextMeshProUGUI>().text = targetValue.text;
            lineCurrentValue = labelTip.Find("Content/CurrentValue/Tmp_CurrentValue").GetComponent<TextMeshProUGUI>();
            lineChart = labelTip.Find("Content/Chart/LineChart").GetComponent<LineChart>();
            pointImage = labelTip.Find("PointImage") as RectTransform;
            targetPoint = labelTip.Find("Content/Chart") as RectTransform;
            //line = labelTip.Find("Line").GetComponent<LineRenderer>();

            RectTransform tipRect = labelTip as RectTransform;

            //tipRect.GetComponent<LabelTipItem>().InitFollowPos(new Vector3(config2nd.TargetPos[0], config2nd.TargetPos[1], config2nd.TargetPos[2]), tipParent);
            //tipRect.anchoredPosition = ChangeUIPosition(new Vector3(config2nd.TargetPos[0], config2nd.TargetPos[1], config2nd.TargetPos[2]));

            //List<Vector3> points = new List<Vector3>();
            //points.Add(new Vector3(tipRect.anchoredPosition.x + 960, tipRect.anchoredPosition.y + 540, 0));
            //points.Add(new Vector3(tipRect.anchoredPosition.x + 960 + 135, tipRect.anchoredPosition.y + 540 + 139, 0));
            //points.Add(new Vector3(tipRect.anchoredPosition.x + 960 + 186, tipRect.anchoredPosition.y + 540 + 139, 0));
            //line.SetPositions(points.ToArray());
            currentToggle.onValueChanged.AddListener(onToggleValueChange);
        }

        private void onToggleValueChange(bool arg0)
        {
            labelTip.gameObject.SetActive(arg0);
        }

        /// <summary>
        /// 修改当前值
        /// </summary>
        private void ChangeCurrentValue(float value)
        {
            lineCurrentValue.text = currentValue.text = value.ToString();
            Serie serie = lineChart.series[0];
            SerieData serieData = new();
            serieData.data = new List<double> { value, value };
            serie.data.Add(serieData);
        }

        public void InitLabelTip()
        {
            if (labelTip != null)
            {
                Destroy(labelTip.gameObject);
            }
        }
    }

}

