using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hotfix.ExcelData;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 第一部分
/// </summary>
public class UIDataScreenFirstPartView : UIDataScreenCommon
{
    #region 

    /// <summary>
    /// 发动机转速
    /// </summary>
    public Button _btnEngineSpeed;
    public TextMeshProUGUI _tmptxtEngineSpeed;
    public float _engineSpeedValue;
    /// <summary>
    /// 电压
    /// </summary>
    public Button _btnVoltage;
    public TextMeshProUGUI _tmptxtVoltage;
    public float _voltageValue;
    /// <summary>
    /// 油温
    /// </summary>
    public Button _btnOilTemperature;
    public TextMeshProUGUI _tmptxtOilTemperature;
    public float _oilTmperatureValue;
    /// <summary>
    /// 缸温
    /// </summary>
    public Button _btnCylinderTemperature;
    public TextMeshProUGUI _tmptxtCylinderTemperature;
    public float _cylinderTmperatureValue;

    #endregion

    /// <summary>
    /// 设置第几位错误
    /// </summary>
    int _currentID = 100;

    /// <summary>
    /// 用于处理Btn是否启用
    /// </summary>
    List<Button> controlButtons;
    Dictionary<int, List<Button>> buttonConfigMapping;

    /// <summary>
    /// 是否为假数据
    /// </summary>
    bool isFake;

    List<EquipmentCheckConfig2nd> config2nds;
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Awake()
    {
        Ctrl_MessageCenter.AddMsgListener<float>("指针旋转-rpm", SetRpmValue);
        Ctrl_MessageCenter.AddMsgListener<float>("电压", SetVoltageValue);
        Ctrl_MessageCenter.AddMsgListener<float>("油温", SetOilTemperatureValue);
        Ctrl_MessageCenter.AddMsgListener<float>("缸温", SetCylinderTemperatureValue);

        // 将所有相关按钮存入集合统一管理
        controlButtons = new List<Button>
        {
            _btnEngineSpeed,
            _btnVoltage,
            _btnOilTemperature,
            _btnCylinderTemperature
        };

        // 批量禁用所有按钮
        controlButtons.ForEach(btn => btn.interactable = false);

        // 使用字典映射配置ID与对应按钮列表（支持多按钮）
        buttonConfigMapping = new Dictionary<int, List<Button>>
        {
            {6, new List<Button> { _btnVoltage }}, // ID=5 时启用电压 + 引擎转速
            {8, new List<Button> { _btnOilTemperature }},           // ID=7 时仅启用油温
            {9, new List<Button> { _btnCylinderTemperature }} // ID=8 时启用缸温 + 油温
        };
        
        //添加按钮交互组件
        InteractionItemList.Add(_btnEngineSpeed.gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(_btnVoltage.gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(_btnCylinderTemperature.gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(_btnOilTemperature.gameObject.AddComponent<UIDataScreenInteractionItem>());

        ApplyItemIndex();
    }

    private void Start()
    {
        _btnEngineSpeed.onClick.AddListener(OnEngineSpeedClickAction);
        _btnVoltage.onClick.AddListener(OnVoltageClickAction);
        _btnOilTemperature.onClick.AddListener(OnOilTemperatureClickAction);
        _btnCylinderTemperature.onClick.AddListener(OnCylinderTemperatureClickAction);
    }

    public override void InitValue(DataCheckConfig config)
    {
        base.InitValue(config);

        List<int> values = DataCheckTopicManager.Instance.GetConfigItem(config.DefaultID);
        _currentID = 100;
        if (values.Count != 0)
        {
            _currentID = values[0];

            isFake = true;
        }

        // 批量禁用所有按钮
        controlButtons.ForEach(btn => btn.interactable = false);
        // 根据配置ID获取目标按钮列表，若未匹配则默认启用引擎转速
        var targetButtons = buttonConfigMapping.TryGetValue(config.Id, out var buttons)? buttons : new List<Button> { _btnEngineSpeed };

        // 启用目标按钮
        targetButtons.ForEach(btn => btn.interactable = true);

        config2nds = DataCheckTopicManager.Instance.GetDataCheckSensorConfigs(config);
    }

    private void OnEngineSpeedClickAction()
    {
        _tmptxtEngineSpeed.text = isFake ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _engineSpeedValue.ToString("f0");
    }

    private void OnVoltageClickAction()
    {
        _tmptxtVoltage.text =  isFake ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _voltageValue.ToString("f0");
    }

    private void OnOilTemperatureClickAction()
    {
        _tmptxtOilTemperature.text = isFake ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _oilTmperatureValue.ToString("f0");
    }

    private void OnCylinderTemperatureClickAction()
    {
        _tmptxtCylinderTemperature.text = isFake ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _cylinderTmperatureValue.ToString("f0");
    }

    /// <summary>
    /// 设置转速
    /// </summary>
    /// <param name="obj"></param>
    private void SetRpmValue(float obj)
    {
        _engineSpeedValue = obj;
    }

    private void SetVoltageValue(float obj)
    {
        _voltageValue = obj;
    }

    /// <summary>
    /// 设置油温
    /// </summary>
    /// <param name="obj"></param>
    private void SetOilTemperatureValue(float obj)
    {
        _oilTmperatureValue = obj;
    }

    /// <summary>
    /// 设置缸温
    /// </summary>
    /// <param name="obj"></param>
    private void SetCylinderTemperatureValue(float obj)
    {
        _cylinderTmperatureValue = obj;
    }
}
