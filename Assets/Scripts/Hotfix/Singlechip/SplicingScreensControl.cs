/*
 * 脚本名称：SplicingScreensControl.cs
 * 脚本功能：新疆wj03项目与大屏幕通信控制类
 * 发送命令控制切换大屏布局
 * 开发时间：2025.1.8
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Threading;
using System;

public class SplicingScreensControl : MonoBehaviour
{
    public static SplicingScreensControl Instance;

    //定义基本信息
    public string PortName = "COM4";
    public int BaudRate = 115200; // 厂家技术人员提供波特率
    public Parity Parity = Parity.None;
    public int DataBits = 8;
    public StopBits StopBits = StopBits.One;

    private SerialPort _sp;
    private Thread _dataReceiveThread;
    private Thread _dataRequestThread;

    private bool _bOpen;
    private int requestModel = 0;

    private object lockObj = new object();

    private Queue<byte[]> cmdQueue = new Queue<byte[]>();

    private List<byte[]> cmdList = new List<byte[]>
    {
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x01, 0x00, 0x00, 0x00, 0x00, 0xC4 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x02, 0x00, 0x00, 0x00, 0x00, 0xC5 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x03, 0x00, 0x00, 0x00, 0x00, 0xC6 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x04, 0x00, 0x00, 0x00, 0x00, 0xC7 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x05, 0x00, 0x00, 0x00, 0x00, 0xC8 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x06, 0x00, 0x00, 0x00, 0x00, 0xC9 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x07, 0x00, 0x00, 0x00, 0x00, 0xCA },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x08, 0x00, 0x00, 0x00, 0x00, 0xCB },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x09, 0x00, 0x00, 0x00, 0x00, 0xCC },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x0A, 0x00, 0x00, 0x00, 0x00, 0xCD },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x0B, 0x00, 0x00, 0x00, 0x00, 0xCE },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x0C, 0x00, 0x00, 0x00, 0x00, 0xCF },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x0D, 0x00, 0x00, 0x00, 0x00, 0xD0 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x0E, 0x00, 0x00, 0x00, 0x00, 0xD1 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x83, 0x06, 0x0F, 0x00, 0x00, 0x00, 0x00, 0xD2 },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x02, 0x01, 0x3D },
        new byte[] { 0xAA, 0x88, 0x08, 0x00, 0x03, 0x01, 0x3E }
    };


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        OpenPort();

        //如果成功打开串口
        if (_bOpen)
        {
            _dataReceiveThread = new Thread(new ThreadStart(DataRequest));
            _dataReceiveThread.Start();
        }
    }

    /// <summary>
    /// 打开串口
    /// </summary>
    private void OpenPort()
    {
        PortName = SensorsComReadManager._coms[1];

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
            Debug.Log("OpenPort()" + PortName + ex.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void DataRequest()
    {
        while (_bOpen)
        {
            lock (lockObj)
            {
                if (cmdQueue.Count > 0)
                {
                    var requestBytes = cmdQueue.Dequeue();
                    _sp.Write(requestBytes, 0, requestBytes.Length);
                }
            }

            Thread.Sleep(30);
        }
    }

    /// <summary>
    /// 增加命令到队列
    /// </summary>
    /// <param name="inde"></param>
    private void AddCmdToQueue(int inde)
    {
        var cmd = cmdList[inde];

        lock (lockObj)
        {
            cmdQueue.Enqueue(cmd);
        }
    }

    /// <summary>
    /// 切换屏幕布局方案
    /// </summary>
    /// <param name="inde">布局编号 范围为1-15</param>
    public void ChangeLayout(int inde)
    {
        // 布局编号不对，不做处理
        if (inde < 1 || inde > 15)
        {
            return;
        }

        AddCmdToQueue(inde - 1);
    }

    /// <summary>
    /// 设备休眠
    /// </summary>
    public void DeviceHibernate()
    {
        AddCmdToQueue(15);
    }

    /// <summary>
    /// 设备运行
    /// </summary>
    public void DeviceRun()
    {
        AddCmdToQueue(16);
    }

    /// <summary>
    /// 退出程序时关闭串口
    /// </summary>
    private void OnApplicationQuit()
    {
        if (_sp != null)
        {
            _sp.Close();
            _bOpen = false;
        }
    }
}