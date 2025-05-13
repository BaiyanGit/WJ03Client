using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System.Linq;
using System;

public class NoiseSensorControl : MonoBehaviour
{
    public static NoiseSensorControl Instance;
    // 定义基本信息
    public string PortName = "com6";
    public int BaudRate = 4800;
    public Parity Parity = Parity.None;
    public int DataBits = 8;
    public StopBits StopBits = StopBits.One;


    // 线程间隔
    public int threadTimeSpan = 100;

    private SerialPort _sp;
    private Thread _dataReceiveThread;
    private Thread _dataRequestThread;
    private bool _bOpen;
    private object lockObj = new object();
    private object lockObjData = new object();


    // 接收数据缓存
    List<byte> _acceptingdata = new List<byte>();

    // 寄存器地址 00 温度 21 22  x震动 23 24 y震动 25 26 z震动
    // modul bus 协议 1个寄存器地址 2个字节
    byte[] byteRequest = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x01, 0x84, 0x0A };


    /// <summary>
    /// 记录有效数据
    /// </summary>
    private byte[] byteData = new byte[2];

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _bOpen = false;

        OpenPort();

        //如果成功打开串口
        if (_bOpen)
        {
            _dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
            _dataReceiveThread.Start();

            _dataRequestThread = new Thread(new ThreadStart(DataRequestFunction));
            _dataRequestThread.Start();
        }
    }


    /// <summary>
    /// 打开串口
    /// </summary>
    private void OpenPort()
    {
        PortName = SensorsComReadManager._coms[3];
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
        // 使用较长的缓冲区来读取数据
        byte[] buffer = new byte[56]; //新单片机返回报文

        while (_bOpen)
        {
            if (_sp != null && _sp.IsOpen)
            {
                try
                {
                    lock (lockObj)
                    {
                        // 读取串口数据
                        int length = _sp.Read(buffer, 0, buffer.Length);
                        // 将读取到的数据存入缓存
                        _acceptingdata.AddRange(buffer.Take(length));
                    }


                    // 数据不足3位不用处理
                    if (_acceptingdata.Count < 3)
                    {
                        continue;
                    }

                    // 比对报文头部,处理为正确的报文头
                    if (_acceptingdata[0] != 0x01 || _acceptingdata[1] != 0x03)
                    {
                        int num = _acceptingdata.Count;

                        //异常处理 抛掉异常的报文
                        for (int i = 0; i < num; i++)
                        {
                            if (_acceptingdata[0] == 0x01)
                            {
                                if (_acceptingdata.Count > 1)
                                {
                                    if (_acceptingdata[1] == 0x03)
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


                    // 温度 7 震动 17 为一个数据帧长度
                    // 此处将因网络问题造成的数据积压一次性处理完
                    while (_acceptingdata.Count >= 7)
                    {
                        // 数据长度
                        int dataLen = _acceptingdata[2];

                        // 长度不足一帧不处理
                        //1 位地址码+1数据类型+1数据长度+2校验
                        if (_acceptingdata.Count < dataLen + 5)
                        {
                            break;
                        }

                        //校验 类型+长度+数据
                        var chkBytes = CRC16Standard.CrcCalc(_acceptingdata.ToArray(), 0, dataLen + 3);
                        // 校验结果正确
                        if (_acceptingdata[dataLen + 3] == chkBytes[0] && _acceptingdata[dataLen + 4] == chkBytes[1])
                        {
                            var dat = _acceptingdata.ToArray();
                            lock (lockObjData)
                            {
                                Array.Copy(dat, 3, byteData, 0, dataLen);
                            }

                            _acceptingdata.RemoveRange(0, dataLen + 5);

                        }
                        else // 校验不正确，丢掉一帧
                        {
                            _acceptingdata.RemoveRange(0, dataLen + 5);

                            break;
                        }
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
    /// 发送请求数据报文  间隔发送温度和振动请求
    /// </summary>
    private void DataRequestFunction()
    {
        while (_bOpen)
        {
            lock (lockObj)
            {
                if (_sp.IsOpen)
                {
                    _sp.Write(byteRequest, 0, byteRequest.Length);
                }
            }

            Thread.Sleep(threadTimeSpan);
        }
    }

    /// <summary>
    /// 获取噪声值
    /// </summary>
    /// <returns></returns>
    public float GetNoiseValue()
    {
        float ret = 0;
        int tmpVal = 0;

        lock (lockObjData)
        {
            tmpVal = byteData[0] * 256 + byteData[1];
        }

        ret = tmpVal / 10f;

        return ret;
    }

    /// <summary>
    /// 退出时发送结束
    /// </summary>
    private void OnApplicationQuit()
    {
        _bOpen = false;

        if (_sp != null)
        {
            _sp.Close();
        }
    }
}
