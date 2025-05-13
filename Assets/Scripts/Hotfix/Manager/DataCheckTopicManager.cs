using UnityEngine;
using Hotfix.ExcelData;
using Wx.Runtime.Singleton;
using System.Collections.Generic;
using System.IO;
using Hotfix;
using Hotfix.UI;
using XCharts.Runtime;

/// <summary>
/// 针对数据检测模拟课题临时Manager
/// </summary>
public class DataCheckTopicManager : SingletonInstance<DataCheckTopicManager>, ISingleton
{
    /// <summary>
    /// 是否启用假数据(全局使用假数据)
    /// </summary>
    public bool IsFake = false;

    /// <summary>
    /// 是否界面需要刷新数据
    /// </summary>
    private bool isUINeedRefresh = false;

    /// <summary>
    /// 当前错误的数据
    /// </summary>
    private List<DataCheckConfig> dataCheckDefaultValue = new();

    public List<DataCheckConfig> DataCheckDefaultValue
    {
        get { return dataCheckDefaultValue; }
        set { dataCheckDefaultValue = value; }
    }

    /// <summary>
    /// 预留的错误信息
    /// </summary>
    private List<ConfigItem> configItems = new();

    public List<ConfigItem> ConfigItems
    {
        get { return configItems; }
    }

    /// <summary>
    /// 当前处理的DataConfig
    /// </summary>
    DataCheckConfig _currentDataConfig;

    /// <summary>
    /// 当前处理的config
    /// </summary>
    List<ConfigItem> _currentConfig = new();

    private Dictionary<int, List<EquipmentCheckConfig2nd>> _dataCheckSensorDic = new();

    /// <summary>
    /// 记录所有点的状态
    /// </summary>
    private Dictionary<int, bool> _dataCheckSensorStateDic = new();

    float uploadSensorRecordTime;

    public void OnCreate(object createParam)
    {
        Ctrl_MessageCenter.AddMsgListener<UIDataSceneOpenType>("OnUIDataScreenOpen", OnUIDataScreenOpen);
        Ctrl_MessageCenter.AddMsgListener<UIDataSceneOpenType>("OnUIDataScreenClose", OnUIDataScreenClose);
        Ctrl_MessageCenter.AddMsgListener<DataCheckConfig, bool>("OnDataCheckPointClick", CurrentDataConfig);

        string path = Path.Combine(Application.streamingAssetsPath + "/Config/", "DefaultValueConfig.txt");
        configItems = SensorsComReadManager.ReadConfigCom(path);

        realtimeSinceStartupCacheI = realtimeSinceStartupCacheII = realtimeSinceStartupCacheIII = Time.realtimeSinceStartup;
    }

    public void OnDestroy()
    {
    }

    bool _switch;

    public void OnFixedUpdate()
    {
        if (_currentDataConfig != null)
        {
            if (IsCurrentDataConfigAlive() || Input.GetKeyDown(KeyCode.F5))
            {
                _switch = true;
                RemoveProcessedPoint();
                RemoveProcessConfigItem(_currentConfig[0].Value);
                ShowAllTip();
            }
        }
    }

    private bool IsCurrentDataConfigAlive()
    {
        var temp = GetDataCheckSensorConfigs(_currentDataConfig);

        for (int i = 0; i < temp.Count; i++)
        {
            if (!PortControl.Instance.GetKey(temp[i].SinglechipKey))
            {
                return false;
            }
        }

        return true;
    }

    public void OnLateUpdate()
    {
    }

    public void OnUpdate()
    {
        if (isUINeedRefresh)
        {
            RefreshData();
        }
    }

    #region RefreshData

    #region 记录当前值，进行对比

    private float currentTemperatureValue;
    private float currentOilTemperatureValue;
    private float currentKPAValue;
    private float currentKPAIIValue;
    private float currentAPSValue;
    private float currentOilKPAValue;
    private float currentOilValue;
    public float currentElectricityValue;

    #endregion

    float realtimeSince;

