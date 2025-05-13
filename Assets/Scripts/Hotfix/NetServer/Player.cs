using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家类
/// </summary>
public class Player
{
    /// <summary>
    /// 玩家账号
    /// </summary>
    public string id;

    /// <summary>
    /// 玩家姓名
    /// </summary>
    public string name = "";

    /// <summary>
    /// -1代表不在房间里
    /// </summary>
    public int roomId = -1;

    /// <summary>
    /// 机器型号的id
    /// </summary>
    public int machineTypeId;

    /// <summary>
    /// 是否准备了
    /// </summary>
    public bool hasReady;

    /// <summary>
    /// 客户端场景加载成功
    /// </summary>
    public bool loadSuccess;

    /// <summary>
    /// 本次信息已上传
    /// </summary>
    public bool hasSync;

    public Conn conn;

    public bool isOwner;

    public Player(string id)
    {
        this.id = id;
    }

    /// <summary>
    /// 发送
    /// </summary>
    public void Send(MsgBase msgBase)
    {
        ServNet.Instance.Send(conn, msgBase);
    }
}


/// <summary>
/// 玩家信息类
/// </summary>
[System.Serializable]
public class PlayerInfo
{
    /// <summary>
    /// 账号id
    /// </summary>
    public string accountId;

    /// <summary>
    /// 玩家名字
    /// </summary>
    public string playerName;

    /// <summary>
    /// 机器设备的id
    /// </summary>
    public int machineId;

    /// <summary>
    /// 0不是房主 1是房主
    /// </summary>
    public int isOwner = 0;

    /// <summary>
    /// 是否准备了
    /// </summary>
    public bool hasReady;
}