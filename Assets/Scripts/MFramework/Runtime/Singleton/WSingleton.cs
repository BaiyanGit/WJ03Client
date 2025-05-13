using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wx.Runtime.Singleton
{
	public class WSingleton : WModule
	{
		private class Wrapper
		{
			public int Priority { private set; get; }
			public ISingleton Singleton { private set; get; }

			public Wrapper(ISingleton module, int priority)
			{
				Singleton = module;
				Priority = priority;
			}
		}
		
		private readonly List<Wrapper> _wrappers = new List<Wrapper>(100);
		private bool _isDirty = false;

        public override int Priority => 4;

        protected override void Awake()
        {
            base.Awake();
            WLog.Log($"{nameof(WSingleton)} initialize !");
        }
		
		public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
		{
			if (_isDirty)
			{
				_isDirty = false;
				_wrappers.Sort((left, right) =>
				{
					if (left.Priority > right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}

			// 轮询所有模块
			foreach (var wrapper in _wrappers)
			{
				wrapper.Singleton.OnUpdate();
			}
		}
		
		public override void OnFixedUpdate(float fixedDeltaTime,float unscaledFixedDeltaTime)
		{
			if (_isDirty)
			{
				_isDirty = false;
				_wrappers.Sort((left, right) =>
				{
					if (left.Priority > right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}

			// 轮询所有模块
			foreach (var wrapper in _wrappers)
			{
				wrapper.Singleton.OnFixedUpdate();
			}
		}
		
		public override void OnLateUpdate()
		{
			if (_isDirty)
			{
				_isDirty = false;
				_wrappers.Sort((left, right) =>
				{
					if (left.Priority > right.Priority)
						return -1;
					else if (left.Priority == right.Priority)
						return 0;
					else
						return 1;
				});
			}

			// 轮询所有模块
			foreach (var wrapper in _wrappers)
			{
				wrapper.Singleton.OnLateUpdate();
			}
		}

		public override void OnDestroy()
		{
			DestroyAll();
		}

		/// <summary>
		/// 获取单例
		/// </summary>
		public T GetSingleton<T>() where T : class, ISingleton
		{
			System.Type type = typeof(T);
			foreach (var t in _wrappers.Where(t => t.Singleton.GetType() == type))
			{
				return t.Singleton as T;
			}

            WLog.Error($"Not found manager : {type}");
			return null;
		}

		/// <summary>
		/// 查询单例是否存在
		/// </summary>
		public bool Contains<T>() where T : class, ISingleton
		{
			System.Type type = typeof(T);
			return _wrappers.Any(t => t.Singleton.GetType() == type);
		}

		/// <summary>
		/// 创建单例
		/// </summary>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public T CreateSingleton<T>(int priority = 0) where T : class, ISingleton
		{
			return CreateSingleton<T>(null, priority);
		}

		/// <summary>
		/// 创建单例
		/// </summary>
		/// <param name="createParam">附加参数</param>
		/// <param name="priority">运行时的优先级，优先级越大越早执行。如果没有设置优先级，那么会按照添加顺序执行</param>
		public T CreateSingleton<T>(System.Object createParam, int priority = 0) where T : class, ISingleton
		{
			if (priority < 0)
				throw new Exception("The priority can not be negative");

			if (Contains<T>())
			{
				DestroySingleton<T>();
			}
			
			// 如果没有设置优先级
			if (priority == 0)
			{
				var minPriority = GetMinPriority();
				priority = --minPriority;
			}

			var module = Activator.CreateInstance<T>();
			var wrapper = new Wrapper(module, priority);
			wrapper.Singleton.OnCreate(createParam);
			_wrappers.Add(wrapper);
			_isDirty = true;
			return module;
		}

		/// <summary>
		/// 销毁单例
		/// </summary>
		public bool DestroySingleton<T>() where T : class, ISingleton
		{
			var type = typeof(T);
			for (var i = 0; i < _wrappers.Count; i++)
			{
				if (_wrappers[i].Singleton.GetType() != type) continue;
				_wrappers[i].Singleton.OnDestroy();
				if (_wrappers[i].Singleton is SingletonInstance<T>)
				{
					var instance = _wrappers[i].Singleton as SingletonInstance<T>;
					instance?.DestroyInstance();
				}
				_wrappers.RemoveAt(i);
				return true;
			}
			return false;
		}

		private int GetMinPriority()
		{
			return _wrappers.Select(t => t.Priority).Prepend(0).Min();
		}
		private void DestroyAll()
		{
			foreach (var t in _wrappers)
			{
				t.Singleton.OnDestroy();
			}

			_wrappers.Clear();
		}
	}
}