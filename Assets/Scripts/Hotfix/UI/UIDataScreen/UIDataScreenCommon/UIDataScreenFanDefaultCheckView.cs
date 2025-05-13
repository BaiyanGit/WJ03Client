using System.Collections.Generic;
using Hotfix.ExcelData;
using Hotfix.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 风扇转速检测（FanDefaultCheckView）
/// </summary>
public class UIDataScreenFanDefaultCheckView : UIDataScreenCommon
{
    public Transform tsFanBladeSpeed;
    public TextMeshProUGUI tmptxtFanSpeedValue;

    public Image imgCylinderTemperature;
    public TextMeshProUGUI tmptxtCylinderTemperature;

    public TextMeshProUGUI tmptxtTransmissionShaftValue;

    /// <summary>
    /// 传感器信息
    /// </summary>
    private List<EquipmentCheckConfig2nd> sensorConfigs;

    void Awake()
    {
        tsFanBladeSpeed = transform.Find("FanSpeed/FanSpeed_Icon/Ts_FanBladeSpeed").GetComponent<Transform>();
        tmptxtFanSpeedValue = transform.Find("FanSpeed/FanSpeedValue_Bg/TmpTxt_FanSpeedValue").GetComponent<TextMeshProUGUI>();

        imgCylinderTemperature = transform.Find("CylinderTemperature/CylinderTemperature_Icon/Img_CylinderTemperature").GetComponent<Image>();
        tmptxtCylinderTemperature = transform.Find("CylinderTemperature/CylinderTemperature_Bg/TmpTxt_CylinderTemperatureValue").GetComponent<TextMeshProUGUI>();

        tmptxtTransmissionShaftValue = transform.Find("TransmissionShaft/TransmissionShaft_Bg/TmpTxt_TransmissionShaftValue").GetComponent<TextMeshProUGUI>();

        Ctrl_MessageCenter.AddMsgListener<float>("缸温", OnGangWenChange);
        Ctrl_MessageCenter.AddMsgListener<float>("指针旋转-rpm", OnUpdateNeedleRotationRPM);
        Ctrl_MessageCenter.AddMsgListener<float, float, float>("风扇", OnFenShanChange);
        
        //添加按钮交互组件
        InteractionItemList.Add(tsFanBladeSpeed.gameObject.AddComponent<UIDataScreenInteractionItem>());
        
        ApplyItemIndex();
    }

    private void OnGangWenChange(float value)
    {
        tmptxtCylinderTemperature.text = value.ToString();
        imgCylinderTemperature.fillAmount = value / 70;
    }

    private void OnUpdateNeedleRotationRPM(float rpm)
    {
        tmptxtTransmissionShaftValue.text = ((int)(rpm / 100)).ToString();
    }

    private void OnFenShanChange(float x, float y, float z)
    {
        tmptxtFanSpeedValue.text = x.ToString("f0");
        tmptxtCylinderTemperature.text = y.ToString("f0");
        //tsFanBladeSpeed.Rotate(Vector3.forward,Time.deltaTime * z, Space.Self);
    }

    public override void InitValue(DataCheckConfig config)
    {
        base.InitValue(config);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
}