    /// <summary>
    /// 使用设置的正确范围的值、或者直接使用真实数据
    /// </summary>
    void RefreshData()
    {
        Ctrl_MessageCenter.SendMessage("震动频率",
            TemperatureVibrationControl._instance.GetVibrationFrequencyX(),
            TemperatureVibrationControl._instance.GetVibrationFrequencyY(),
            TemperatureVibrationControl._instance.GetVibrationFrequencyZ());

        Ctrl_MessageCenter.SendMessage("温度", TemperatureVibrationControl._instance.GetTemperature());

        float noiseValue = NoiseSensorControl.Instance.GetNoiseValue();
        Ctrl_MessageCenter.SendMessage("噪声", noiseValue);


        UpdateSpeedMileageElectricity();

        UpdateNeedleRotation();

        UpdateFanData();

        if (Time.realtimeSinceStartup - realtimeSince < 2f && IsFake && _dataCheckSensorStateDic.Count < 1) return;

        realtimeSince = Time.realtimeSinceStartup;
        foreach (var item in _dataCheckSensorStateDic)
        {
            List<EquipmentCheckConfig2nd> config2Nds = GetDataCheckSensorConfigs(DataCheckConfigTable.Instance.dataList[item.Key - 1]);
            float[] values = config2Nds[0].TargetValues;
            float value = UnityEngine.Random.Range(values[0], values[1]);

            switch (item.Key)
            {
                case 9:
                    float temperatureValue = IsFake ? value : SinglechipManager.Instance.GetTemperatureValue();
                    temperatureValue = (int)temperatureValue;
                    if (ComparisonValue((int)currentTemperatureValue, (int)temperatureValue))
                    {
                        Ctrl_MessageCenter.SendMessage("缸温", (float)temperatureValue);
                        currentTemperatureValue = temperatureValue;
                    }

                    break;
                case 7:
                    //设置第一个
                    float kPAValue = IsFake ? value : SinglechipManager.Instance.GetKPAValue();
                    if (ComparisonValue((int)currentKPAValue, (int)kPAValue))
                    {
                        Ctrl_MessageCenter.SendMessage("气压", kPAValue);
                        currentKPAValue = kPAValue;
                    }

                    //设置第二个气压值
                    float[] valuesII = config2Nds[1].TargetValues;
                    float valueII = UnityEngine.Random.Range(values[0], values[1]);
                    float kPAIIValue = IsFake ? valueII : SinglechipManager.Instance.GetKPAIIValue();
                    if (ComparisonValue((int)currentKPAIIValue, (int)kPAIIValue))
                    {
                        Ctrl_MessageCenter.SendMessage("气压II", kPAIIValue);
                        currentKPAIIValue = kPAIIValue;
                    }

                    break;
                case 11:
                    float apsValue = IsFake ? value : SinglechipManager.Instance.GetAPSValue();
                    apsValue = (int)apsValue;
                    if (ComparisonValue((int)currentAPSValue, (int)apsValue))
                    {
                        Ctrl_MessageCenter.SendMessage("压力", apsValue);
                        currentAPSValue = apsValue;
                    }

                    break;
                case 10:
                    float oilKPAValue = IsFake ? value : SinglechipManager.Instance.GetOilKPAValue();
                    oilKPAValue = (int)oilKPAValue;
                    if (ComparisonValue((int)currentOilKPAValue, (int)oilKPAValue))
                    {
                        Ctrl_MessageCenter.SendMessage("油压", oilKPAValue);
                        currentOilKPAValue = oilKPAValue;
                    }

                    break;
                case 8:
                    float oilTemperatureValue = IsFake ? value : SinglechipManager.Instance.GetOilTemperatureValue();
                    oilTemperatureValue = (int)oilTemperatureValue;
                    if (ComparisonValue((int)currentOilTemperatureValue, (int)oilTemperatureValue))
                    {
                        Ctrl_MessageCenter.SendMessage("油温", (float)oilTemperatureValue);
                        currentOilTemperatureValue = oilTemperatureValue;
                    }

                    break;
                case 6:
                    float electricityValue = IsFake ? value : SinglechipManager.Instance.GetElectricityValue();
                    if (ComparisonValue((int)currentElectricityValue, (int)electricityValue))
                    {
                        Ctrl_MessageCenter.SendMessage("电压", electricityValue);
                        currentElectricityValue = electricityValue;
                    }

                    break;
                case 4:
                    float oilValue = IsFake ? value : SinglechipManager.Instance.GetOilValue();
                    if (ComparisonValue((int)currentOilValue, (int)oilValue))
                    {
                        Ctrl_MessageCenter.SendMessage("油量", oilValue);
                        currentOilValue = oilValue;
                    }

                    break;
            }

            //每隔10秒发送一次传感器数据
            UploadSensorData(noiseValue);
        }

        // float oilValue = IsFake ? 0f : SinglechipManager.Instance.GetOilValue();
        // if (ComparisonValue(currentOilValue, oilValue))
        // {
        //     Ctrl_MessageCenter.SendMessage("油量", oilValue);
        //     currentOilValue = oilValue;
        // }
    }

