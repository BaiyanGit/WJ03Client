using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgBase
{
    public string protoName = "null";

    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] Encode(MsgBase msgBase)
    {
        string s = JsonUtility.ToJson(msgBase);
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="protoName"></param>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);

        MsgBase msgBase = (MsgBase)JsonUtility.FromJson(s, Type.GetType(protoName));

        return msgBase;
    }


    /// <summary>
    /// 编码协议名（2字节长度+字符串）
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] EncodeName(MsgBase msgBase)
    {
        //名字bytes和长度
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
        Int16 len = (Int16)nameBytes.Length;
        //申请bytes数值
        byte[] bytes = new byte[2 + len];
        //组装2字节的长度信息
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        //组装名字bytes
        Array.Copy(nameBytes, 0, bytes, 2, len);

        return bytes;
    }

    /// <summary>
    /// 解码协议名（2字节长度+字符串）
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        //必须大于2字节
        if (offset + 2 > bytes.Length)
        {
            return "";
        }

        //读取长度
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        //长度必须足够
        if (offset + 2 + len > bytes.Length)
        {
            return "";
        }

        //解析
        count = 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
        return name;
    }
}