using UnityEngine;

namespace Wx.Runtime.UI
{
    public abstract class UIFormLogic : MonoBehaviour
    {
        public abstract void OnInit(IUIForm uiForm);

        public abstract void OnRecycle();

        public abstract void OnOpen();

        public abstract void OnClose();

        public abstract void OnPause();

        public abstract void OnResume();

        public abstract void OnCover();

        public abstract void OnReveal();

        public abstract void OnRefocus();

        public abstract void OnUpdate();

        public abstract void OnFixedUpdate();

        public abstract void OnDepthChanged(int uiGroupDepth, int depthInUIGroup);

        public abstract void SetMainFont(Object font);

        public abstract void PlayUIEffect(string soundAssetName);
        public abstract void StopUIEffect(string soundAssetName);


    }
}
