using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 控制层消息中心
/// </summary>
public class Ctrl_MessageCenter
{
    // <summary>
    /// 无参消息缓存集合
    /// </summary>
    private static Dictionary<string, List<Delegate>> m_dicMessages0 = new Dictionary<string, List<Delegate>>();

    /// <summary>
    /// 带有一个参数消息缓存集合
    /// </summary>
    private static Dictionary<string, List<Delegate>> m_dicMessages1 = new Dictionary<string, List<Delegate>>();

    /// <summary>
    /// 带有二个参数消息缓存集合
    /// </summary>
    private static Dictionary<string, List<Delegate>> m_dicMessages2 = new Dictionary<string, List<Delegate>>();

    /// <summary>
    /// 带有三个参数消息缓存集合
    /// </summary>
    private static Dictionary<string, List<Delegate>> m_dicMessages3 = new Dictionary<string, List<Delegate>>();

    /// <summary>
    /// 增加消息的监听（无参）
    /// </summary>
    public static void AddMsgListener(string message, Action handler)
    {
        lock (m_dicMessages0)
        {
            if (!m_dicMessages0.ContainsKey(message))
            {
                m_dicMessages0.Add(message, new List<Delegate>());
            }

            if (m_dicMessages0[message].Contains(handler))
            {
#if UNITY_EDITOR
                Debug.LogWarning("该事件已经注册:  " + "事件type" + message.ToString() + "callback:" + handler.ToString());
#endif
            }
            else
            {
                m_dicMessages0[message].Add(handler);
            }
        }
    }

    /// <summary>
    /// 增加消息的监听（1参）
    /// </summary>
    public static void AddMsgListener<T>(string message, Action<T> handler)
    {
        lock (m_dicMessages1)
        {
            if (!m_dicMessages1.ContainsKey(message))
            {
                m_dicMessages1.Add(message, new List<Delegate>());
            }

            if (m_dicMessages1[message].Contains(handler))
            {
#if UNITY_EDITOR
                Debug.LogWarning("该事件已经注册:  " + "事件type" + message.ToString() + "callback:" + handler.ToString());
#endif
            }
            else
            {
                m_dicMessages1[message].Add(handler);
            }
        }
    }

    /// <summary>
    /// 增加消息的监听（2参）
    /// </summary>
    public static void AddMsgListener<T1, T2>(string message, Action<T1, T2> handler)
    {
        lock (m_dicMessages2)
        {
            if (!m_dicMessages2.ContainsKey(message))
            {
                m_dicMessages2.Add(message, new List<Delegate>());
            }

            if (m_dicMessages2[message].Contains(handler))
            {
#if UNITY_EDITOR
                Debug.LogWarning("该事件已经注册:  " + "事件type" + message.ToString() + "callback:" + handler.ToString());
#endif
            }
            else
            {
                m_dicMessages2[message].Add(handler);
            }
        }
    }

    /// <summary>
    /// 增加消息的监听（3参）
    /// </summary>
    public static void AddMsgListener<T1, T2, T3>(string message, Action<T1, T2, T3> handler)
    {
        lock (m_dicMessages3)
        {
            if (!m_dicMessages3.ContainsKey(message))
            {
                m_dicMessages3.Add(message, new List<Delegate>());
            }

            if (m_dicMessages3[message].Contains(handler))
            {
#if UNITY_EDITOR
                Debug.LogWarning("该事件已经注册:  " + "事件type" + message.ToString() + "callback:" + handler.ToString());
#endif
            }
            else
            {
                m_dicMessages3[message].Add(handler);
            }
        }
    }

    /// <summary>
    /// 移除消息的监听(无参)
    /// </summary>
    public static void RemoveMsgListener(string message, Action handler)
    {
        lock (m_dicMessages0)
        {
            if (m_dicMessages0.ContainsKey(message))
            {
                if (m_dicMessages0[message].Contains(handler))
                {
                    m_dicMessages0[message].Remove(handler);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning("该事件尚未注册：  " + "事件ype:" + message + "callback:" + handler.ToString());
#endif
                }
            }
        }
    }

