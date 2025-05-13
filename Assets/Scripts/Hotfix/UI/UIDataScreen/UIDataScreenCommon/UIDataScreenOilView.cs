using Hotfix.ExcelData;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDataScreenOilView : UIDataScreenCommon
{
    public TMP_Text JinYouYouYa;
    public TMP_Text ChuYouYouYa;
    public Button JinYouCheckBtn;
    public Button ChuYouCheckBtn;
    public Image JinYouSliderI;
    public Image JinYouSliderII;

    public Image leftOilShowNumBG;
    public Image rightOilShowNumBG;

    public Transform tsInfoItem;

    int _currentID = 100;

    public Sprite _wrongSprite;
    Sprite defaultSprite;

    Vector2 leftRangeValue;
    Vector2 rightRangeValue;

    DataCheckConfig dataCheckConfig;

    /// <summary>
    /// 传感器信息
    /// </summary>
    private List<EquipmentCheckConfig2nd> sensorConfigs;

    float maxOilValue = 200f;

    bool leftSwitch;
    bool rightSwitch;
    List<int> keyValues = new List<int>();

    List<Transform> infoItems = new();

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Awake()
    {
        defaultSprite = leftOilShowNumBG.sprite;
        
        //添加按钮交互组件
        InteractionItemList.Add(JinYouCheckBtn.gameObject.AddComponent<UIDataScreenInteractionItem>());
        InteractionItemList.Add(ChuYouCheckBtn.gameObject.AddComponent<UIDataScreenInteractionItem>());
        
        ApplyItemIndex();
    }

    private void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/Config/", "KeyValueConfig.txt");
        string[] values = SensorsComReadManager.ReadConfigKey(path);

        for (int i = 0; i < values.Length; i++)
        {
            Debug.Log(values[i]);
            keyValues.Add(int.Parse(values[i]));
        }

        JinYouCheckBtn.onClick.AddListener(OnLeftOilClickAction);
        ChuYouCheckBtn.onClick.AddListener(OnRightOilClickAction);
    }

    public override void InitValue(DataCheckConfig config)
    {
        base.InitValue(config);
        
        dataCheckConfig = config;
        if (sensorConfigs == null)
        {
            sensorConfigs = DataCheckTopicManager.Instance.GetDataCheckSensorConfigs(config);
        }

        JinYouYouYa.text = 0.ToString();
        ChuYouYouYa.text = 0.ToString();
        List<int> values = DataCheckTopicManager.Instance.GetConfigItem(config.DefaultID);

        _currentID = 100;

        if (values.Count == 1)
        {
            _currentID = values[0];
        }
        else if (values.Count > 1)
        {
            _currentID = 2;
        }

        leftSwitch = false;
        rightSwitch = false;

        foreach (var item in infoItems)
        {
            Object.Destroy(item.gameObject);
        }

        infoItems.Clear();

        rightOilShowNumBG.sprite = defaultSprite;
        leftOilShowNumBG.sprite = defaultSprite;

        JinYouSliderI.fillAmount = 0;
        JinYouSliderII.fillAmount = 0;
    }
    
    private void Update()
    {
        if (PortControl.Instance.GetKey(keyValues[0]) && !leftSwitch)
        {
            leftSwitch = true;
            JinYouCheckBtn.onClick.Invoke();
        }

        if (PortControl.Instance.GetKey(keyValues[1]) && !rightSwitch)
        {
            rightSwitch = true;
            ChuYouCheckBtn.onClick.Invoke();
        }
    }

    /// <summary>
    /// 点击左油箱检测
    /// </summary>
    void OnLeftOilClickAction()
    {
        Transform infoItem = Object.Instantiate(tsInfoItem, tsInfoItem.parent);
        DataScreenInfoItem dataScreen = infoItem.gameObject.AddComponent<DataScreenInfoItem>();
        //Debug.Log(_currentID);
        if (_currentID == 0 || _currentID == 2)
        {
            leftRangeValue = new Vector2(.1f, 1);
            leftOilShowNumBG.sprite = _wrongSprite;
            dataScreen.InitData("错误", "左侧油箱油量过低");
        }
        else
        {
            leftRangeValue = new Vector2(sensorConfigs[0].TargetValues[0], sensorConfigs[0].TargetValues[1]);
            leftOilShowNumBG.sprite = defaultSprite;
            dataScreen.InitData("正常", "左侧油箱油量正常");
        }

        float value = UnityEngine.Random.Range(leftRangeValue.x, leftRangeValue.y);
        JinYouYouYa.text = value.ToString("f1");
        JinYouSliderI.fillAmount = value / maxOilValue;

        infoItems.Add(infoItem);
    }

    /// <summary>
    /// 点击右油箱检测
    /// </summary>
    void OnRightOilClickAction()
    {
        Transform infoItem = Object.Instantiate(tsInfoItem, tsInfoItem.parent);
        DataScreenInfoItem dataScreen = infoItem.gameObject.AddComponent<DataScreenInfoItem>();
        //Debug.Log(_currentID);
        if (_currentID == 1 || _currentID == 2)
        {
            rightRangeValue = new Vector2(.1f, 1);
            rightOilShowNumBG.sprite = _wrongSprite;
            dataScreen.InitData("错误", "右侧油箱油量过低");
        }
        else
        {
            rightRangeValue = new Vector2(sensorConfigs[0].TargetValues[0], sensorConfigs[0].TargetValues[1]);
            rightOilShowNumBG.sprite = defaultSprite;
            dataScreen.InitData("正常", "右侧油箱油量正常");
        }

        float value = UnityEngine.Random.Range(rightRangeValue.x, rightRangeValue.y);
        ChuYouYouYa.text = value.ToString("f0");
        JinYouSliderII.fillAmount = value / maxOilValue;

        infoItems.Add(infoItem);
    }
}