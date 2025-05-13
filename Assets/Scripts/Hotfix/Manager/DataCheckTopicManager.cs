using UnityEngine;
using Hotfix.ExcelData;
using Wx.Runtime.Singleton;
using System.Collections.Generic;
using System.IO;
using Hotfix;
using Hotfix.UI;
using XCharts.Runtime;

/// <summary>
/// ������ݼ��ģ�������ʱManager
/// </summary>
public class DataCheckTopicManager : SingletonInstance<DataCheckTopicManager>, ISingleton
{
    /// <summary>
    /// �Ƿ����ü�����(ȫ��ʹ�ü�����)
    /// </summary>
    public bool IsFake = false;

    /// <summary>
    /// �Ƿ������Ҫˢ������
    /// </summary>
    private bool isUINeedRefresh = false;

    /// <summary>
    /// ��ǰ���������
    /// </summary>
    private List<DataCheckConfig> dataCheckDefaultValue = new();

    public List<DataCheckConfig> DataCheckDefaultValue
    {
        get { return dataCheckDefaultValue; }
        set { dataCheckDefaultValue = value; }
    }

    /// <summary>
    /// Ԥ���Ĵ�����Ϣ
    /// </summary>
    private List<ConfigItem> configItems = new();

    public List<ConfigItem> ConfigItems
    {
        get { return configItems; }
    }

    /// <summary>
    /// ��ǰ�����DataConfig
    /// </summary>
    DataCheckConfig _currentDataConfig;

    /// <summary>
    /// ��ǰ�����config
    /// </summary>
    List<ConfigItem> _currentConfig = new();

    private Dictionary<int, List<EquipmentCheckConfig2nd>> _dataCheckSensorDic = new();

    /// <summary>
    /// ��¼���е��״̬
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

    #region ��¼��ǰֵ�����жԱ�

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
    /// ʹ�����õ���ȷ��Χ��ֵ������ֱ��ʹ����ʵ����
    /// </summary>
    void RefreshData()
    {
        Ctrl_MessageCenter.SendMessage("��Ƶ��",
            TemperatureVibrationControl._instance.GetVibrationFrequencyX(),
            TemperatureVibrationControl._instance.GetVibrationFrequencyY(),
            TemperatureVibrationControl._instance.GetVibrationFrequencyZ());

        Ctrl_MessageCenter.SendMessage("�¶�", TemperatureVibrationControl._instance.GetTemperature());

        float noiseValue = NoiseSensorControl.Instance.GetNoiseValue();
        Ctrl_MessageCenter.SendMessage("����", noiseValue);


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
                        Ctrl_MessageCenter.SendMessage("����", (float)temperatureValue);
                        currentTemperatureValue = temperatureValue;
                    }

                    break;
                case 7:
                    //���õ�һ��
                    float kPAValue = IsFake ? value : SinglechipManager.Instance.GetKPAValue();
                    if (ComparisonValue((int)currentKPAValue, (int)kPAValue))
                    {
                        Ctrl_MessageCenter.SendMessage("��ѹ", kPAValue);
                        currentKPAValue = kPAValue;
                    }

                    //���õڶ�����ѹֵ
                    float[] valuesII = config2Nds[1].TargetValues;
                    float valueII = UnityEngine.Random.Range(values[0], values[1]);
                    float kPAIIValue = IsFake ? valueII : SinglechipManager.Instance.GetKPAIIValue();
                    if (ComparisonValue((int)currentKPAIIValue, (int)kPAIIValue))
                    {
                        Ctrl_MessageCenter.SendMessage("��ѹII", kPAIIValue);
                        currentKPAIIValue = kPAIIValue;
                    }

                    break;
                case 11:
                    float apsValue = IsFake ? value : SinglechipManager.Instance.GetAPSValue();
                    apsValue = (int)apsValue;
                    if (ComparisonValue((int)currentAPSValue, (int)apsValue))
                    {
                        Ctrl_MessageCenter.SendMessage("ѹ��", apsValue);
                        currentAPSValue = apsValue;
                    }

