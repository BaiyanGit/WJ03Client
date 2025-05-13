using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.Pool
{
    public class ObjectPool
    {
        private readonly GameObject _root;
        private readonly string _location;
        private readonly Queue<GameObject> _cacheEntitys;
        private readonly int _initCapacity;
        private readonly int _maxCapacity;
        private readonly float _releaseTimeInterval;
        private float _lastRestoreRealTime = -1f;
        private readonly bool _dontDestroy;
        private readonly PoolHelperBase _poolHelper;
        private GameObject _handle;

        /// <summary>
		/// 内部缓存总数
		/// </summary>
		public int CacheCount
        {
            get { return _cacheEntitys.Count; }
        }

        /// <summary>
        /// 外部使用总数
        /// </summary>
        public int SpawnCount { private set; get; } = 0;

        /// <summary>
        /// 是否常驻不销毁
        /// </summary>
        public bool DontDestroy
        {
            get { return _dontDestroy; }
        }

        public string Location
        {
            get { return _location; }
        }


        public ObjectPool(GameObject poolRoot, string location, bool dontDestroy, int initCapacity, int maxCapacity, float releaseTimeInterval, PoolHelperBase poolHelper)
        {
            _root = new GameObject($"[{location}]");
            _root.transform.SetParent(poolRoot.transform);
            _location = location;
            _dontDestroy = dontDestroy;
            _initCapacity = initCapacity;
            _maxCapacity = maxCapacity;
            _releaseTimeInterval = releaseTimeInterval;
            _poolHelper = poolHelper;

            _cacheEntitys = new Queue<GameObject>();    
        }

        public void CreatePoolSync()
        {
            _handle = _poolHelper.LoadEntitySync(Location);

            for(int i = 0;i< _initCapacity; i++)
            {
                var cache = _poolHelper.InstantiateHandleSync(_handle, _root.transform);
                cache.SetActive(false);
                _cacheEntitys.Enqueue(cache);
            }
        }

        public async UniTask CreatePoolAsync()
        {
            _handle = await _poolHelper.LoadEntityAsync(Location);

            for (int i = 0; i < _initCapacity; i++)
            {
                var cache = await _poolHelper.InstantiateHandleAsync(_handle, _root.transform);
                cache.SetActive(false);
                _cacheEntitys.Enqueue(cache);
            }
        }

        public void DestroyPool()
        {
            _handle = null;

            GameObject.Destroy(_root);
            _cacheEntitys.Clear();
        }

        public bool CanAutoDestroy()
        {
            if (_dontDestroy)
            {
                return false;
            }

            if(_releaseTimeInterval < 0)
            {
                return false;
            }

            if(_lastRestoreRealTime > 0 && SpawnCount <= 0)
            {
                return (Time.realtimeSinceStartup - _lastRestoreRealTime) > _releaseTimeInterval;
            }
            else
            {
                return false;
            }

        }

        public bool IsDestroyed()
        {
            return _handle == null;
        }

        public GameObject SpawnSync(Vector3 position, Quaternion rotation, bool forceClone, object userData)
        {
            GameObject cache;
            if (!forceClone && _cacheEntitys.Count > 0)
            {
                cache = _cacheEntitys.Dequeue();
            }
            else
            {
                cache = _poolHelper.InstantiateHandleSync(_handle, _root.transform);
            }

            SpawnCount++;

            cache.transform.SetPositionAndRotation(position, rotation);
            cache.SetActive(true);

            return cache;
        }

        public T SpawnSync<T>(Vector3 position,Quaternion rotation,bool forceClone,object userData) where T : MonoBehaviour,IObject
        {
            GameObject cache;
            if(!forceClone && _cacheEntitys.Count > 0)
            {
                cache = _cacheEntitys.Dequeue();
            }
            else
            {
                cache = _poolHelper.InstantiateHandleSync(_handle,_root.transform);
            }

            SpawnCount++;

            cache.transform.SetPositionAndRotation(position, rotation);
            cache.SetActive(true);

            T _t = cache.GetOrAddComponent<T>();
            _t.OnSpawn(userData);
            return _t;
        }

        public async UniTask<GameObject> SpawnAsync(Vector3 position, Quaternion rotation, bool forceClone, object userData)
        {
            GameObject cache;
            if (!forceClone && _cacheEntitys.Count > 0)
            {
                cache = _cacheEntitys.Dequeue();
            }
            else
            {
                cache = await _poolHelper.InstantiateHandleAsync(_handle, _root.transform);
            }

            SpawnCount++;

            cache.transform.SetPositionAndRotation(position, rotation);
            cache.SetActive(true);

            return cache;
        }

        public async UniTask<T> SpawnAsync<T>(Vector3 position, Quaternion rotation, bool forceClone, object userData) where T : MonoBehaviour, IObject
        {
            GameObject cache;
            if (!forceClone && _cacheEntitys.Count > 0)
            {
                cache = _cacheEntitys.Dequeue();
            }
            else
            {
                cache = await _poolHelper.InstantiateHandleAsync(_handle,_root.transform);
            }

            SpawnCount++;

            cache.transform.SetPositionAndRotation(position, rotation);
            cache.SetActive(true);

            T _t = cache.GetOrAddComponent<T>();
            _t.OnSpawn(userData);
            return _t;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Restore(GameObject entity)
        {
            if (entity == null) return;

            if (IsDestroyed())
            {
                GameObject.Destroy(entity);
                return;
            }

            SpawnCount--;
            if (SpawnCount <= 0)
            {
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            }

            if (_cacheEntitys.Count < _maxCapacity)
            {
                entity.gameObject.SetActive(false);
                entity.transform.SetParent(_root.transform);
                entity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _cacheEntitys.Enqueue(entity);
            }
            else
            {
                GameObject.Destroy(entity);
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Restore<T>(T entity) where T : MonoBehaviour, IObject
        {
            if (entity == null) return;

            entity.OnRestore();

            if (IsDestroyed())
            {
                GameObject.Destroy(entity.gameObject);
                return;
            }

            SpawnCount--; 
            if(SpawnCount <= 0)
            {
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            }

            if(_cacheEntitys.Count < _maxCapacity)
            {
                entity.gameObject.SetActive(false);
                entity.transform.SetParent(_root.transform);
                entity.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _cacheEntitys.Enqueue(entity.gameObject);
            }
            else
            {
                GameObject.Destroy(entity.gameObject);
            }
        }

        public void Discard(GameObject entity)
        {
            if (entity == null) return;

            if (IsDestroyed())
            {
                GameObject.Destroy(entity);
                return;
            }

            SpawnCount--;
            if (SpawnCount <= 0)
            {
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            }

            GameObject.Destroy(entity);
        }

        /// <summary>
        /// 丢弃
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Discard<T>(T entity) where T : MonoBehaviour, IObject
        {
            if (entity == null) return;

            entity.OnRestore();

            if (IsDestroyed())
            {
                GameObject.Destroy(entity.gameObject);
                return;
            }

            SpawnCount--;
            if (SpawnCount <= 0)
            {
                _lastRestoreRealTime = Time.realtimeSinceStartup;
            }

            GameObject.Destroy(entity.gameObject);
        }

    }
}
