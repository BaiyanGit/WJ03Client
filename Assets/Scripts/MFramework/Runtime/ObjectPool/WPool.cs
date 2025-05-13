using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wx.Runtime.Pool
{
	/// <summary>
	/// 游戏对象池系统
	/// </summary>
	public class WPool : WModule
    {
        [SerializeField]
        private string mPoolHelperTypeName = "Wx.Runtime.DefaultPoolHelper";
        [SerializeField]
        private PoolHelperBase mCustomPoolHelper = null;
        [SerializeField]
        private PoolHelperBase poolHelper = null;

        public override int Priority => 10;

        private readonly List<ObjectPool> _objectPools = new();
        private readonly List<ObjectPool> _removeList = new();

        private GameObject _spawnerRoot;


        protected override void Awake()
        {
            base.Awake();

            poolHelper = Helper.CreateHelper(mPoolHelperTypeName, mCustomPoolHelper);
            if(poolHelper == null)
            {
                WLog.Log("Can not create pool helper");
            }

            poolHelper.name = "PoolHelper";
            var thisTransform = poolHelper.transform;
            thisTransform.SetParent(this.transform);
            thisTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            thisTransform.localScale = Vector3.one;

            _spawnerRoot = this.gameObject;

            WLog.Log($"{nameof(WPool)} initialize !");
        }
        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            _removeList.Clear();
            foreach (var pool in _objectPools.Where(pool => pool.CanAutoDestroy()))
            {
                _removeList.Add(pool);
            }

            foreach(var pool in _removeList)
            {
                _objectPools.Remove(pool);
                pool.DestroyPool();
            }
        }

        /// <summary>
		/// 销毁所有对象池及其资源
		/// </summary>
		/// <param name="includeAll">销毁所有对象池，包括常驻对象池</param>
        public void DestroyAll(bool includeAll)
        {
            if (includeAll)
            {
                foreach(var pool in _objectPools)
                {
                    pool.DestroyPool();
                }
                _objectPools.Clear();
            }
            else
            {
                List<ObjectPool> removeList = new List<ObjectPool>();   
                foreach(var pool in _objectPools)
                {
                    if (!pool.DontDestroy)
                    {
                        removeList.Add(pool);
                    }
                }
                foreach(var pool in removeList)
                {
                    _objectPools.Remove(pool);
                    pool.DestroyPool();
                }
            }
        }

        private ObjectPool TryGetObjectPool(string location)
        {
            foreach(var pool in _objectPools)
            {
                if(pool.Location == location)
                {
                    return pool;
                }
            }
            return null;
        }

        public ObjectPool CreateObjectPoolSync(string location, GameObject root = null, bool dontDestroy = false,int initCapacity = 0,int maxCapacity = int.MaxValue,float releaseTimeInterval = -1f)
        {
            if(maxCapacity < initCapacity)
            {
                throw new Exception($"The maxCapacity value: {maxCapacity} must be greater the init Capcaity value : {initCapacity}");
            }
            ObjectPool pool = TryGetObjectPool(location);
            if(pool != null)
            {
                WLog.Warning($"ObjectPool is already existed : {location}");
            }
            else
            {
                if(root == null)
                {
                    root = _spawnerRoot;
                }
                pool = new ObjectPool(root, location, dontDestroy, initCapacity, maxCapacity, releaseTimeInterval, poolHelper);
                pool.CreatePoolSync();
                _objectPools.Add(pool);
            }

            return pool;
        }

        public async UniTask<ObjectPool> CreateObjectPoolAsync(string location, GameObject root = null, bool dontDestroy = false, int initCapacity = 0, int maxCapacity = int.MaxValue, float releaseTimeInterval = -1f)
        {
            if (maxCapacity < initCapacity)
            {
                throw new Exception($"The maxCapacity value: {maxCapacity} must be greater the init Capcaity value : {initCapacity}");
            }
            ObjectPool pool = TryGetObjectPool(location);
            if (pool != null)
            {
                WLog.Warning($"ObjectPool is already existed : {location}");
            }
            else
            {
                if (root == null)
                {
                    root = _spawnerRoot;
                }
                pool = new ObjectPool(root, location, dontDestroy, initCapacity, maxCapacity, releaseTimeInterval, poolHelper);
                await pool.CreatePoolAsync();
                _objectPools.Add(pool);
            }

            return pool;
        }


        public T SpawnSync<T>(string location, Vector3 position, Quaternion rotation, bool forceClone) where T : MonoBehaviour, IObject
        {
            return SpawnSyncInternal<T>(location, position, rotation, forceClone, null);
        }

        public T SpawnSync<T>(string location, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IObject
        {
            return SpawnSyncInternal<T>(location, position, rotation, false, null);
        }

        public T SpawnSync<T>(string location, Vector3 position, Quaternion rotation, object userData) where T : MonoBehaviour, IObject
        {
            return SpawnSyncInternal<T>(location, position, rotation, false, userData);
        }

        public T SpawnSync<T>(string location, bool forceClone, object userData = null) where T : MonoBehaviour, IObject
        {
            return SpawnSyncInternal<T>(location, Vector3.zero, Quaternion.identity, forceClone, userData);
        }

        public T SpawnSync<T>(string location) where T : MonoBehaviour, IObject
        {
            return SpawnSyncInternal<T>(location, Vector3.zero, Quaternion.identity, false, null);
        }

        public T SpawnSync<T>(string location, object userData) where T : MonoBehaviour, IObject
        {
            return SpawnSyncInternal<T>(location, Vector3.zero, Quaternion.identity, false, userData);
        }


        public async UniTask<T> SpawnAsync<T>(string location, Vector3 position, Quaternion rotation, bool forceClone) where T : MonoBehaviour, IObject
        {
            return await SpawnAsyncInternal<T>(location,position, rotation, forceClone, null);
        }

        public async UniTask<T> SpawnAsync<T>(string location, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IObject
        {
            return await SpawnAsyncInternal<T>(location, position, rotation, false, null);
        }

        public async UniTask<T> SpawnAsync<T>(string location, Vector3 position, Quaternion rotation, object userData) where T : MonoBehaviour, IObject
        {
            return await SpawnAsyncInternal<T>(location, position, rotation, false, userData);
        }

        public async UniTask<T> SpawnAsync<T>(string location, bool forceClone, object userData = null) where T : MonoBehaviour, IObject
        {
            return await SpawnAsyncInternal<T>(location, Vector3.zero, Quaternion.identity, forceClone, userData);
        }

        public async UniTask<T> SpawnAsync<T>(string location) where T : MonoBehaviour, IObject
        {
            return await SpawnAsyncInternal<T>(location,Vector3.zero, Quaternion.identity, false, null);
        }

        public async UniTask<T> SpawnAsync<T>(string location, object userData) where T : MonoBehaviour, IObject
        {
            return await SpawnAsyncInternal<T>(location, Vector3.zero, Quaternion.identity, false, userData);
        }

        public GameObject SpawnSync(string location, object userData = null)
        {
            return SpawnSyncInternal(location, Vector3.zero, Quaternion.identity, false, userData);
        }

        public GameObject SpawnSync(string location, Vector3 position, Quaternion rotation, object userDate = null)
        {
            return SpawnSyncInternal(location, position, rotation, false, userDate);
        }

        private GameObject SpawnSyncInternal(string location, Vector3 position, Quaternion rotation , bool forceClone, object userData)
        {
            ObjectPool pool = TryGetObjectPool(location);
            if (pool == null)
            {
                pool = CreateObjectPoolSync(location);
                pool.CreatePoolSync();
                _objectPools.Add(pool);
            }

            return pool.SpawnSync(position, rotation, forceClone, userData);
        }

        public async UniTask<GameObject> SpawnAsync(string location, object userData = null)
        {
            return await SpawnAsyncInternal(location, Vector3.zero, Quaternion.identity, false, userData);
        }

        public async UniTask<GameObject> SpawnAsync(string location, Vector3 position, Quaternion rotation, object userDate = null)
        {
            return await SpawnAsyncInternal(location, position, rotation, false, userDate);
        }

        private async UniTask<GameObject> SpawnAsyncInternal(string location, Vector3 position, Quaternion rotation, bool forceClone, object userData)
        {
            ObjectPool pool = TryGetObjectPool(location);
            if (pool == null)
            {
                pool = await CreateObjectPoolAsync(location);
                await pool.CreatePoolAsync();
                _objectPools.Add(pool);
            }

            return await pool.SpawnAsync(position, rotation, forceClone, userData);
        }

        private T SpawnSyncInternal<T>(string location, Vector3 position,Quaternion rotation,bool forceClone,object userData) where T : MonoBehaviour,IObject
        {
            ObjectPool pool = TryGetObjectPool(location);
            if (pool == null)
            {
                pool = CreateObjectPoolSync(location);
                pool.CreatePoolSync();
                _objectPools.Add(pool);
            }

            return pool.SpawnSync<T>(position, rotation, forceClone, userData);
        }

        private async UniTask<T> SpawnAsyncInternal<T>(string location,  Vector3 position, Quaternion rotation, bool forceClone, object userData) where T : MonoBehaviour, IObject
        {
            ObjectPool pool = TryGetObjectPool(location);
            if (pool == null)
            {
                pool = await CreateObjectPoolAsync(location);
                await pool.CreatePoolAsync();
                _objectPools.Add(pool);
            }

            return await pool.SpawnAsync<T>(position, rotation, forceClone, userData);
        }

        public void Restore(string location, GameObject entity)
        {
            ObjectPool objectPool = TryGetObjectPool(location);
            if (objectPool == null)
            {
                throw new Exception($"Can not get objectPool : {location}");
            }
            objectPool.Restore(entity);
        }

        public void Restore<T>(string location, T entity) where T : MonoBehaviour, IObject
        {
            ObjectPool objectPool = TryGetObjectPool(location);
            if(objectPool == null)
            {
                throw new Exception($"Can not get objectPool : {location}");
            }
            objectPool.Restore(entity);
        }

        public void Discard(string location, GameObject entity)
        {
            ObjectPool objectPool = TryGetObjectPool(location);
            if (objectPool == null)
            {
                throw new Exception($"Can not get objectPool : {location}");
            }
            objectPool.Discard(entity);
        }

        public void Discard<T>(string location, T entity) where T : MonoBehaviour, IObject
        {
            ObjectPool objectPool = TryGetObjectPool(location);
            if (objectPool == null)
            {
                throw new Exception($"Can not get objectPool : {location}");
            }
            objectPool.Discard(entity);
        }

    }
}