                    break;
                case 10:
                    float oilKPAValue = IsFake ? value : SinglechipManager.Instance.GetOilKPAValue();
                    oilKPAValue = (int)oilKPAValue;
                    if (ComparisonValue((int)currentOilKPAValue, (int)oilKPAValue))
                    {
                        Ctrl_MessageCenter.SendMessage("��ѹ", oilKPAValue);
                        currentOilKPAValue = oilKPAValue;
                    }

                    break;
                case 8:
                    float oilTemperatureValue = IsFake ? value : SinglechipManager.Instance.GetOilTemperatureValue();
                    oilTemperatureValue = (int)oilTemperatureValue;
                    if (ComparisonValue((int)currentOilTemperatureValue, (int)oilTemperatureValue))
                    {
                        Ctrl_MessageCenter.SendMessage("����", (float)oilTemperatureValue);
                        currentOilTemperatureValue = oilTemperatureValue;
                    }

                    break;
                case 6:
                    float electricityValue = IsFake ? value : SinglechipManager.Instance.GetElectricityValue();
                    if (ComparisonValue((int)currentElectricityValue, (int)electricityValue))
                    {
                        Ctrl_MessageCenter.SendMessage("��ѹ", electricityValue);
                        currentElectricityValue = electricityValue;
                    }

                    break;
                case 4:
                    float oilValue = IsFake ? value : SinglechipManager.Instance.GetOilValue();
                    if (ComparisonValue((int)currentOilValue, (int)oilValue))
                    {
                        Ctrl_MessageCenter.SendMessage("����", oilValue);
                        currentOilValue = oilValue;
                    }

