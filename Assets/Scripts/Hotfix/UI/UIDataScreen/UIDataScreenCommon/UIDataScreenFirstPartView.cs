using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hotfix.ExcelData;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ��һ����
/// </summary>
public class UIDataScreenFirstPartView : UIDataScreenCommon
{
    #region 

    /// <summary>
    /// ������ת��
    /// </summary>
    public Button _btnEngineSpeed;
    public TextMeshProUGUI _tmptxtEngineSpeed;
    public float _engineSpeedValue;
    /// <summary>
    /// ��ѹ
    /// </summary>
    public Button _btnVoltage;
    public TextMeshProUGUI _tmptxtVoltage;
    public float _voltageValue;
    /// <summary>
    /// ����
    /// </summary>
    public Button _btnOilTemperature;
    public TextMeshProUGUI _tmptxtOilTemperature;
    public float _oilTmperatureValue;
    /// <summary>
    /// ����
    /// </summary>
    public Button _btnCylinderTemperature;
    public TextMeshProUGUI _tmptxtCylinderTemperature;
    public float _cylinderTmperatureValue;

    #endregion

    /// <summary>
    /// ���õڼ�λ����
    /// </summary>
    int _currentID = 100;

    /// <summary>
    /// ���ڴ���Btn�Ƿ�����
    /// </summary>
    List<Button> controlButtons;
    Dictionary<int, List<Button>> buttonConfigMapping;

    /// <summary>
    /// �Ƿ�Ϊ������
    /// </summary>
    bool isFake;

    List<EquipmentCheckConfig2nd> config2nds;
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Awake()
    {
        Ctrl_MessageCenter.AddMsgListener<float>("ָ����ת-rpm", SetRpmValue);
        Ctrl_MessageCenter.AddMsgListener<float>("��ѹ", SetVoltageValue);
        Ctrl_MessageCenter.AddMsgListener<float>("����", SetOilTemperatureValue);
        Ctrl_MessageCenter.AddMsgListener<float>("����", SetCylinderTemperatureValue);

        // ��������ذ�ť���뼯��ͳһ����
        controlButtons = new List<Button>
        {
            _btnEngineSpeed,
            _btnVoltage,
            _btnOilTemperature,
            _btnCylinderTemperature
        };

        // �����������а�ť
        controlButtons.ForEach(btn => btn.interactable = false);

        // ʹ���ֵ�ӳ������ID���Ӧ��ť�б�֧�ֶఴť��
        buttonConfigMapping = new Dictionary<int, List<Button>>
        {
            {6, new List<Button> { _btnVoltage }}, // ID=5 ʱ���õ�ѹ + ����ת��
            {8, new List<Button> { _btnOilTemperature }},           // ID=7 ʱ����������
            {9, new List<Button> { _btnCylinderTemperature }} // ID=8 ʱ���ø��� + ����
        };
        
        //��Ӱ�ť�������
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

        // �����������а�ť
        controlButtons.ForEach(btn => btn.interactable = false);
        // ��������ID��ȡĿ�갴ť�б���δƥ����Ĭ����������ת��
        var targetButtons = buttonConfigMapping.TryGetValue(config.Id, out var buttons)? buttons : new List<Button> { _btnEngineSpeed };

        // ����Ŀ�갴ť
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
    /// ����ת��
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
    /// ��������
    /// </summary>
    /// <param name="obj"></param>
    private void SetOilTemperatureValue(float obj)
    {
        _oilTmperatureValue = obj;
    }

    /// <summary>
    /// ���ø���
    /// </summary>
    /// <param name="obj"></param>
    private void SetCylinderTemperatureValue(float obj)
    {
        _cylinderTmperatureValue = obj;
    }
}
