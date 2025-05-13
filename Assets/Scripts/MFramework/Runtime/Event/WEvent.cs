using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.Event
{
	public class WEvent : WModule
    {
		private readonly Dictionary<int, LinkedList<Action<IEventMessage>>> _listeners = new Dictionary<int, LinkedList<Action<IEventMessage>>>(1000);

        public override int Priority => 5;

        protected override void Awake()
        {
            base.Awake();
            WLog.Log($"{nameof(WEvent)} initialize !");
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            
        }

        /// <summary>
        /// 清空所有监听
        /// </summary>
        public void ClearAll()
		{
			foreach (int eventId in _listeners.Keys)
			{
				_listeners[eventId].Clear();
			}
			_listeners.Clear();
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		public void AddListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			System.Type eventType = typeof(TEvent);
			int eventId = eventType.GetHashCode();
			AddListener(eventId, listener);
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		public void AddListener(System.Type eventType, System.Action<IEventMessage> listener)
		{
			int eventId = eventType.GetHashCode();
			AddListener(eventId, listener);
		}

		/// <summary>
		/// 添加监听
		/// </summary>
		public void AddListener(int eventId, System.Action<IEventMessage> listener)
		{
			if (!_listeners.ContainsKey(eventId))
				_listeners.Add(eventId, new LinkedList<Action<IEventMessage>>());
			if (!_listeners[eventId].Contains(listener))
				_listeners[eventId].AddLast(listener);
		}


		/// <summary>
		/// 移除监听
		/// </summary>
		public void RemoveListener<TEvent>(System.Action<IEventMessage> listener) where TEvent : IEventMessage
		{
			System.Type eventType = typeof(TEvent);
			int eventId = eventType.GetHashCode();
			RemoveListener(eventId, listener);
		}

		/// <summary>
		/// 移除监听
		/// </summary>
		public void RemoveListener(System.Type eventType, System.Action<IEventMessage> listener)
		{
			int eventId = eventType.GetHashCode();
			RemoveListener(eventId, listener);
		}

		/// <summary>
		/// 移除监听
		/// </summary>
		public void RemoveListener(int eventId, System.Action<IEventMessage> listener)
		{
			if (_listeners.ContainsKey(eventId))
			{
				if (_listeners[eventId].Contains(listener))
					_listeners[eventId].Remove(listener);
			}
		}


		/// <summary>
		/// 实时广播事件
		/// </summary>
		public void SendMessage(IEventMessage message)
		{
			int eventId = message.GetType().GetHashCode();
			SendMessage(eventId, message);
		}

		/// <summary>
		/// 实时广播事件
		/// </summary>
		public void SendMessage(int eventId, IEventMessage message)
		{
			if (!_listeners.ContainsKey(eventId))
				return;

			LinkedList<Action<IEventMessage>> listeners = _listeners[eventId];
			if (listeners.Count > 0)
			{
				var currentNode = listeners.Last;
				while (currentNode != null)
				{
					currentNode.Value.Invoke(message);
					currentNode = currentNode.Previous;
				}
			}
		}
		

        

        

        
    }
}