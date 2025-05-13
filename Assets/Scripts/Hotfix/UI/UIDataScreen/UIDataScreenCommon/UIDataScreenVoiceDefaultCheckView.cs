using System.Collections.Generic;
using System.Linq;
using Hotfix.ExcelData;
using UnityEngine;

public class UIDataScreenVoiceDefaultCheckView : UIDataScreenCommon
{
    private Transform tsVoiceDefaultInfoItem;
    
    public List<Sprite> _voiceImages;
    public List<Sprite> _voiceDefaultImages;

    List<Transform> voiceInfoItems = new();

    Dictionary<int, List<string>> tmpVoiceInfoItems = new Dictionary<int, List<string>> { { 0, new List<string> { "正常", "车辆启动" } }, { 1, new List<string> { "正常", "请进行数据检测" } } };
    List<VoiceDataItem> voiceDataItems;
    /// <summary>
    /// 存放正确的数量
    /// </summary>
    List<int> availableNumbers = new();

    /// <summary>
    /// 存放错误的数量
    /// </summary>
    List<int> defaultNumbers = new();

    protected override void OnEnable()
    {
        base.OnEnable();
        InitVoiceDataitem();
    }

    void Awake()
    {
        tsVoiceDefaultInfoItem = transform.Find("OldData_Bg/VoiceDefaultDataInfoItems/Ts_VoiceDefaultInfoItem").GetComponent<Transform>();
        
        _voiceImages = Resources.LoadAll<Sprite>("Image/RightVoiceImage").ToList();
        _voiceDefaultImages = Resources.LoadAll<Sprite>("Image/DefaultVoiceImage").ToList();
        
        voiceDataItems = transform.GetComponentsInChildren<VoiceDataItem>().ToList();
        
        InitializePool();
        
        //添加按钮交互组件
        for (int i = 0; i < voiceDataItems.Count; i++)
        {
            var btn = voiceDataItems[i].transform.Find("Button");
            InteractionItemList.Add(btn.gameObject.AddComponent<UIDataScreenInteractionItem>());
        }
        
        ApplyItemIndex();
    }

    /// <summary>
    /// 初始化声音界面数据
    /// </summary>
    void InitVoiceDataitem()
    {
        foreach (var item in voiceInfoItems)
        {
            Object.Destroy(item.gameObject);
        }

        voiceInfoItems.Clear();

        for (int i = 0; i < tmpVoiceInfoItems.Count; i++)
        {
            Transform infoItem = Object.Instantiate(tsVoiceDefaultInfoItem, tsVoiceDefaultInfoItem.parent);
            DataScreenInfoItem dataScreen = infoItem.gameObject.AddComponent<DataScreenInfoItem>();
            dataScreen.InitData(tmpVoiceInfoItems[i][0], tmpVoiceInfoItems[i][1]);
            voiceInfoItems.Add(infoItem);
        }
    }

    #region 声音处理页面(存在多项检测数据的处理，TODO：需要进行标准化处理)

    /// <summary>
    /// 初始化声音模块处理
    /// </summary>
   public override void InitValue(DataCheckConfig config)
    {
        base.InitValue(config);
        
        InitializePool();
        
        List<int> values = DataCheckTopicManager.Instance.GetConfigItem(config.DefaultID);

        int num = 0;
        for (int i = 0; i < voiceDataItems.Count; i++)
        {
            if (values.Count > 0)
            {
                for (int j = 0; j < values.Count; j++)
                {
                    if (i == values[j])
                    {
                        //处理错误数据
                        num = FakeDataProcess.GetUniqueRandomNumber(defaultNumbers);
                        voiceDataItems[i].Init(i, _voiceDefaultImages[num], true);
                        continue;
                    }
                    else
                    {
                        num = FakeDataProcess.GetUniqueRandomNumber(availableNumbers);
                        voiceDataItems[i].Init(i, _voiceImages[num]);
                    }
                }
            }
            else
            {
                num = FakeDataProcess.GetUniqueRandomNumber(availableNumbers);
                voiceDataItems[i].Init(i, _voiceImages[num]);
            }
        }
    }

    #endregion

    /// <summary>
    /// 初始化或重置数字池
    /// </summary>
    public void InitializePool()
    {
        availableNumbers.Clear();
        defaultNumbers.Clear();
        for (int i = 0; i < _voiceImages.Count; i++)
        {
            availableNumbers.Add(i);
        }

        for (int i = 0; i < _voiceDefaultImages.Count; i++)
        {
            defaultNumbers.Add(i);
        }
    }
}