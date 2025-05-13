using System;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using Wx.Runtime.Event;
using RainbowArt.CleanFlatUI;
using Object = UnityEngine.Object;

namespace Hotfix.UI
{
    /// <summary>
    /// 加载界面
    /// </summary>
    public class UILoading : UIForm<UIViewLoading, UIModelLoading>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private readonly EventGroup _eventGroup = new();
        private UIViewLoading _view;
        private UIModelLoading _model;

        #endregion

        #region UIComponent

        private ProgressBar _progressBarBubble;

        #endregion

        #region Private

        private Action _cancel;
        private Action _confirm;

        #endregion



        public override UIGroupInfo SetUIGroupInfo()
        {
            return base.SetUIGroupInfo();
        }

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitUIComponent();
            InitBtnListener();

            showPauseAndResumeAction = true;
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userDatas)
        {
            base.OnOpen(userDatas);

            InitEventListener();
        }


        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override void OnCover()
        {
            base.OnCover();
        }

        public override void OnReveal()
        {
            base.OnReveal();
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);
        }

        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);
            _eventGroup.RemoveAllListener();
        }

        #region UI

        private void InitUIComponent()
        {
            _progressBarBubble = _view.tsProgressBarBubble.GetComponent<ProgressBar>();
        }

        private void InitBtnListener()
        {
            _view.btnCancel.onClick.AddListener(OnCancelHandle);
            _view.btnConfirm.onClick.AddListener(OnConfirmHandle);
        }

        private void OnConfirmHandle()
        {
            _confirm?.Invoke();
        }

        private void OnCancelHandle()
        {
            _cancel?.Invoke();
        }

        #endregion

        #region EventHandle

        private void InitEventListener()
        {
            _eventGroup.AddListener<UIEventDefine.UILoadingShowPro>(OnShowProHandle);
            _eventGroup.AddListener<UIEventDefine.UILoadingUpdatePro>(OnUpdateProHandle);
            _eventGroup.AddListener<UIEventDefine.UILoadingShowWindow>(OnShowWindowHandle);
        }

        private void OnShowProHandle(IEventMessage msg)
        {
            var message = (UIEventDefine.UILoadingShowPro)msg;

            _view.tsProgressBarCircularLoop.gameObject.SetActive(message.showIndex == 0);
            _view.tsProgressBarBubble.gameObject.SetActive(message.showIndex == 1);
            _view.tsWindow.gameObject.SetActive(false);
        }

        private void OnUpdateProHandle(IEventMessage msg)
        {
            var message = (UIEventDefine.UILoadingUpdatePro)msg;
            _progressBarBubble.CurrentValue = message.progress;
        }

        private void OnShowWindowHandle(IEventMessage msg)
        {
            var message = (UIEventDefine.UILoadingShowWindow)msg;

            _cancel = message.cancel;
            _confirm = message.confirm;

            _view.tmptxtWindowTitle.text = message.title;
            _view.tmptxtContent.text = message.content;
            _view.tmptxtCancel.text = message.cancelText;
            _view.tmptxtConfirm.text = message.confirmText;

            _view.tsWindow.gameObject.SetActive(true);
        }

        #endregion


    }
}