                    break;
            }

            //ÿ��10�뷢��һ�δ���������
            UploadSensorData(noiseValue);
        }

        // float oilValue = IsFake ? 0f : SinglechipManager.Instance.GetOilValue();
        // if (ComparisonValue(currentOilValue, oilValue))
        // {
        //     Ctrl_MessageCenter.SendMessage("����", oilValue);
        //     currentOilValue = oilValue;
        // }
    }

    /// <summary>
    /// ���´���������
    /// </summary>
    /// <param name="voiceValue"></param>
    void UploadSensorData(float voiceValue)
    {
        //ͨ�����жϽ��д�������
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
    /// ���ݱȶԣ������Ƿ����
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
    /// ���������
    /// </summary>
    /// <param name="values">��ȷ���ݵ�ȡֵ��Χ</param>
    /// <param name="isValues">�Ƿ������ͷȡ����ֵ</param>
    /// <returns></returns>
    public float CreateFakeValue(float[] values, bool isValues)
    {
        float tmpValue = 0;

        if (isValues)
        {
        }

        return tmpValue;
    }

    #region ʱ��-���-����

    /// <summary>
    /// �������
    /// </summary>
    float num = 3000f;

    /// <summary>
    /// �����ϴεĸı�ʱ��
    /// �ٶ�-���-����
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

    #region ����ת��ָ��

    public float minRPM = 0f; // ��Сת��
    public float maxRPM = 35f; // ���ת��
    public float minAngle = 180f; // ��С�Ƕ�
    public float maxAngle = -131f; // ���Ƕ�

    /// <summary>
    /// �����ϴεĸı�ʱ��
    /// ָ����ת
    /// </summary>
    private float realtimeSinceStartupCacheII;

    float RPMValue;

    /// <summary>
    /// ����ָ����ת
    /// </summary>
    /// <param name="rpm"></param>
    public void UpdateNeedleRotation()
    {
        RPMValue = SinglechipManager.Instance.GetRPMValue();
        //ģ�⵱ǰת��
        float rpm = /*Random.Range(1100, 1250);*/ RPMValue;
        // ��ת��ӳ�䵽�Ƕ�
        float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(minRPM, maxRPM, rpm));

        //if (Time.realtimeSinceStartup - realtimeSinceStartupCacheII > 1f)
        //{
        //    Ctrl_MessageCenter.SendMessage("ָ����ת-rpm", rpm);
        //    realtimeSinceStartupCacheII = Time.realtimeSinceStartup;
        //}
        Ctrl_MessageCenter.SendMessage("ָ����ת-rpm", rpm);
        // ����ָ�����ת
        Ctrl_MessageCenter.SendMessage("ָ����ת-angle", angle);
    }

    #endregion

    #region ����ת��

    /// <summary>
    /// �Ƿ�������ݳ�ʼ����
    /// </summary>
    private bool isFanDataInitialized = false;

    /// <summary>
    /// ͼƬ��ת�ٶ�
    /// </summary>
    float fanRotateSpeed;

    Vector2 setSpeedValueRange;
    Vector2 cylinderTemperatureRange;

    /// <summary>
    /// �����ϴεĸı�ʱ��
    /// ����
    /// </summary>
    private float realtimeSinceStartupCacheIII;

    /// <summary>
    /// ����ת��ģ�鴦��(ֻ��Ҫ�����Ƿ�������ֵ��)
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
    /// ʵʱ���·�������
    /// </summary>
    void UpdateFanData()
    {
        if (!isFanDataInitialized)
        {
            InitProcessFanData();
        }

        if (Time.realtimeSinceStartup - realtimeSinceStartupCacheIII > 1f)
        {
            Ctrl_MessageCenter.SendMessage("����", Random.Range(setSpeedValueRange.x, setSpeedValueRange.y), Random.Range(cylinderTemperatureRange.x, cylinderTemperatureRange.y), fanRotateSpeed);
            realtimeSinceStartupCacheIII = Time.realtimeSinceStartup;
        }
    }

    #endregion

    #endregion

    /// <summary>
    /// ��ʼ����ʱ�������еĵ��Ƿ����ǰ�����ã�������˵��
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
    /// ���õ�ǰ����
    /// </summary>
    /// <param name="pointConfig"></param>
    private void CurrentDataConfig(DataCheckConfig pointConfig, bool isHand)
    {
        _currentDataConfig = pointConfig;
        GetConfigItem(_currentDataConfig.DefaultID);
    }

    /// <summary>
    /// ��������Ĵ��б��Ƴ�
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
    /// ͨ��ID�Ƴ����õĴ�����Ϣ
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
    /// ͨ����Ӧ��ID��ȡ��������
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

    #region ��ʼ������

    /// <summary>
    /// ���ڴ洢��ʾ��Tip����������
    /// </summary>
    private List<GameObject> tips = new();

    /// <summary>
    /// ����Ԥ����
    /// </summary>
    private Transform _pointTip;

    /// <summary>
    /// �����¼���еĵ�
    /// </summary>
    private Dictionary<int, List<GameObject>> allPointsDic = new Dictionary<int, List<GameObject>>();

    /// <summary>
    /// ��ʾ���еĴ�����(�������ȡ������ʾ������ȫ������ʾ)
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

        //��������״̬������
        List<DataCheckConfig> defaultValue = Instance.DataCheckDefaultValue;
        List<DataCheckConfig> datalist = DataCheckConfigTable.Instance.dataList;

        for (int i = 0; i < datalist.Count; i++)
        {
            //Ĭ�ϰ�Tip���ڵ�һ��·��ģ��λ��
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
                //�����Id����Ϊ�й���
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
            //�������е���ʾ��
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
    /// ͨ��ѡ���Type��������Tip
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
    /// ������±�����
    /// </summary>
    /// <param name="lineChart">Ҫ����ı�</param>
    /// <param name="dataName">��������</param>
    /// <param name="addDataValue">��Ҫ��ӵ�����</param>
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
    /// ��ȡ���������ϵ��Ӧ�Ĵ���������
    /// </summary>
    /// <returns></returns>
    public List<EquipmentCheckConfig2nd> GetDataCheckSensorConfigs(DataCheckConfig config)
    {
        //��ʼ�����û���
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
/// �������������
/// </summary>
public class ConfigItem
{
    /// <summary>
    /// ID
    /// </summary>
    public int Key { get; set; }

    /// <summary>
    /// ����ID
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// ��Ӧ�Ĵ�������
    /// </summary>
    public int DefaultValueID { get; set; }
}