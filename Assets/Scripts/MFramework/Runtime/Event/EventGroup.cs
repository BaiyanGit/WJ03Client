using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.Event
{
	public class EventGroup
	{
		private readonly Dictionary<System.Type, List<Action<IEventMessage>>> _cachedListener = new Dictionary<System.Type, List<Action<IEventMessage>>>();

		/// <summary>
		/// 添加一个监听
		/// </summary>
		public void AddListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			System.Type eventType = typeof(TEvent);
			if (!_cachedListener.ContainsKey(eventType))
				_cachedListener.Add(eventType, new List<Action<IEventMessage>>());

			if (!_cachedListener[eventType].Contains(listener))
			{
				_cachedListener[eventType].Add(listener);
				AppEntry.GetModule<WEvent>().AddListener(eventType, listener);
			}
			else
			{
				WLog.Warning($"Event listener is exist : {eventType}");
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
                    AppEntry.GetModule<WEvent>().RemoveListener(eventType, pair.Value[i]);
				}
				pair.Value.Clear();
			}
			_cachedListener.Clear();
		}

		public void RemoveListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            System.Type eventType = typeof(TEvent);
            if (!_cachedListener.ContainsKey(eventType))
                WLog.Warning($"Event listener is not exist : {eventType}");

            if (_cachedListener[eventType].Contains(listener))
            {
                _cachedListener[eventType].Remove(listener);
                AppEntry.GetModule<WEvent>().RemoveListener(eventType, listener);
            }
            else
            {
                WLog.Warning($"Event listener is not exist : {listener}");
            }
        }
	}
}