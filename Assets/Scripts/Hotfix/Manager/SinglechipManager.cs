using System.Collections.Generic;
using System.IO;
using Hotfix;
using UnityEngine;
using Wx.Runtime.Singleton;

public class SinglechipManager : SingletonInstance<SinglechipManager>, ISingleton
{
    //public static SinglechipManager Instance;

    //private void Awake()
    //{
    //    Instance = this;
    //}

    private string configFilePath;

    // 使用字典存储每个传感器的值
    private Dictionary<string, List<int>> sensorConfigs;

    /// <summary>
    /// 滤波
    /// </summary>
    private Dictionary<int, Queue<float>> SensorBuffers = new Dictionary<int, Queue<float>>();
    /// <summary>
    /// 滤波窗口大小
    /// </summary>
    private int filterWindowSize = 8; 


    /// <summary>
    /// 信息比对
    /// </summary>
    /// <param name="targetKey">目标键值</param>
    /// <param name="currentKey">当前变化键值</param>
    /// <returns></returns>
    public bool ComparisonKeyInfo(int targetKey, int currentKey)
    {
        return false;
    }

    public void ComparisonKeyInfo(int currentKey)
    {
        Ctrl_MessageCenter.SendMessage<int>("ChangeSinglechipValue", currentKey);
    }

    /// <summary>
    ///获取为True的值，然后发送出去
    /// </summary>
    List<int> portValues = new();

    private void GetChangeValue(int offset = 0)
    {
        for (int i = 0; i < 32; i++)
        {
            int index = i + offset;
            bool isAnyKeyValue;

            isAnyKeyValue = offset == 0 ? PortControl.Instance.GetKey(i) : PortControl.Instance.GetAnyKey(index);

            if (isAnyKeyValue)
            {
                if (portValues.Contains(index)) return;
                portValues.Add(index);
                ComparisonKeyInfo(index + 1);
                Debug.Log("进来一次");
            }
            else
            {
                if (portValues.Contains(index))
                {
                    portValues.Remove(index);
                    Debug.Log("移除一次");
                }
            }
        }
    }

    public bool TestComparisonKeyInfo(int targetKey)
    {
        return false;
    }

    /// <summary>
    /// 获取缸温温度值
    /// </summary>
    public float GetTemperatureValue()
    {
        float value = CarSensorsControl._instance.GetResistanceSensorWithFilter(sensorConfigs["Resistor"][1]);
        value = CarSensorsValueTool.Instance.GetTemperature(value, .102f);
        //Debug.Log("油温___________________" + value);
        //value = AverageFilter(0, (int)value);
        return value;
    }

    /// <summary>
    /// 获取油温温度值
    /// </summary>
    public float GetOilTemperatureValue()
    {
        float value = CarSensorsControl._instance.GetResistanceSensorWithFilter(sensorConfigs["Resistor"][2]);
        value = CarSensorsValueTool.Instance.GetTemperature(value,0.027f);
        //Debug.Log("油温++++++++++++++++++++" + value);
        //value = AverageFilter(1, (int)value);
        return value;
    }

    /// <summary>
    /// 获取气压值KPA
    /// </summary>
    /// <returns></returns>
    public float GetKPAValue()
    {
        float value = CarSensorsControl._instance.GetVoltageSensorWithFilterTransValue(sensorConfigs["Voltage"][0]);
        //if (value <= 5) return value = 0f;
        //value = AverageFilter(2, (int)value);
        return value;
    }

    /// <summary>
    /// 获取气压II值KPA
    /// </summary>
    /// <returns></returns>
    public float GetKPAIIValue()
    {
        float value = CarSensorsControl._instance.GetVoltageSensorWithFilterTransValue(sensorConfigs["Voltage"][1]);
        //if (value <= 5) return value = 0f;
        //value = AverageFilter(2, (int)value);
        return value;
    }

    /// <summary>
    /// 获取压力值
    /// </summary>
    /// <returns></returns>
    public float GetAPSValue()
    {
        float value = CarSensorsControl._instance.GetVoltageSensorWithFilterTransValue(sensorConfigs["Voltage"][2]);
        //if (value <= 5) return value = 0f;
        //value = AverageFilter(3, (int)value);
        value /= 100;
        return value;
    }

