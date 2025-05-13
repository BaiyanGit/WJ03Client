using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime.UI;
using TMPro;
using XCharts.Runtime;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIViewDataScreen : IUIView
    {
        public Image imgBg;
        public Button btnClose;
        public TextMeshProUGUI tmpTimeNow;
        public Transform tsAllView;
        
        public Image imgPointer;
        public Transform tsInfoItem;
        public Image imgElectricity;
        public TextMeshProUGUI tmptxt_Electricity;
        public TextMeshProUGUI tmptxt_Mileage;
        public TextMeshProUGUI tmptxt_Speed;
        public Transform tsToggleList;
        public Toggle tog1;
        public Toggle tog2;
        public Toggle tog3;
        public Toggle tog4;
        public TextMeshProUGUI tmptxt_OffsetX;
        public TextMeshProUGUI tmptxt_OffsetY;
        public TextMeshProUGUI tmptxt_OffsetZ;
        public Transform tsNoiseSlider;
        public Transform tsShakeSlider;
        public Transform tsTemperatureSlider;
        
        public LineChart _oilTemperatureChart;
        public LineChart _cylinderTemperatureChart;
        public LineChart _kPALineChart;
        public LineChart _aPSLineChart;
        public LineChart _oilKPAChart;
        public BarChart _oilChart;
        
        public Transform tsEngineView;
        public Transform tsChassisView;
        public Transform tsElectricView;

        public void Init(GameObject handle)
        {
            imgBg = handle.transform.Find("Img_Bg").GetComponent<Image>();
            btnClose = handle.transform.Find("Img_Bg/Btn_Close").GetComponent<Button>();
            tmpTimeNow = handle.transform.Find("Img_Bg/TmpTxt_Time").GetComponent<TextMeshProUGUI>();
            tsAllView = handle.transform.Find("Img_Bg/AllView");
            
            imgPointer = tsAllView.Find("RPM/Img_Pointer").GetComponent<Image>();
            tsInfoItem = tsAllView.Find("DataInfoItems/Ts_InfoItem").GetComponent<Transform>();
            imgElectricity = tsAllView.Find("Electricity/Img_Electricity").GetComponent<Image>();
            tmptxt_Electricity = tsAllView.Find("Electricity/TmpTxt__Electricity").GetComponent<TextMeshProUGUI>();
            tmptxt_Mileage = tsAllView.Find("SpeedAndMileage/TmpTxt__Mileage").GetComponent<TextMeshProUGUI>();
            tmptxt_Speed = tsAllView.Find("SpeedAndMileage/Speed/TmpTxt__Speed").GetComponent<TextMeshProUGUI>();
            tsToggleList = tsAllView.Find("Ts_ToggleList").GetComponent<Transform>();
            tog1 = tsAllView.Find("Ts_ToggleList/Tog_1").GetComponent<Toggle>();
            tog2 = tsAllView.Find("Ts_ToggleList/Tog_2").GetComponent<Toggle>();
            tog3 = tsAllView.Find("Ts_ToggleList/Tog_3").GetComponent<Toggle>();
            tog4 = tsAllView.Find("Ts_ToggleList/Tog_4").GetComponent<Toggle>();
            tmptxt_OffsetX = tsAllView.Find("Noise/Noise_Bg/Offset/Offset_Value/X/TmpTxt__OffsetX").GetComponent<TextMeshProUGUI>();
            tmptxt_OffsetY = tsAllView.Find("Noise/Noise_Bg/Offset/Offset_Value/Y/TmpTxt__OffsetY").GetComponent<TextMeshProUGUI>();
            tmptxt_OffsetZ = tsAllView.Find("Noise/Noise_Bg/Offset/Offset_Value/Z/TmpTxt__OffsetZ").GetComponent<TextMeshProUGUI>();
            tsNoiseSlider = tsAllView.Find("Noise/Noise_Bg/Noise/Ts_NoiseSlider").GetComponent<Transform>();
            tsShakeSlider = tsAllView.Find("Noise/Noise_Bg/Shake/Ts_ShakeSlider").GetComponent<Transform>();
            tsTemperatureSlider = tsAllView.Find("Noise/Noise_Bg/Temperature/Ts_TemperatureSlider").GetComponent<Transform>();

            _oilTemperatureChart = tsAllView.Find("XCharts/OilTemperature").GetComponent<LineChart>();
            _cylinderTemperatureChart = tsAllView.Find("XCharts/CylinderTemperature").GetComponent<LineChart>();
            _kPALineChart = tsAllView.Find("XCharts/KPA").GetComponent<LineChart>();
            _aPSLineChart = tsAllView.Find("XCharts/APS").GetComponent<LineChart>();
            _oilKPAChart = tsAllView.Find("XCharts/OilKPA").GetComponent<LineChart>();
            _oilChart = tsAllView.Find("XCharts/Oil").GetComponent<BarChart>();
            
            tsEngineView = handle.transform.Find("Img_Bg/EngineView");
            tsChassisView = handle.transform.Find("Img_Bg/ChassisView");
            tsElectricView = handle.transform.Find("Img_Bg/ElectricView");
        }
    }
}