    /// <summary>
    /// 更新传感器数据
    /// </summary>
    /// <param name="voiceValue"></param>
    void UploadSensorData(float voiceValue)
    {
        //通过此判断进行传输数据
        if (!IsFake && currentElectricityValue >= 23 && Time.realtimeSinceStartup - uploadSensorRecordTime > 10f)
        {
            var sensorInfo = new SensorPost();
            sensorInfo.TemperatureValue = currentTemperatureValue;
            sensorInfo.KPAValueI = currentKPAValue;
            sensorInfo.KPAValueII = currentKPAIIValue;
            sensorInfo.ApsValue = currentAPSValue;
            sensorInfo.OilKPAValue = currentOilKPAValue;
            sensorInfo.OilTemperatureValue = currentOilTemperatureValue;
            sensorInfo.ElectricityValue = currentElectricityValue;
            sensorInfo.OilValue = currentOilValue;
            sensorInfo.VibrationFrequency = new Vector3(TemperatureVibrationControl._instance.GetVibrationFrequencyX(),
                TemperatureVibrationControl._instance.GetVibrationFrequencyY(),
                TemperatureVibrationControl._instance.GetVibrationFrequencyZ());
            sensorInfo.RPMValue = RPMValue;
            sensorInfo.NoiseValue = voiceValue;
            sensorInfo.CurrentTime = System.DateTime.Now.ToString();
            TopicManager.Instance.UploadSensorData(sensorInfo);
            uploadSensorRecordTime = Time.realtimeSinceStartup;
        }
    }

    /// <summary>
    /// 数据比对，决定是否更新
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool ComparisonValue(int currentValue, int value)
    {
        bool comparisonResoult;
        if (currentValue == value)
        {
            comparisonResoult = false;
            return comparisonResoult;
        }

        currentValue = value;
        comparisonResoult = true;
        return comparisonResoult;
    }

    /// <summary>
    /// 创造假数据
    /// </summary>
    /// <param name="values">正确数据的取值范围</param>
    /// <param name="isValues">是否可以两头取错误值</param>
    /// <returns></returns>
    public float CreateFakeValue(float[] values, bool isValues)
    {
        float tmpValue = 0;

        if (isValues)
        {
        }

        return tmpValue;
    }

    #region 时速-里程-电力

    /// <summary>
    /// 基础里程
    /// </summary>
    float num = 3000f;

    /// <summary>
    /// 缓存上次的改变时间
    /// 速度-里程-电力
    /// </summary>
    private float realtimeSinceStartupCacheI;

    void UpdateSpeedMileageElectricity()
    {
        if (Time.realtimeSinceStartup - realtimeSinceStartupCacheI > 1.3f)
        {
            float chesulicheng = SinglechipManager.Instance.GetSpeedValue();
            Ctrl_MessageCenter.SendMessage("Speed-Mileage-Electricity", Random.Range(60, 65), (int)chesulicheng /*(int)(num += Time.deltaTime)*/, Random.Range(10, 15));
            realtimeSinceStartupCacheI = Time.realtimeSinceStartup;
        }
    }