    /// <summary>
    /// 移除消息的监听(1参)
    /// </summary>
    public static void RemoveMsgListener<T>(string message, Action<T> handler)
    {
        lock (m_dicMessages1)
        {
            if (m_dicMessages1.ContainsKey(message))
            {
                if (m_dicMessages1[message].Contains(handler))
                {
                    m_dicMessages1[message].Remove(handler);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning("该事件尚未注册：  " + "事件ype:" + message + "callback:" + handler.ToString());
#endif
                }
            }
        }
    }

    /// <summary>
    /// 移除消息的监听(2参)
    /// </summary>
    public static void RemoveMsgListener<T1, T2>(string message, Action<T1, T2> handler)
    {
        lock (m_dicMessages2)
        {
            if (m_dicMessages2.ContainsKey(message))
            {
                if (m_dicMessages2[message].Contains(handler))
                {
                    m_dicMessages2[message].Remove(handler);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning("该事件尚未注册：  " + "事件ype:" + message + "callback:" + handler.ToString());
#endif
                }
            }
        }
    }

    /// <summary>
    /// 移除消息的监听(3参)
    /// </summary>
    public static void RemoveMsgListener<T1, T2, T3>(string message, Action<T1, T2, T3> handler)
    {
        lock (m_dicMessages3)
        {
            if (m_dicMessages3.ContainsKey(message))
            {
                if (m_dicMessages3[message].Contains(handler))
                {
                    m_dicMessages3[message].Remove(handler);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogWarning("该事件尚未注册：  " + "事件ype:" + message + "callback:" + handler.ToString());
#endif
                }
            }
        }
    }

    /// <summary>
    /// 发送消息(无参)
    /// </summary>
    public static void SendMessage(string message)
    {
        lock (m_dicMessages0)
        {
            if (m_dicMessages0.ContainsKey(message))
            {
                for (int i = 0; i < m_dicMessages0[message].Count; i++)
                {
                    Action callback = (Action)m_dicMessages0[message][i];
                    if (callback != null)
                    {
                        callback();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 发送消息(1参)
    /// </summary>
    public static void SendMessage<T>(string message, T arg)
    {
        lock (m_dicMessages1)
        {
            //try
            //{
            if (m_dicMessages1.ContainsKey(message))
            {
                for (int i = 0; i < m_dicMessages1[message].Count; i++)
                {
                    Action<T> callback = (Action<T>)m_dicMessages1[message][i];
                    if (callback != null)
                    {
                        callback(arg);
                    }
                }
            }
            //}
            //catch
            //{
            //    Debug.Log("DSB");
            //}
        }
    }

    /// <summary>
    /// 发送消息(2参)
    /// </summary>
    public static void SendMessage<T1, T2>(string message, T1 arg1, T2 arg2)
    {
        lock (m_dicMessages2)
        {
            if (m_dicMessages2.ContainsKey(message))
            {
                for (int i = 0; i < m_dicMessages2[message].Count; i++)
                {
                    Action<T1, T2> callback = (Action<T1, T2>)m_dicMessages2[message][i];
                    if (callback != null)
                    {
                        callback(arg1, arg2);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 发送消息(3参)
    /// </summary>
    public static void SendMessage<T1, T2, T3>(string message, T1 arg1, T2 arg2, T3 arg3)
    {
        lock (m_dicMessages3)
        {
            if (m_dicMessages3.ContainsKey(message))
            {
                for (int i = 0; i < m_dicMessages3[message].Count; i++)
                {
                    Action<T1, T2, T3> callback = (Action<T1, T2, T3>)m_dicMessages3[message][i];
                    if (callback != null)
                    {
                        callback(arg1, arg2, arg3);
                    }
                }
            }
        }
    }
}