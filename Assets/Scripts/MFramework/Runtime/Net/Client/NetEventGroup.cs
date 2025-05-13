using Google.Protobuf;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.Net
{
    public class NetEventGroup
    {
        private readonly Dictionary<System.Type, List<Action<IMessage>>> _cachedListener = new Dictionary<System.Type, List<Action<IMessage>>>();
        private readonly Dictionary<System.Type, ushort> _typePair = new Dictionary<System.Type, ushort>();
        /// <summary>
        /// 添加一个监听
        /// </summary>
        public void AddNetListener<TNet>(ushort mainId, System.Action<IMessage> listener) where TNet : IMessage
        {
            System.Type eventType = typeof(TNet);

            if (!_typePair.ContainsKey(eventType))
            {
                _typePair.Add(eventType, mainId);
            }

            if (!_cachedListener.ContainsKey(eventType))
                _cachedListener.Add(eventType, new List<Action<IMessage>>());

            if (!_cachedListener[eventType].Contains(listener))
            {
                _cachedListener[eventType].Add(listener);
                AppEntry.GetModule<WNet>().AddNetListener(mainId, eventType, listener);
            }
            else
            {
                WLog.Warning($"net listener is exist : {eventType}");
            }
        }

        /// <summary>
        /// 移除所有缓存的监听
        /// </summary>
        public void RemoveAllListener()
        {
            foreach (var pair in _cachedListener)
            {
                System.Type eventType = pair.Key;
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    AppEntry.GetModule<WNet>().RemoveNetListener(_typePair[eventType], pair.Value[i]);                   
                }
                _typePair.Remove(eventType);
                pair.Value.Clear();
            }
            _cachedListener.Clear();
        }

        public void RemoveNetListener<TNet>(System.Action<IMessage> listener) where TNet : IMessage
        {
            System.Type eventType = typeof(TNet);
            if (!_cachedListener.ContainsKey(eventType))
                WLog.Warning($"net listener is not exist : {eventType}");

            if (_cachedListener[eventType].Contains(listener))
            {
                _cachedListener[eventType].Remove(listener);
                AppEntry.GetModule<WNet>().RemoveNetListener(_typePair[eventType], listener);
                if(_cachedListener[eventType].Count == 0)
                {
                    _typePair.Remove(eventType);
                }
            }
            else
            {
                WLog.Warning($"net listener is not exist : {listener}");
            }
        }

        public void RemoveNetListener<TNet>() where TNet : IMessage
        {
            System.Type eventType = typeof(TNet);
            if (!_cachedListener.ContainsKey(eventType))
                WLog.Warning($"net listener is not exist : {eventType}");

            foreach(var listener in _cachedListener[eventType])
            {
                _cachedListener[eventType].Remove(listener);
                AppEntry.GetModule<WNet>().RemoveNetListener(_typePair[eventType], listener);
            }
            _typePair.Remove(eventType);
        }
    }
}
