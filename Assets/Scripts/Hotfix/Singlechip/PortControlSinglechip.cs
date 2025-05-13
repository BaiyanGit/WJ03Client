using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using UnityEngine;

public class PortControlSinglechip
{
    //定义基本信息
    public string PortName = "";
    public int BaudRate = 115200;
    public Parity Parity = Parity.None;
    public int DataBits = 8;
    public StopBits StopBits = StopBits.One;

    private SerialPort _sp;
    private Thread _dataReceiveThread;
    private Thread _dataRequestThread;

    public bool _bOpen;

    /// <summary>
    /// 模拟量（数量24）
    /// </summary>
    private byte[] _analogs = new byte[24];
    /// <summary>
    /// 开关量（数量32）  新单片机开关量 4*8 32路
    /// </summary>
    public bool[] _switchs = new bool[32];
    /// <summary>
    /// 薄膜开关
    /// </summary>
    private bool[] _keyboard = new bool[16];
    /// <summary>
    /// 薄膜开关临时
    /// </summary>
    private bool[] _keyboardTemp = new bool[16];

    List<byte> _acceptingdata = new List<byte>();

    public void InitPort(string portName)
    {
        _bOpen = false;

        PortName = portName;
        OpenPort();


        //如果成功打开串口
        if (_bOpen)
        {

            _dataRequestThread = new Thread(new ThreadStart(DataRequestFunction));
            _dataRequestThread.Start();

            _dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
            _dataReceiveThread.Start();
        }
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
                Debug.Log(PortName + "串口已经打开");
                _bOpen = true;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("OpenPort()" + ex.Message);

        }
    }

    /// <summary>
    /// 发送请求数据报文  新版单片机需要请求才发送数据
    /// </summary>
    private void DataRequestFunction()
    {
        while (_bOpen)
        {
            byte[] requestBytes;
            if (islight)   //发送一次灯光信息
            {
                requestBytes = GetRequestDateLight();
                islight = false;
            }
            else
            {
                requestBytes = GetRequestDate();
            }

            lock (this)
            {
                if (_sp != null && _sp.IsOpen && !islight)
                {
                    _sp.Write(requestBytes, 0, requestBytes.Length);
                }
            }
       
            Thread.Sleep(100);
        }
    }

    private byte[] GetRequestDate()
    {

        byte[] requestBytes = null;

        requestBytes = new byte[8];

        requestBytes[0] = 0xEF;
        requestBytes[1] = 0x01;
        requestBytes[2] = 0x01;
        requestBytes[3] = 0x04;
        requestBytes[4] = 0x00;
        requestBytes[5] = 0x00;
        requestBytes[6] = 0x00;
        requestBytes[7] = 0x04;

        return requestBytes;
    }


    /// <summary>
    /// 接收单片机返回的报文，并放入缓存队列
    /// 将缓存队列的数据送入解析函数
    /// </summary>
    private void DataReceiveFunction()
    {
        // 2024.1.3 正常数据采集长度为37
        // 校验功能当前采集值长度 56
        byte[] buffer = new byte[56]; //新单片机返回报文

        while (_bOpen)
        {
            lock (this)
            {
                if (_sp != null && _sp.IsOpen)
                {
                    try
                    {
                        // 读取串口数据
                        int length = _sp.Read(buffer, 0, 56);
                        // 将读取到的数据存入缓存
                        _acceptingdata.AddRange(buffer.Take(length));

                        // 比对报文头部,处理为正确的报文头
                        if (_acceptingdata[0] != 0xEF && _acceptingdata[1] != 0x01)
                        {
                            int num = _acceptingdata.Count;

                            //异常处理 抛掉异常的报文
                            for (int i = 0; i < num; i++)
                            {
                                if (_acceptingdata[0] == 0xEF)
                                {
                                    if (_acceptingdata.Count > 1)
                                    {
                                        if (_acceptingdata[1] == 0x01)
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
                        if (_acceptingdata.Count > 4) //大于4个字节可得知报文长度
                        {
                            int dataLen = _acceptingdata[3];

                            // 长度不足一帧不处理
                            if (_acceptingdata.Count < dataLen + 4)
                            {
                                continue;
                            }


                            //校验 和校验
                            int num = 0;
                            for (int i = 3; i < 35; i++)
                            {
                                num += _acceptingdata[i];
                            }
                            if (_acceptingdata[35] * 256 + _acceptingdata[36] == num)
                            {
                                //string s = BitConverter.ToString(_acceptingdata.Take(dataLen + 4).ToArray()).Replace("-", " ");
                                //Debug.Log("byte: " + s);

                                DataInWork(_acceptingdata.Take(dataLen + 4).ToArray());
                                _acceptingdata.RemoveRange(0, dataLen + 4);
                            }
                            else
                            {
                                _acceptingdata.RemoveRange(0, dataLen + 4);
                                continue;
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
            }
            Thread.Sleep(100);
        }
    }


    /// <summary>
    /// 解析接收到的报文
    /// </summary>
    /// <param name="buffer"></param>
    private void DataInWork(byte[] buffer)
    {
        //解析模拟量
        byte[] analogyList = new byte[24];
        for (int i = 0; i < 24; i++)
        {
            analogyList[i] = buffer[4 + i];
        }
        //解析开关量
        bool[] switchList = new bool[32];
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                switchList[i * 8 + j] = ByteHelper.GetValue(buffer[28 + i], 7 - j);
            }
        }
        //解析薄膜开关
        byte value = buffer[34];
        KeyBoard(value);

        _analogs = analogyList;
        _switchs = switchList;
        _keyboard = _keyboardTemp;

    }

    /// <summary>
    /// 获取开关量
    /// </summary>
    public bool GetKey(int index)
    {
        if (index >= 0 && index < 32)
        {
            return _switchs[index];
        }
        return false;
    }

    /// <summary>
    /// 获取模拟量
    /// </summary>
    public byte GetAxis(int index)
    {
        if (index >= 0 && index < 24)
            return _analogs[index];
        return 0;
    }

    /// <summary>
    /// 获取薄膜开关的值
    /// </summary>
    public bool GetKeyBoard(int index)
    {
        if (index >= 0 && index < 16)
        {
            return _keyboard[index];
        }
        return false;
    }

    private bool islight = false;
    private byte indexlight;
    /// <summary>
    /// 让灯亮或者不亮
    /// </summary>
    public void MakeLightOnOrOff(byte index)
    {
        indexlight = index;
        islight = true;
    }

    private byte[] GetRequestDateLight()
    {

        byte[] lightBytes = null;

        lightBytes = new byte[7];

        lightBytes[0] = 0xEF;
        lightBytes[1] = 0x01;
        lightBytes[2] = 0x0A;
        lightBytes[3] = 0x03;
        lightBytes[4] = indexlight;
        lightBytes[5] = 0;
        int check = 0x03 + indexlight;
        lightBytes[6] = (byte)check;

        return lightBytes;
    }


    /// <summary>
    /// 键盘值
    /// </summary>
    private void KeyBoard(byte _byte)
    {
        for (int i = 0; i < 16; i++)
        {
            _keyboardTemp[i] = false;
        }
        switch (_byte)
        {
            case 0x53:
                {
                    //1|0
                    _keyboardTemp[0] = true;
                }
                break;
            case 0x5b:
                {
                    //2|1  
                    _keyboardTemp[1] = true;
                }
                break;
            case 0x63:
                {
                    //3|2
                    _keyboardTemp[2] = true;
                }
                break;
            case 0x52:
                {
                    //4|3
                    _keyboardTemp[3] = true;
                }
                break;
            case 0x5a:
                {
                    //5|4
                    _keyboardTemp[4] = true;
                }
                break;
            case 0x62:
                {
                    //6|5
                    _keyboardTemp[5] = true;
                }
                break;
            case 0x51:
                {
                    //7|6
                    _keyboardTemp[6] = true;
                }
                break;
            case 0x59:
                {
                    //8|7
                    _keyboardTemp[7] = true;
                }
                break;
            case 0x61:
                {
                    //9|8
                    _keyboardTemp[8] = true;
                }
                break;
            case 0x50:
                {
                    //退出|9
                    _keyboardTemp[9] = true;
                }
                break;
            case 0x58:
                {
                    //0|10
                    _keyboardTemp[10] = true;
                }
                break;
            case 0x60:
                {
                    //确定|11
                    _keyboardTemp[11] = true;
                }
                break;
            case 0x6b:
                {
                    //功能1|12
                    _keyboardTemp[12] = true;
                }
                break;
            case 0x6a:
                {
                    //菜单|13
                    _keyboardTemp[13] = true;
                }
                break;
            case 0x69:
                {
                    //功能2|14
                    _keyboardTemp[14] = true;
                }
                break;
            case 0x68:
                {
                    //功能3|14
                    _keyboardTemp[15] = true;
                }
                break;
        }

    }

    /// <summary>
    /// 关闭串口
    /// </summary>
    private void ClosePort()
    {
        try
        {
            _sp.Close(); //新单片机 采用请求发送模式，不请求，不发送报文，此处不需发送关闭报文
        }
        catch (Exception ex)
        {
            Debug.Log("关闭" + ex.Message);
        }
    }

    /// <summary>
    /// 程序退出时关闭端口，避免异常
    /// </summary>
    public void ClosePortControl()
    {
        if (_bOpen)
        {
            ClosePort();
            _bOpen = false;
        }
    }
}