    #endregion

    #region 设置转速指针

    public float minRPM = 0f; // 最小转速
    public float maxRPM = 35f; // 最大转速
    public float minAngle = 180f; // 最小角度
    public float maxAngle = -131f; // 最大角度

    /// <summary>
    /// 缓存上次的改变时间
    /// 指针旋转
    /// </summary>
    private float realtimeSinceStartupCacheII;

    float RPMValue;

    /// <summary>
    /// 设置指针旋转
    /// </summary>
    /// <param name="rpm"></param>
    public void UpdateNeedleRotation()
    {
        RPMValue = SinglechipManager.Instance.GetRPMValue();
        //模拟当前转速
        float rpm = /*Random.Range(1100, 1250);*/ RPMValue;
        // 将转速映射到角度
        float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(minRPM, maxRPM, rpm));

        //if (Time.realtimeSinceStartup - realtimeSinceStartupCacheII > 1f)
        //{
        //    Ctrl_MessageCenter.SendMessage("指针旋转-rpm", rpm);
        //    realtimeSinceStartupCacheII = Time.realtimeSinceStartup;
        //}
        Ctrl_MessageCenter.SendMessage("指针旋转-rpm", rpm);
        // 设置指针的旋转
        Ctrl_MessageCenter.SendMessage("指针旋转-angle", angle);
    }

    #endregion

    #region 风扇转速

    /// <summary>
    /// 是否风扇数据初始化过
    /// </summary>
    private bool isFanDataInitialized = false;

    /// <summary>
    /// 图片旋转速度
    /// </summary>
    float fanRotateSpeed;

    Vector2 setSpeedValueRange;
    Vector2 cylinderTemperatureRange;

    /// <summary>
    /// 缓存上次的改变时间
    /// 风扇
    /// </summary>
    private float realtimeSinceStartupCacheIII;

    /// <summary>
    /// 风扇转速模块处理(只需要处理是否再正常值内)
    /// </summary>
    /// <param name="config"></param>
    /// <param name="uiModel"></param>
    private void InitProcessFanData()
    {
        var config = DataCheckConfigTable.Instance.Get(2);
        List<int> values = Instance.GetConfigItem(config.DefaultID);

        var sensorConfigs = Instance.GetDataCheckSensorConfigs(config);

        Debug.Log(values.Count);

        if (values.Count > 0)
        {
            setSpeedValueRange = new Vector2(sensorConfigs[0].TargetValues[0] - 30, sensorConfigs[0].TargetValues[0]);
            cylinderTemperatureRange = new Vector2(175, 187);
            fanRotateSpeed = 120f;
        }
        else
        {
            setSpeedValueRange = new Vector2(sensorConfigs[0].TargetValues[0], sensorConfigs[0].TargetValues[1]);
            cylinderTemperatureRange = new Vector2(110, 130);
            fanRotateSpeed = 280f;
        }

        isFanDataInitialized = true;
    }

    /// <summary>
    /// 实时更新风扇数据
    /// </summary>
    void UpdateFanData()
    {
        if (!isFanDataInitialized)
        {
            InitProcessFanData();
        }

        if (Time.realtimeSinceStartup - realtimeSinceStartupCacheIII > 1f)
        {
            Ctrl_MessageCenter.SendMessage("风扇", Random.Range(setSpeedValueRange.x, setSpeedValueRange.y), Random.Range(cylinderTemperatureRange.x, cylinderTemperatureRange.y), fanRotateSpeed);
            realtimeSinceStartupCacheIII = Time.realtimeSinceStartup;
        }
    }

    #endregion

    #endregion

    /// <summary>
    /// 初始化的时候检查所有的点是否错误（前期配置，后期再说）
    /// DataCheckConfig
    /// </summary>
    private void CheckAllPoints()
    {
        dataCheckDefaultValue.Clear();
        _dataCheckSensorStateDic.Clear();
        List<DataCheckConfig> datalist = DataCheckConfigTable.Instance.dataList;

        int num = datalist.Count;

        for (int i = 0; i < num; i++)
        {
            bool tmpState = false;
            for (int j = 0; j < configItems.Count; j++)
            {
                if (datalist[i].Id == configItems[j].Key)
                {
                    dataCheckDefaultValue.Add(datalist[i]);
                    tmpState = true;
                }
            }

            _dataCheckSensorStateDic.Add(datalist[i].Id, tmpState);
        }
    }

    /// <summary>
    /// 设置当前数据
    /// </summary>
    /// <param name="pointConfig"></param>
    private void CurrentDataConfig(DataCheckConfig pointConfig, bool isHand)
    {
        _currentDataConfig = pointConfig;
        GetConfigItem(_currentDataConfig.DefaultID);
    }

    /// <summary>
    /// 将处理过的从列表移除
    /// </summary>
    public void RemoveProcessedPoint()
    {
        for (int i = 0; i < dataCheckDefaultValue.Count; i++)
        {
            if (dataCheckDefaultValue[i] == _currentDataConfig)
            {
                dataCheckDefaultValue.Remove(_currentDataConfig);
            }
        }
    }

    /// <summary>
    /// 通过ID移除配置的错误信息
    /// </summary>
    /// <param name="id"></param>
    public void RemoveProcessConfigItem(int id)
    {
        for (int i = 0; i < configItems.Count; i++)
        {
            if (id == configItems[i].Value)
            {
                configItems.Remove(configItems[i]);
            }
        }
    }

    /// <summary>
    /// 通过对应的ID获取错误数据
    /// </summary>
    public List<int> GetConfigItem(int id)
    {
        _currentConfig.Clear();
        List<int> values = new();

        foreach (var item in configItems)
        {
            if (item.Value == id)
            {
                values.Add(item.DefaultValueID);
                _currentConfig.Add(item);
            }
        }

        return values;
    }

    #region 初始化监测点

    /// <summary>
    /// 用于存储提示点Tip，方便清理
    /// </summary>
    private List<GameObject> tips = new();

    /// <summary>
    /// 检查点预制物
    /// </summary>
    private Transform _pointTip;

    /// <summary>
    /// 分类记录所有的点
    /// </summary>
    private Dictionary<int, List<GameObject>> allPointsDic = new Dictionary<int, List<GameObject>>();

    /// <summary>
    /// 显示所有的错误项(是随机抽取几个显示，还是全部都显示)
    /// </summary>
    public void ShowAllTip()
    {
        ClearAllTip();

        if (!_pointTip)
        {
            _pointTip = Resources.Load<Transform>("PointTip");
        }

        if (Instance.DataCheckDefaultValue.Count == 0)
        {
            CheckAllPoints();
        }

        //决定故障状态的配置
        List<DataCheckConfig> defaultValue = Instance.DataCheckDefaultValue;
        List<DataCheckConfig> datalist = DataCheckConfigTable.Instance.dataList;

        for (int i = 0; i < datalist.Count; i++)
        {
            //默认把Tip放在第一个路径模型位置
            Transform tmpTran = GameManager.Instance.MainTarget.Find(datalist[i].ModelPaths[0]);
            Transform tmpTip = Object.Instantiate(_pointTip, tmpTran);

            Vector3 offsetVector = new Vector3();
            switch (datalist[i].TipPosOffsetAxis)
            {
                case "x":
                    offsetVector = new Vector3((float)(1080 * 0.001), 0, 0);
                    break;
                case "y":
                    offsetVector = new Vector3(0, datalist[i].TipPosOffsetValue, 0);
                    break;
                case "z":
                    offsetVector = new Vector3(0, 0, datalist[i].TipPosOffsetValue);
                    break;
            }

            tmpTip.localPosition = Vector3.zero + offsetVector;
            tmpTip.GetComponentInChildren<TipItemExtend>().Init(null, datalist[i]);

            tmpTip.gameObject.AddComponent<YAxisFaceCamera>();
            tmpTip.GetComponentInChildren<Animator>().Play("Right");
            tmpTip.GetComponentInChildren<Animator>().keepAnimatorStateOnDisable = true;
            tips.Add(tmpTip.gameObject);

            if (!allPointsDic.ContainsKey(datalist[i].Type))
            {
                allPointsDic[datalist[i].Type] = new List<GameObject>();
            }

            allPointsDic[datalist[i].Type].Add(tmpTip.gameObject);

            for (int j = 0; j < defaultValue.Count; j++)
            {
                //有这个Id即认为有故障
                if (datalist[i].Id == defaultValue[j].Id)
                {
                    tmpTip.GetComponentInChildren<Animator>().Play("Wrong");
                }
            }
        }
    }

    private void ClearAllTip()
    {
        if (tips.Count > 0)
        {
            //清理所有的提示点
            // foreach (var item in tips)
            // {
            //     Object.Destroy(item.gameObject);
            // }

            for (int i = tips.Count - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(tips[i]);
            }
        }

        allPointsDic.Clear();
    }

    /// <summary>
    /// 通过选择的Type进行显隐Tip
    /// </summary>
    /// <param name="index"></param>
    public void HideTips(int index)
    {
        Debug.Log(index);
        List<GameObject> tmpTips = new List<GameObject>();

        foreach (var item in allPointsDic)
        {
            item.Value?.ForEach(obj => { obj?.SetActive(true); });

            if (index + 1 != 0 && item.Key != index + 1)
            {
                item.Value.ForEach(tip => tip.SetActive(false));
            }
        }
    }

    private void OnUIDataScreenOpen(UIDataSceneOpenType type)
    {
        isUINeedRefresh = true;
    }

    private void OnUIDataScreenClose(UIDataSceneOpenType type)
    {
        isUINeedRefresh = false;
        if (type == UIDataSceneOpenType.Pure)
        {
            ClearAllTip();
            GameEntry.UI.OpenUIFormSync<UIMainMenu>();
        }
    }

    #endregion

    /// <summary>
    /// 处理更新表数据
    /// </summary>
    /// <param name="lineChart">要处理的表</param>
    /// <param name="dataName">数据名称</param>
    /// <param name="addDataValue">需要添加的数据</param>
    public void UpdateLineData(LineChart lineChart, string dataName, double addDataValue)
    {
        var serieData = lineChart.GetSerie(dataName).data;
        int dataNum = serieData.Count;
        serieData.RemoveAt(0);
        for (int i = 0; i < dataNum - 1; i++)
        {
            serieData[i].data[0] = i;
        }

        lineChart.AddData(dataName, addDataValue);
    }

    /// <summary>
    /// 获取传感器故障点对应的传感器配置
    /// </summary>
    /// <returns></returns>
    public List<EquipmentCheckConfig2nd> GetDataCheckSensorConfigs(DataCheckConfig config)
    {
        //初始化配置缓存
        if (_dataCheckSensorDic.Count == 0)
        {
            var temp = DataCheckConfigTable.Instance.dataList;
            for (int i = 0; i < temp.Count; i++)
            {
                var sensorConfigs = new System.Collections.Generic.List<EquipmentCheckConfig2nd>();
                for (int j = 0; j < temp[i].TargetSensors.Length; j++)
                {
                    sensorConfigs.Add(EquipmentCheckConfig2ndTable.Instance.Get(temp[i].TargetSensors[j]));
                }

                _dataCheckSensorDic.Add(temp[i].Id, sensorConfigs);
            }
        }

        return _dataCheckSensorDic[config.Id];
    }
}

/// <summary>
/// 错误点配置设置
/// </summary>
public class ConfigItem
{
    /// <summary>
    /// ID
    /// </summary>
    public int Key { get; set; }

    /// <summary>
    /// 错误ID
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// 对应的错误序列
    /// </summary>
    public int DefaultValueID { get; set; }
}