using UnityEngine;

namespace Wx.Runtime.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region 局部变量
        private static T _Instance;
        #endregion
        #region 属性
        /// <summary>
        /// 获取单例对象
        /// </summary>
        public static T Instance
        {
            get
            {
                if (null == _Instance)
                {
                    _Instance = FindObjectOfType<T>();
                    if (null == _Instance)
                    {
                        GameObject go = new($"[{nameof(T)}]");
                        _Instance = go.AddComponent<T>();
                    }
                }
                return _Instance;
            }
        }
        #endregion
        #region 方法
        protected virtual void Awake()
        {
            if (null == _Instance)
            {
                _Instance = this as T;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}