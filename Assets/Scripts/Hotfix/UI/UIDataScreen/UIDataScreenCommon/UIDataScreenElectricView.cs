using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class UIDataScreenElectricView : MonoBehaviour
{
    public TextMeshProUGUI tmptxtKPANum;
    public TextMeshProUGUI tmptxtElectricityNum;
    public Image imgElectricityImageValue;
    public TextMeshProUGUI tmptxt_ElectricityValue;
    public TextMeshProUGUI tmptxtVoltageValue;
    public TextMeshProUGUI tmptxtVoltageNum;
    
    public LineChart _electricKPALineChart;
    void Awake()
    {
        tmptxtKPANum = transform.Find("KPA_Bg/KPAShowNum/TmpTxt_KPANum").GetComponent<TextMeshProUGUI>();
        tmptxtElectricityNum = transform.Find("Electricity_Bg/ElectricityShowNum/TmpTxt_ElectricityNum").GetComponent<TextMeshProUGUI>();
        imgElectricityImageValue = transform.Find("Electricity_Bg/Electricity/Img_ElectricityImageValue").GetComponent<Image>();
        tmptxt_ElectricityValue = transform.Find("Electricity_Bg/Electricity/TmpTxt__ElectricityValue").GetComponent<TextMeshProUGUI>();
        tmptxtVoltageValue = transform.Find("Voltage_Bg/Voltage/TmpTxt_VoltageValue").GetComponent<TextMeshProUGUI>();
        tmptxtVoltageNum = transform.Find("Voltage_Bg/VoltageShowNum/TmpTxt_VoltageNum").GetComponent<TextMeshProUGUI>();
        
        _electricKPALineChart = transform.Find("KPA_Bg/KPA").GetComponent<LineChart>();

        Ctrl_MessageCenter.AddMsgListener<float>("气压", OnQiYaChange);
        Ctrl_MessageCenter.AddMsgListener<int, int, int>("Speed-Mileage-Electricity", OnZhenDongPinLvChange);
        Ctrl_MessageCenter.AddMsgListener<float>("电压", OnVoltageChange);
    }

    private void OnVoltageChange(float obj)
    {
        tmptxtVoltageNum.text = obj.ToString("f0");
        tmptxtVoltageValue.text = obj.ToString("f0");
    }

    private void OnQiYaChange(float value)
    {
        DataCheckTopicManager.Instance.UpdateLineData(_electricKPALineChart, "气压", value);
        tmptxtKPANum.text = value.ToString();
    }
    private void OnZhenDongPinLvChange(int x, int y, int z)
    {
        tmptxtElectricityNum.text = tmptxt_ElectricityValue.text = z.ToString();
    }
}