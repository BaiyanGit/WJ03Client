using System;
using TMPro;
using UnityEngine;
using XCharts.Runtime;

public class UIDataScreenChassisView : MonoBehaviour
{
    public TextMeshProUGUI tmptxtChassisOilNum;
    public TextMeshProUGUI tmptxtAPSNum;
    public TextMeshProUGUI tmptxt_ChassisMileage;
    public TextMeshProUGUI tmptxt_ChassisSpeed;
    public TextMeshProUGUI tmptxtMileageNum;
    public TextMeshProUGUI tmptxtSpeedNum;
    public TextMeshProUGUI tmptxtOilTemperatureNum;
    
    public LineChart _chassisAPSLineChart;
    public LineChart _chassisOilTemperatureChart;
    public BarChart _chassisOilChart;

    void Awake()
    {
        tmptxtChassisOilNum = transform.Find("Oil_Bg/OilShowNum/TmpTxt_ChassisOilNum").GetComponent<TextMeshProUGUI>();
        tmptxtAPSNum = transform.Find("APS_Bg/APSShowNum/TmpTxt_APSNum").GetComponent<TextMeshProUGUI>();
        tmptxt_ChassisMileage = transform.Find("SpeedAndMileage_Bg/SpeedAndMileage/TmpTxt__ChassisMileage").GetComponent<TextMeshProUGUI>();
        tmptxt_ChassisSpeed = transform.Find("SpeedAndMileage_Bg/SpeedAndMileage/Speed/TmpTxt__ChassisSpeed").GetComponent<TextMeshProUGUI>();
        tmptxtMileageNum = transform.Find("SpeedAndMileage_Bg/SpeedAndMileageShowNum/Mileage/TmpTxt_MileageNum").GetComponent<TextMeshProUGUI>();
        tmptxtSpeedNum = transform.Find("SpeedAndMileage_Bg/SpeedAndMileageShowNum/Speed/TmpTxt_SpeedNum").GetComponent<TextMeshProUGUI>();
        tmptxtOilTemperatureNum = transform.Find("OilTemperature_Bg/OilTemperatureShowNum/Temperature/TmpTxt_OilTemperatureNum").GetComponent<TextMeshProUGUI>();
        
        _chassisAPSLineChart = transform.Find("APS_Bg/APS").GetComponent<LineChart>();
        _chassisOilTemperatureChart = transform.Find("OilTemperature_Bg/OilTemperature").GetComponent<LineChart>();
        _chassisOilChart = transform.Find("Oil_Bg/Oil").GetComponent<BarChart>();
        
        Ctrl_MessageCenter.AddMsgListener<float>("压力", OnYaLiChange);
        Ctrl_MessageCenter.AddMsgListener<float>("油量", OnYouLiangChange);
        Ctrl_MessageCenter.AddMsgListener<int, int, int>("Speed-Mileage-Electricity", OnZhenDongPinLvChange);
        Ctrl_MessageCenter.AddMsgListener<float>("油温", OnYouWenChange);
    }

    private void OnYouWenChange(float obj)
    {
        tmptxtOilTemperatureNum.text = obj.ToString("f0");
        DataCheckTopicManager.Instance.UpdateLineData(_chassisOilTemperatureChart, "油温", obj);
    }

    private void OnYaLiChange(float value)
    {
        tmptxtAPSNum.text = value.ToString();
        DataCheckTopicManager.Instance.UpdateLineData(_chassisAPSLineChart, "压力", value);
    }

    private void OnYouLiangChange(float value)
    {
        tmptxtChassisOilNum.text = value.ToString("f0");
        //_chassisOilChart.AddData("油量", value);

        var serieData = _chassisOilChart.GetSerie("油量").data;
        int dataNum = serieData.Count;
        serieData.RemoveAt(0);
        for (int i = 0; i < dataNum - 1; i++)
        {
            serieData[i].data[0] = i;
        }

        _chassisOilChart.AddData("油量", value);
    }

    private void OnZhenDongPinLvChange(int x, int y, int z)
    {
        tmptxtSpeedNum.text = tmptxt_ChassisSpeed.text = x.ToString();
        tmptxtMileageNum.text = tmptxt_ChassisMileage.text = y.ToString();
    }
}
