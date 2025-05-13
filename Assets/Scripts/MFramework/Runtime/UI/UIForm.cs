
using UnityEngine;
using Wx.Runtime.Sound;

namespace Wx.Runtime.UI
{
    public class UIForm<TV,TM> : IUIForm where TV :class,IUIView where TM : class,IUIModel
    {
        private string _mUIFromAssetName;
        private UIGroup _mUIGroup;
        private int _mDepthInUIGroup;
        private bool _mPauseCoveredUIForm;

        private int _mOriginalLayer = 0;
        private bool _mAvailable = false;
        private bool _mVisible = false;

        private bool _mPaused = false;
        private bool _mCovered = false;
        
        public string UIFormAssetName => _mUIFromAssetName;

        public UIFormLogic Handle
        {
            get;
            set;
        }

        public UIGroup UIGroup => _mUIGroup;

        public int DepthInUIGroup => _mDepthInUIGroup;

        public bool PauseCoveredUIForm => _mPauseCoveredUIForm;


        public bool Available
        {
            get
            {
                return _mAvailable;
            }
        }

        /// <summary>
        /// 获取或设置界面是否可见。
        /// </summary>
        public bool Visible
        {
            get
            {
                return _mAvailable && _mVisible;
            }
            set
            {
                if (!_mAvailable)
                {
                    WLog.Warning($"UI form '{_mUIFromAssetName}' is not available.");
                    return;
                }

                if (_mVisible == value)
                {
                    return;
                }

                _mVisible = value;
                InternalSetVisible(value);
            }
        }

        public bool Paused
        {
            get => _mPaused;
            set => _mPaused = value;
        }

        public bool Covered
        {
            get => _mCovered; 
            set => _mCovered = value;
        }

        public IUIView UIView
        {
            get; 
            set;
        }

        public IUIModel UIModel 
        {
            get; 
            set; 
        }

        public TV GetView()
        {
            return UIView as TV;
        }

        public TM GetModel()
        {
            return UIModel as TM;
        }

        protected bool showPauseAndResumeAction;
        protected bool showOpenAndCloseAction;

        public virtual UIGroupInfo SetUIGroupInfo()
        {
            return UIGroupInfo.Normal;
        }


        public virtual void OnInit(string uiFormAssetName,UIGroup uiGroup, bool pauseCoveredUIForm,UIFormLogic handle)
        {
            _mUIFromAssetName = uiFormAssetName;
            _mUIGroup = uiGroup;
            _mPauseCoveredUIForm = pauseCoveredUIForm;
            _mDepthInUIGroup = 0;
            Handle = handle != null ? handle : Handle;

            UIView.Init(Handle.gameObject);
            Handle.OnInit(this);

            showPauseAndResumeAction = false;
            showOpenAndCloseAction = true;
        }

        public virtual void OnRecycle()
        {
            _mDepthInUIGroup = 0;
            _mPauseCoveredUIForm = true;
            Handle.OnRecycle();
        }


        public virtual void OnOpen(object userData)
        {
            _mOriginalLayer = Handle.gameObject.layer;
            _mAvailable = true;
            Visible = true;
            _mPaused = false;
            _mCovered = false;

            if (!showOpenAndCloseAction) return;
            Handle.OnOpen();
        }

        public virtual void Close(object userData = null)
        {
            Handle.gameObject.SetLayerRecursively(_mOriginalLayer);
            Visible = false;
            _mAvailable = false;
            _mPaused = false;
            _mCovered = false;
            
            if (!showOpenAndCloseAction) return;
            Handle.OnClose();
        }

        public virtual void OnPause()
        {
            if (!showPauseAndResumeAction) return;
            Visible = false;
            Handle.OnPause();
        }


        public virtual void OnResume()
        {
            if (!showPauseAndResumeAction) return;
            Visible = true;
            Handle.OnResume();
        }

        public virtual void OnCover()
        {
            Handle.OnCover();
        }


        public virtual void OnReveal()
        {
            Handle.OnReveal();
        }

        public virtual void OnRefocus(object userData = null)
        {
            Handle.OnRefocus();
        }


        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Handle.OnUpdate();
        }

        public virtual void OnFixedUpdate()
        {
            Handle.OnFixedUpdate();
        }


        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            Handle.OnDepthChanged(uiGroupDepth, depthInUIGroup);    
        }

        public void SetMainFont(Object font)
        {
            Handle.SetMainFont(font);
        }

        public void PlayUIEffect(string soundAssetName)
        {
            Handle.PlayUIEffect(soundAssetName);
        }

        public void StopUIEffect(string soundAssetName)
        {
            Handle.StopUIEffect(soundAssetName);
        }

        protected virtual void InternalSetVisible(bool visible)
        {
            Handle.gameObject.SetActive(visible);
        }


    }
}
