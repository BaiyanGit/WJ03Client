using Hotfix.ExcelData;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;
using Object = UnityEngine.Object;

public class UIDataScreenEngineView : MonoBehaviour
{
    List<Transform> engineInfoItems = new();
    Dictionary<int, List<string>> tmpEngineInfoItems = new Dictionary<int, List<string>>();

    public Image imgEnginePointer;
    public TextMeshProUGUI tmptxtRPMNum;
    public TextMeshProUGUI tmptxtTemperatureNum;
    public Transform tsNoisePos;
    public TextMeshProUGUI tmptxt_EngineOffsetX;
    public TextMeshProUGUI tmptxt_EngineOffsetY;
    public TextMeshProUGUI tmptxt_EngineOffsetZ;
    public Transform tsEngineNoiseSlider;
    public Transform tsEngineShakeSlider;
    public Transform tsEngineTemperatureSlider;
    public TextMeshProUGUI tmptxtNoiseNum;
    public TextMeshProUGUI tmptxtOffsetNum;
    public TextMeshProUGUI tmptxtSensorTemperatureNum;
    public Transform tsEngineInfoItem;
    public LineChart _engineTemperatureChart;

    //缓存Slider组件
    private Slider sEngineTemperatureSlider;
    private Slider sEngineNoiseSlider;

    private void OnEnable()
    {
        InitEngineDataItem();
    }

    void Awake()
    {
        imgEnginePointer = transform.Find("RPM_Bg/RPM/Img_EnginePointer").GetComponent<Image>();
        tmptxtRPMNum = transform.Find("RPM_Bg/RPMShowNum/TmpTxt_RPMNum").GetComponent<TextMeshProUGUI>();
        tmptxtTemperatureNum = transform.Find("Temperature_Bg/TemperatureShowNum/TmpTxt_TemperatureNum").GetComponent<TextMeshProUGUI>();
        tsNoisePos = transform.Find("Sensor/Ts_NoisePos").GetComponent<Transform>();
        tmptxt_EngineOffsetX = transform.Find("Sensor/Ts_NoisePos/Offset/Offset_Value/X/TmpTxt__EngineOffsetX").GetComponent<TextMeshProUGUI>();
        tmptxt_EngineOffsetY = transform.Find("Sensor/Ts_NoisePos/Offset/Offset_Value/Y/TmpTxt__EngineOffsetY").GetComponent<TextMeshProUGUI>();
        tmptxt_EngineOffsetZ = transform.Find("Sensor/Ts_NoisePos/Offset/Offset_Value/Z/TmpTxt__EngineOffsetZ").GetComponent<TextMeshProUGUI>();
        tsEngineNoiseSlider = transform.Find("Sensor/Ts_NoisePos/Noise/Ts_EngineNoiseSlider").GetComponent<Transform>();
        tsEngineShakeSlider = transform.Find("Sensor/Ts_NoisePos/Shake/Ts_EngineShakeSlider").GetComponent<Transform>();
        tsEngineTemperatureSlider = transform.Find("Sensor/Ts_NoisePos/Temperature/Ts_EngineTemperatureSlider").GetComponent<Transform>();
        tmptxtNoiseNum = transform.Find("Sensor/SensorParam/Noise/TmpTxt_NoiseNum").GetComponent<TextMeshProUGUI>();
        tmptxtOffsetNum = transform.Find("Sensor/SensorParam/Offset/TmpTxt_OffsetNum").GetComponent<TextMeshProUGUI>();
        tmptxtSensorTemperatureNum = transform.Find("Sensor/SensorParam/Temperature/TmpTxt_SensorTemperatureNum").GetComponent<TextMeshProUGUI>();
        tsEngineInfoItem = transform.Find("DataInfo/DataInfoItems/Ts_EngineInfoItem").GetComponent<Transform>();

        _engineTemperatureChart = transform.Find("Temperature_Bg/CylinderTemperature").GetComponent<LineChart>();

        //缓存Slider组件
        sEngineTemperatureSlider = tsEngineTemperatureSlider.GetComponent<Slider>();
        sEngineNoiseSlider = tsEngineNoiseSlider.GetComponent<Slider>();

        Ctrl_MessageCenter.AddMsgListener<float>("缸温", OnGangWenChange);
        Ctrl_MessageCenter.AddMsgListener<float>("油温", OnYouWenChange);
        Ctrl_MessageCenter.AddMsgListener<float, float, float>("震动频率", OnZhenDongPinLvChange);
        Ctrl_MessageCenter.AddMsgListener<float>("温度", OnWenDuChange);
        Ctrl_MessageCenter.AddMsgListener<float>("噪声", OnZaoShengChange);
        Ctrl_MessageCenter.AddMsgListener<float>("指针旋转-rpm", OnUpdateNeedleRotationRPM);
        Ctrl_MessageCenter.AddMsgListener<float>("指针旋转-angle", OnUpdateNeedleRotationAngle);
    }

    /// <summary>
    /// 初始化发动机界面数据
    /// </summary>
    void InitEngineDataItem()
    {
        foreach (var item in engineInfoItems)
        {
            Object.Destroy(item.gameObject);
        }

        engineInfoItems.Clear();
        tmpEngineInfoItems.Clear();
        //if (engineInfoItems.Count > 5)
        //{
        //    Object.Destroy(engineInfoItems[0]);
        //    engineInfoItems.RemoveAt(0);
        //}

        List<DataCheckConfig> defaultValue = DataCheckTopicManager.Instance.DataCheckDefaultValue;

        int index = 0;
        defaultValue.ForEach(config =>
        {
            if (!tmpEngineInfoItems.ContainsKey(config.Id) && config.Type == 1)
            {
                tmpEngineInfoItems.Add(index, new List<string>() { "错误", config.Title + "异常" });
            }
            index += 1;
        });


        for (int i = 0; i < tmpEngineInfoItems.Count; i++)
        {
            Transform infoItem = Object.Instantiate(tsEngineInfoItem, tsEngineInfoItem.parent);
            DataScreenInfoItem dataScreen = infoItem.gameObject.AddComponent<DataScreenInfoItem>();
            dataScreen.InitData(tmpEngineInfoItems[i][0], tmpEngineInfoItems[i][1]);
            engineInfoItems.Add(infoItem);
        }
    }

    private void OnGangWenChange(float value)
    {
        tmptxtTemperatureNum.text = value.ToString();
        DataCheckTopicManager.Instance.UpdateLineData(_engineTemperatureChart, "缸温", value);
    }

    private void OnYouWenChange(float value)
    {
        tmptxtTemperatureNum.text = value.ToString();
    }

    private void OnZhenDongPinLvChange(float x, float y, float z)
    {
        tmptxt_EngineOffsetX.text = x.ToString("F1");
        tmptxt_EngineOffsetY.text = y.ToString("F1");
        tmptxt_EngineOffsetZ.text = z.ToString("F1");
    }

    private void OnWenDuChange(float value)
    {
        tmptxtSensorTemperatureNum.text = value.ToString();
        sEngineTemperatureSlider.value = value / 180;
    }

    private void OnZaoShengChange(float value)
    {
        tmptxtNoiseNum.text = value.ToString();
        sEngineNoiseSlider.value = value / 150;
    }

    /// <summary>
    /// 设置指针旋转
    /// </summary>
    /// <param name="rpm"></param>
    private void OnUpdateNeedleRotationRPM(float rpm)
    {
        // 将转速映射到角度
        tmptxtRPMNum.text = rpm.ToString("f0");
    }

    private void OnUpdateNeedleRotationAngle(float angle)
    {
        // 设置指针的旋转
        imgEnginePointer.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}