using System;
using System.Collections.Generic;

/// <summary>
/// 玩家管理类
/// </summary>
public class PlayerManager
{
    /// <summary>
    /// 当前在线玩家列表
    /// </summary>
    public static Dictionary<string, Player> onLinePlayers = new Dictionary<string, Player>();

    public static List<Player> playerList = new List<Player>();

    //玩家是否在线
    public static bool IsOnline(string id)
    {
        return onLinePlayers.ContainsKey(id);
    }

    /// <summary>
    /// 获取在线玩家
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Player GetPlayer(string id)
    {
        if (onLinePlayers.ContainsKey(id))
        {
            return onLinePlayers[id];
        }

        return null;
    }

    //添加玩家
    public static void AddPlayer(string id, Player player)
    {
        onLinePlayers.Add(id, player);
        playerList.Add(player);
    }

    //删除玩家
    public static void RemoveOnLinePlayer(string id)
    {
        if (!onLinePlayers.ContainsKey(id))
            return;
        
        // 只有用户已处于房间内才需要更新房间用户
        //if(onLinePlayers[id].roomId>0)
        //{
        //    RoomManager.UpdateRoomSite(id);
        //}
        
        
        playerList.Remove(onLinePlayers[id]);
        
        onLinePlayers.Remove(id);
    }
}