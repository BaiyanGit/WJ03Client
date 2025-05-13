using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Hotfix.ExcelData;

/// <summary>
/// 第二部分
/// </summary>
public class UIDataScreenSecondPartView : UIDataScreenCommon
{
    /// <summary>
    /// 气压I
    /// </summary>
    public Button _btnEnginePressureFirst;

    public TextMeshProUGUI _tmptxtEnginePressureFirst;
    public float _enginePressureFirstValue;

    /// <summary>
    /// 气压II
    /// </summary>
    public Button _btnEnginePressureSecond;

    public TextMeshProUGUI _tmptxtEnginePressureSecond;
    public float _enginePressureSecondValue;

    /// <summary>
    /// 传动箱压力
    /// </summary>
    public Button _btnTransmissionPressure;

    public TextMeshProUGUI _tmptxtTransmissionPressure;
    public float _transmissionPressureValue;

    /// <summary>
    /// 机油油压
    /// </summary>
    public Button _btnOilPressure;

    public TextMeshProUGUI _tmptxtOilPressure;
    public float _oilPressureValue;

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
        Ctrl_MessageCenter.AddMsgListener<float>("气压", SetEnginePressureFirstValue);
        Ctrl_MessageCenter.AddMsgListener<float>("气压II", SetEnginePressureSecondValue);
        Ctrl_MessageCenter.AddMsgListener<float>("压力", SetTransmissionPressureValue);
        Ctrl_MessageCenter.AddMsgListener<float>("油压", SetOilPressureValue);

        // 将所有相关按钮存入集合统一管理
        controlButtons = new List<Button>
        {
            _btnEnginePressureFirst,
            _btnEnginePressureSecond,
            _btnTransmissionPressure,
            _btnOilPressure
        };

        // 使用字典映射配置ID与对应按钮列表（支持多按钮）
        buttonConfigMapping = new Dictionary<int, List<Button>>
        {
            { 7, new List<Button> { _btnEnginePressureFirst, _btnEnginePressureSecond } }, 
            { 10, new List<Button> { _btnOilPressure } },
            { 11, new List<Button> { _btnTransmissionPressure } } 
        };
        
        //添加按钮交互组件
        InteractionItemList.Add(_btnOilPressure.gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(_btnTransmissionPressure.gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(_btnEnginePressureFirst.gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(_btnEnginePressureSecond.gameObject.AddComponent<UIDataScreenInteractionItem>());
        
        ApplyItemIndex();
    }

    private void Start()
    {
        _btnEnginePressureFirst.onClick.AddListener(OnEnginePressureFirstClickAction);
        _btnEnginePressureSecond.onClick.AddListener(OnEnginePressureSecondClickAction);
        _btnTransmissionPressure.onClick.AddListener(OnTransmissionPressureClickAction);
        _btnOilPressure.onClick.AddListener(OnOilPressureClickAction);
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

        // 根据配置ID获取目标按钮列表
        List<Button> targetButtons;
        var isTargetButton = buttonConfigMapping.TryGetValue(config.Id, out targetButtons);

        // 启用所有目标按钮
        targetButtons.ForEach(btn => btn.interactable = true);

        config2nds = DataCheckTopicManager.Instance.GetDataCheckSensorConfigs(config);
    }

    private void OnEnginePressureFirstClickAction()
    {
        bool _isWrong = false;
        if (_currentID == 0 || _currentID == 2) _isWrong = true;

        _tmptxtEnginePressureFirst.text = isFake && _isWrong ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _enginePressureFirstValue.ToString();

        _tmptxtEnginePressureFirst.transform.parent.GetComponent<Image>().color = _isWrong ? Color.red : Color.white;
    }

    private void OnEnginePressureSecondClickAction()
    {
        bool _isWrong = false;
        if (_currentID == 1 || _currentID == 2) _isWrong = true;
        _tmptxtEnginePressureSecond.text = isFake && _isWrong ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _enginePressureSecondValue.ToString();
        _tmptxtEnginePressureSecond.transform.parent.GetComponent<Image>().color = _isWrong ? Color.red : Color.white;
    }

    private void OnTransmissionPressureClickAction()
    {
        _tmptxtTransmissionPressure.text = isFake ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _transmissionPressureValue.ToString();
    }

    private void OnOilPressureClickAction()
    {
        _tmptxtOilPressure.text = isFake ? CreateFakeValue(config2nds[0].TargetValues).ToString("f1") : _oilPressureValue.ToString();
    }

    /// <summary>
    /// 设置气压I
    /// </summary>
    /// <param name="obj"></param>
    private void SetEnginePressureFirstValue(float obj)
    {
        _enginePressureFirstValue = obj;
    }

    /// <summary>
    /// 设置气压II
    /// </summary>
    /// <param name="obj"></param>
    private void SetEnginePressureSecondValue(float obj)
    {
        _enginePressureSecondValue = obj;
    }

    /// <summary>
    /// 设置传动箱压力
    /// </summary>
    /// <param name="obj"></param>
    private void SetTransmissionPressureValue(float obj)
    {
        _transmissionPressureValue = obj;
    }

    /// <summary>
    /// 设置油压
    /// </summary>
    /// <param name="obj"></param>
    private void SetOilPressureValue(float obj)
    {
        _oilPressureValue = obj;
    }
}