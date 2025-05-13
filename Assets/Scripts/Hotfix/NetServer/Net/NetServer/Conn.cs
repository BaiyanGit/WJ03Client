using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 连接类
/// </summary>
public class Conn
{
    public ByteArray readBuff = new ByteArray(1024 * 1024);

    /// <summary>
    /// 与客户端连接的套接字
    /// </summary>
    public Socket socket;

    /// <summary>
    /// 是否使用
    /// </summary>
    public bool isUse = false;

    /// <summary>
    /// 对应的player
    /// </summary>
    public Player player = null;

    //初始化
    public void Init(Socket socket)
    {
        this.socket = socket;
        isUse = true;
    }

    //获取客户端地址
    public string GetAdress()
    {
        if (!isUse)
        {
            return "无法获取地址";
        }

        return socket.RemoteEndPoint.ToString();
    }

    //关闭
    public void Close()
    {
        if (!isUse)
        {
            return;
        }

        RemovePlayer();

        //socket.Shutdown(SocketShutdown.Both);

        socket.Close();

        isUse = false;
        // 重新初始化数据缓存 否则二次分配conn的时候 数据拼接错误
        readBuff = new ByteArray(1024 * 1024);
    }

    public void RemovePlayer()
    {
        if (player != null)
        {
            PlayerManager.RemoveOnLinePlayer(player.id);
        }

        player = null;
    }

    /// <summary>
    /// 发送
    /// </summary>
    public void Send(MsgBase msgBase)
    {
        ServNet.Instance.Send(this, msgBase);
    }
}