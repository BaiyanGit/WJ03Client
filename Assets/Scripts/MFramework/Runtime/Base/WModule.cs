using UnityEngine;

namespace Wx.Runtime
{
    public abstract class WModule : MonoBehaviour
    {
        public virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        protected virtual void Awake()
        {
            AppEntry.RegisterModule(this);
        }

        public abstract void OnUpdate(float deltaTime, float unscaledDeltaTime);

        public virtual void OnFixedUpdate(float fixedDeltaTime, float unscaledFixedDeltaTime) { }

        public virtual void OnLateUpdate() { }

        public virtual void OnDestroy() { }
        
    }
}