    /// <summary>
    /// 获取油压
    /// </summary>
    /// <returns></returns>
    public float GetOilKPAValue()
    {
        float value = CarSensorsControl._instance.GetResistanceSensorWithFilter(sensorConfigs["Resistor"][0]);
        value = CarSensorsValueTool.Instance.GetPressures(value, 0.1f);
        value /= 100;
        //value = AverageFilter(4, (int)value);
        return value;
    }

    /// <summary>
    /// 获取油量
    /// </summary>
    /// <returns></returns>
    public float GetOilValue()
    {
        float value = CarSensorsControl._instance.GetVoltageSensorWithFilterTransValue(sensorConfigs["Voltage"][3]);
        value = value * 200;
        //value = AverageFilter(5, value);
        return value;
    }

    /// <summary>
    /// 获取电压数据
    /// </summary>
    /// <returns></returns>
    public float GetElectricityValue()
    {
        float value = CarSensorsControl._instance.GetVoltageSensorWithFilterTransValue(sensorConfigs["Voltage"][4]);
        //value = AverageFilter(6, (int)value);
        return value;
    }

    /// <summary>
    /// 获取转速
    /// </summary>
    /// <returns></returns>
    public float GetRPMValue()
    {
        float value = CarSensorsControl._instance.GetFrequencySensorWithFilterTransValue(sensorConfigs["Frequency"][0]);
        //value = AverageFilter(7, (int)value);
        return value;
    }

    /// <summary>
    /// 获取车速里程
    /// </summary>
    /// <returns></returns>
    public float GetSpeedValue()
    {
        float value = CarSensorsControl._instance.GetFrequencySensorWithFilterTransValue(sensorConfigs["Frequency"][1]);
        //value = AverageFilter(8, (int)value);
        return value;
    }

    private float AverageFilter(int index, int value)
    {
        if (!SensorBuffers.ContainsKey(index))
        {
            SensorBuffers[index] = new Queue<float>(filterWindowSize);
        }

        Queue<float> buffer = SensorBuffers[index];

        // 先移除旧数据再添加新数据（固定窗口长度）
        if (buffer.Count >= filterWindowSize)
        {
            buffer.Dequeue();
        }
        buffer.Enqueue(value);

        // 计算平均值
        int sum = 0;
        foreach (int val in buffer)
        {
            sum += val;
        }
        return sum / buffer.Count;
    }

    public void OnCreate(object createParam)
    {
        // 确定配置文件路径
        configFilePath = Path.Combine(Application.streamingAssetsPath, "Config", "DataScreenValueConfig.txt");

        // 初始化字典
        sensorConfigs = new Dictionary<string, List<int>>();

        // 读取配置文件
        GetConfig();
    }

    public void OnUpdate()
    {
        GetChangeValue();
        GetChangeValue(100);
    }

    public void OnFixedUpdate()
    {
    }

    public void OnLateUpdate()
    {
    }

    public void OnDestroy()
    {
    }

    void GetConfig()
    {
        // 检查文件是否存在
        if (!File.Exists(configFilePath))
        {
            Debug.LogError("Config file not found at: " + configFilePath);
            return;
        }

        // 读取文件中的所有行
        string[] lines = File.ReadAllLines(configFilePath);

        string currentSensor = null;

        foreach (string line in lines)
        {
            // 去掉空白字符
            string trimmedLine = line.Trim();

            // 忽略空行和注释
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
            {
                continue;
            }

            // 检查是否是传感器区块
            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                currentSensor = trimmedLine.Substring(1, trimmedLine.Length - 2);
                sensorConfigs[currentSensor] = new List<int>(); // 初始化列表
            }
            else if (currentSensor != null)
            {
                // 尝试将值解析为int
                if (int.TryParse(trimmedLine, out int sensorValue))
                {
                    // 将值添加到当前传感器的列表中
                    sensorConfigs[currentSensor].Add(sensorValue);
                }
                else
                {
                    Debug.LogWarning($"Failed to parse value for sensor '{currentSensor}': {trimmedLine}");
                }
            }
        }

        //// 打印读取的配置
        //foreach (var sensor in sensorConfigs)
        //{
        //    Debug.Log($"Sensor: {sensor.Key}");
        //    foreach (int value in sensor.Value)
        //    {
        //        Debug.Log($"  Value: {value}");
        //    }
        //}
    }
}