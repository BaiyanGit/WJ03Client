/*
 * 脚本名称：CarSensorsControl.cs 
 * 脚本功能：新疆wj03项目传感器采集单片机控制类
 * 采集多路传感器数据
 * 开发时间：2025.1.10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;

public class CarSensorsControl : MonoBehaviour
{
    public static CarSensorsControl _instance;
    // 定义基本信息
    public string PortName = "com3";
    public int BaudRate = 115200;
    public Parity Parity = Parity.None;
    public int DataBits = 8;
    public StopBits StopBits = StopBits.One;


    // 线程间隔
    public int threadTimeSpan = 20;

    private SerialPort _sp;
    private Thread _dataReceiveThread;
    private Thread _dataRequestThread;
    private bool _bOpen;
    private object lockObj = new object();
  
    // 接收数据缓存
    List<byte> _acceptingdata = new List<byte>();

    // 用于缓存的过滤器字典
    Dictionary<int ,Queue<int>> filterDic = new Dictionary<int , Queue<int>>();

    private int[] filterData = new int[24];

    private int filterCount = 100;

    public bool openFilter = false;
    /// <summary>
    /// 记录有效数据
    /// </summary>
    private byte[] byteData = new byte[80];

    SensorRangeInfos sensorInfos = new SensorRangeInfos();

    private void Awake()
    {
        SensorsComReadManager.Init();

        _instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        _bOpen = false;
        PortName = SensorsComReadManager._coms[0];
        InitSensorRange();

        OpenPort();


        //如果成功打开串口
        if (_bOpen)
        {
            SendStart();

            _dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
            _dataReceiveThread.Start();
        }
    }

    /// <summary>
    /// 读取传感器量程范围数据
    /// </summary>
    private void InitSensorRange()
    {
        string path = Application.streamingAssetsPath + @"/Config/SensorRange.json";
        string ranges = File.ReadAllText(path);

        sensorInfos = JsonConvert.DeserializeObject<SensorRangeInfos>(ranges);
    }

    /// <summary>
    /// 打开串口
    /// </summary>
    private void OpenPort()
    {
        _sp = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
        _sp.ReadTimeout = 400;
        try
        {
            _sp.Open();

            if (_sp.IsOpen)
            {
                Debug.Log("串口已经打开");
                _bOpen = true;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("OpenPort() err " + PortName + ex.Message);
        }
    }

    /// <summary>
    /// 接收数据
    /// </summary>
    private void DataReceiveFunction()
    {
        // 校验功能当前采集值长度 87 因为数据发送较快，使用较长的缓冲区来读取数据
        byte[] buffer = new byte[1024]; //新单片机返回报文

        while (_bOpen)
        {
            if (_sp != null && _sp.IsOpen)
            {
                try
                {
                    // 读取串口数据
                    int length = _sp.Read(buffer, 0, buffer.Length);
                    // 将读取到的数据存入缓存
                    _acceptingdata.AddRange(buffer.Take(length));

                    // 数据不足2位不用处理
                    if (_acceptingdata.Count<2)
                    {
                        continue;
                    }

                    bool isContinue = false;
                    // 87为一个数据帧长度
                    while(_acceptingdata.Count >=87)
                    {
                        // 比对报文头部,处理为正确的报文头
                        if (_acceptingdata[0] != 0x23 || _acceptingdata[1] != 0x23)
                        {
                            int num = _acceptingdata.Count;

                            //异常处理 抛掉异常的报文
                            for (int i = 0; i < num; i++)
                            {
                                if (_acceptingdata[0] == 0x23)
                                {
                                    if (_acceptingdata.Count > 1)
                                    {
                                        if (_acceptingdata[1] == 0x23)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            _acceptingdata.RemoveAt(0);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    _acceptingdata.RemoveAt(0);
                                }
                            }
                        }
                    
                    
                        // 此处报文头已正确
                        if (_acceptingdata.Count > 4) //5个字节可得知报文长度
                        {

                            // 此处数据并非数据长度，是数据最高下标 及 实际数据长度=dataLen+1
                            int dataLen = _acceptingdata[3]+ _acceptingdata[4]*256;

                            // 长度不足一帧不处理
                            if (_acceptingdata.Count < dataLen + 7) //2位头+1数据类型+2数据长度+2校验
                            {
                                isContinue = true;
                                break;
                            }

                            //校验 类型+长度+数据
                            var chkBytes = CRC16Standard.CrcCalc(_acceptingdata.ToArray(), 2, dataLen + 3);
                            // 校验结果正确
                            if (_acceptingdata[dataLen + 5] == chkBytes[0] && _acceptingdata[dataLen + 6] == chkBytes[1])
                            {
                                var dat = _acceptingdata.ToArray();

                                lock (lockObj)
                                {
                                    Array.Copy(dat, 5, byteData, 0, dataLen);
                                }

                                _acceptingdata.RemoveRange(0, dataLen + 7);

                                if(openFilter)
                                {
                                    // 执行滤波程序
                                    RunningFilter();
                                }
                            }
                            else // 校验不正确，丢掉一帧
                            {
                                _acceptingdata.RemoveRange(0, dataLen + 7);
                                isContinue = true;
                                break;
                            }
                        }
                    }

                    if(isContinue)
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ThreadAbortException) && ex.GetType() != typeof(TimeoutException))
                    {
                        Debug.Log(" 读取 err： " + ex.Message);

                    }
                }
            }
            Thread.Sleep(threadTimeSpan);
        }
    }

    /// <summary>
    /// 使用滤波器 （中值平均滤波）
    /// </summary>
    private void RunningFilter()
    {
        
        lock (lockObj)
        {
            // 解析数据并添加到缓冲区
            for (int i = 0; i < 16; i++)
            {
                int val = byteData[2*i] + byteData[2*i + 1] * 256;

                if(filterDic.ContainsKey(i))
                {
                    Queue<int> filterList = filterDic[i];
                    

                    if (filterList.Count == filterCount)
                    {
                        filterList.Dequeue();
                    }

                    filterList.Enqueue(val);
                }
                else
                {
                    Queue<int> list = new Queue<int>();
                    list.Enqueue(val);

                    filterDic.Add(i,list);
                }
            }

     

            for (int i = 0;i < 8;i++)
            {
                int val = byteData[64+2*i] + byteData[64 + 2 * i + 1] * 256;

                if (filterDic.ContainsKey(i+16))
                {
                    Queue<int> filter = filterDic[i+16];
                    if (filter.Count == filterCount)
                    {
                        filter.Dequeue();
                    }

                    filter.Enqueue(val);
                }
                else
                {
                    Queue<int> list = new Queue<int>();
                    list.Enqueue(val);

                    filterDic.Add(i+16, list);
                }
            }

            // 分别计算滤波后的值
            for(int i=0;i<24;i++)
            {
                Queue<int> lst = filterDic[i];

                // 一个通道数据不对，都跳出
                if(lst.Count < filterCount)
                {
                    break;
                }

                var tmpdata = lst.ToArray();

                if(tmpdata.Length>10)
                {
                    //Debug.Log("tmpdata: " + tmpdata.Length);
                }

                // 冒泡排序
                for(int j=0;j< filterCount - 1;j++) 
                {
                    for(int k=0;k< filterCount - j-1;k++)
                    {
                        if (tmpdata[k] > tmpdata[k+1])
                        {
                            int temp = tmpdata[k];
                            tmpdata[k] = tmpdata[ k+1];
                            tmpdata[k+1] = temp;
                        }
                    }
                }

                int sum = 0;
                for(int j=1;j<filterCount-1;j++)
                {
                    sum += tmpdata[j];
                }

                if(i==6)
                {
                    //Debug.Log("a");
                }
                filterData[i] = sum/(filterCount-2);
            }
        }
    }

    /// <summary>
    /// 获取滤波后的电阻值传感器数据
    /// </summary>
    /// <param name="inde">下标</param>
    /// <returns>滤波后原始值</returns>
    public int GetResistanceSensorWithFilter(int inde)
    {
        int ret = 0;

        // 下标超出范围
        if (inde > 5 || inde < 0)
        {
            return ret;
        }

        lock(lockObj)
        {
            ret = filterData[inde];
        }

        return ret;
    }

    /// <summary>
    /// 获取滤波后的电压值传感器数据
    /// </summary>
    /// <param name="inde">下标</param>
    /// <returns>滤波后原始值</returns>
    private int GetVoltageSensorWithFilter(int inde)
    {
        int ret = 0;

        // 下标超出范围
        if (inde > 9 || inde < 0)
        {
            return ret;
        }

        lock (lockObj)
        {
            ret = filterData[inde+6];
        }

        return ret;
    }

    /// <summary>
    /// 获取滤波后的电压值传感器数据
    /// </summary>
    /// <param name="inde">下标</param>
    /// <returns>转换后的值</returns>
    public float GetVoltageSensorWithFilterTransValue(int inde)
    {
        float ret = 0;

        int sVal = GetVoltageSensorWithFilter(inde);

        ret = ChangValue(1, inde, sVal);
        return ret; 
    }

    /// <summary>
    /// 获取滤波后频率传感器数据
    /// </summary>
    /// <param name="inde">下标</param>
    /// <returns>转换后的值</returns>
    public float GetFrequencySensorWithFilterTransValue(int inde)
    {
        float ret = 0;

        int sVal = GetFrequencySensorWithFilter(inde);

        ret = ChangValue(2, inde, sVal);
        return ret;
    }


    /// <summary>
    /// 获取滤波后的频率传感器数据
    /// </summary>
    /// <param name="inde">下标</param>
    /// <returns>滤波后原始值</returns>
    private int GetFrequencySensorWithFilter(int inde)
    {
        int ret = 0;

        // 下标超出范围
        if (inde > 7 || inde < 0)
        {
            return ret;
        }

        lock (lockObj)
        {
            ret = filterData[inde+16];
        }

        return ret;
    }


    /// <summary>
    /// 根据下标获得电阻传感器值
    /// </summary>
    /// <param name="inde">下标 从0开始 </param>
    /// <returns>换算后的值</returns>
    public float GetResistanceSensorValue(int inde)
    {
        float ret = 0;

        int sVal = GetResistanceSensor(inde);

        ret = ChangValue(0, inde, sVal);

        return ret;
    }

    /// <summary>
    /// 根据下标获得电压传感器值
    /// </summary>
    /// <param name="inde">下标 从0开始</param>
    /// <returns>换算后的值</returns>
    public float GetVoltageSensorValue(int inde)
    {
        float ret = 0;

        int sVal = GetVoltageSensor(inde);

        ret = ChangValue(1, inde, sVal);
        
        return ret;
    }

    /// <summary>
    /// 根据下标获得频率传感器值
    /// </summary>
    /// <param name="inde">下标 从0开始</param>
    /// <returns>换算后的值</returns>
    public float GetFrequencySensorValue(int inde)
    {
        float ret = 0;

        int sVal = GetFrequencySensor(inde);

        ret = ChangValue(2, inde, sVal);

        return ret;
    }

    /// <summary>
    /// 根据传感器值换算为传感器量程
    /// </summary>
    /// <param name="sensorType">传感器类型 0 电阻 1 电压 2 频率</param>
    /// <param name="inde">传感器编号，从0开始 </param>
    /// <param name="val">当前传感器值</param>
    /// <returns>换算后的值</returns>
    private float ChangValue(int sensorType,int inde,int val)
    {
        float ret = 0;
        List<SensorRange> range = new List<SensorRange>();
        switch (sensorType)
        {
            case 0:
                range = sensorInfos.ResistanceSensor;
                break;
            case 1:
                range = sensorInfos.VoltageSensor;
                break;
            case 2:
                range = sensorInfos.FrequencySensor;
                break;
            default:
                break;
        }

        if(range.Count<inde+1)
        {
            ret = (float)val;
            return ret;
        }

        if(val>range[inde].MaxValue)
        {
            val = range[inde].MaxValue;
        }

        if(val<range[inde].MinValue)
        {
            val = range[inde].MinValue;
        }


        ret = (float)(val - range[inde].MinValue) * (range[inde].MaxMeasure - range[inde].MinMeasure) / (range[inde].MaxValue - range[inde].MinValue) + range[inde].MinMeasure;

        return ret;
    }

    /// <summary>
    /// 根据下标获取电阻传感器的值
    /// </summary>
    /// <param name="inde">传感器编号0-5 最多支持6个</param>
    /// <returns></returns>
    public int GetResistanceSensor(int inde)
    {
        int ret = 0;

        // 下标超出范围
        if(inde>5 || inde<0)
        {
            return ret;
        }

        ret = GetValue(2 * inde);

        return ret;
    }

    /// <summary>
    /// 根据下标获取电压传感器的值
    /// </summary>
    /// <param name="index">传感器编号 0-9 最多10个传感器</param>
    /// <returns></returns>
    private int GetVoltageSensor(int inde)
    {
        int ret = 0;

        // 下标超出范围
        if (inde > 9 || inde < 0)
        {
            return ret;
        }

        ret = GetValue(12+2 * inde);

        return ret;
    }

    /// <summary>
    /// 根据下标获取频率传感器的值
    /// </summary>
    /// <param name="index">传感器编号 0-7 最多8个传感器</param>
    /// <returns></returns>
    private int GetFrequencySensor(int inde)
    {
        int ret = 0;

        // 下标超出范围
        if (inde > 7 || inde < 0)
        {
            return ret;
        }

        ret = GetValue(64 + 2 * inde);

        return ret;
    }

    /// <summary>
    /// 从有效数据中取值
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private int GetValue(int index)
    {
        int ret = 0;

        lock (lockObj)
        {
            ret = byteData[index] + byteData[index + 1] * 256;
        }

        return ret;
    }
    /// <summary>
    /// 发送一个开始接收的命令
    /// </summary>
    private void SendStart()
    {
        if (_bOpen && _sp != null && _sp.IsOpen)
        {
           _sp.Write("<[RONGYI_CMD:1]>");
        }
        

    }

    /// <summary>
    /// 发送一个结束接收的命令
    /// </summary>
    private void SendEnd()
    {
        if (_bOpen && _sp != null && _sp.IsOpen)
        {
            _sp.Write("<[RONGYI_CMD:2]>");
        }
    }

    /// <summary>
    /// 退出时发送结束
    /// </summary>
    private void OnApplicationQuit()
    {
        SendEnd();

        if (_sp != null)
        {
            _sp.Close();
            _bOpen = false;
        }
    }






}
