using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Hotfix.ExcelData;

/// <summary>
/// �ڶ�����
/// </summary>
public class UIDataScreenSecondPartView : UIDataScreenCommon
{
    /// <summary>
    /// ��ѹI
    /// </summary>
    public Button _btnEnginePressureFirst;

    public TextMeshProUGUI _tmptxtEnginePressureFirst;
    public float _enginePressureFirstValue;

    /// <summary>
    /// ��ѹII
    /// </summary>
    public Button _btnEnginePressureSecond;

    public TextMeshProUGUI _tmptxtEnginePressureSecond;
    public float _enginePressureSecondValue;

    /// <summary>
    /// ������ѹ��
    /// </summary>
    public Button _btnTransmissionPressure;

    public TextMeshProUGUI _tmptxtTransmissionPressure;
    public float _transmissionPressureValue;

    /// <summary>
    /// ������ѹ
    /// </summary>
    public Button _btnOilPressure;

    public TextMeshProUGUI _tmptxtOilPressure;
    public float _oilPressureValue;

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
        Ctrl_MessageCenter.AddMsgListener<float>("��ѹ", SetEnginePressureFirstValue);
        Ctrl_MessageCenter.AddMsgListener<float>("��ѹII", SetEnginePressureSecondValue);
        Ctrl_MessageCenter.AddMsgListener<float>("ѹ��", SetTransmissionPressureValue);
        Ctrl_MessageCenter.AddMsgListener<float>("��ѹ", SetOilPressureValue);

        // ��������ذ�ť���뼯��ͳһ����
        controlButtons = new List<Button>
        {
            _btnEnginePressureFirst,
            _btnEnginePressureSecond,
            _btnTransmissionPressure,
            _btnOilPressure
        };

        // ʹ���ֵ�ӳ������ID���Ӧ��ť�б�֧�ֶఴť��
        buttonConfigMapping = new Dictionary<int, List<Button>>
        {
            { 7, new List<Button> { _btnEnginePressureFirst, _btnEnginePressureSecond } }, 
            { 10, new List<Button> { _btnOilPressure } },
            { 11, new List<Button> { _btnTransmissionPressure } } 
        };
        
        //��Ӱ�ť�������
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

        // �����������а�ť
        controlButtons.ForEach(btn => btn.interactable = false);

        // ��������ID��ȡĿ�갴ť�б�
        List<Button> targetButtons;
        var isTargetButton = buttonConfigMapping.TryGetValue(config.Id, out targetButtons);

        // ��������Ŀ�갴ť
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
    /// ������ѹI
    /// </summary>
    /// <param name="obj"></param>
    private void SetEnginePressureFirstValue(float obj)
    {
        _enginePressureFirstValue = obj;
    }

    /// <summary>
    /// ������ѹII
    /// </summary>
    /// <param name="obj"></param>
    private void SetEnginePressureSecondValue(float obj)
    {
        _enginePressureSecondValue = obj;
    }

    /// <summary>
    /// ���ô�����ѹ��
    /// </summary>
    /// <param name="obj"></param>
    private void SetTransmissionPressureValue(float obj)
    {
        _transmissionPressureValue = obj;
    }

    /// <summary>
    /// ������ѹ
    /// </summary>
    /// <param name="obj"></param>
    private void SetOilPressureValue(float obj)
    {
        _oilPressureValue = obj;
    }
}