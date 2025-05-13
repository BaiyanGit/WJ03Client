using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using RainbowArt.CleanFlatUI;

namespace Hotfix.UI
{
    /// <summary>
    /// 弹出提示界面
    /// </summary>
    public class UIPopTip : UIForm<UIViewPopTip, UIModelPopTip>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private UIViewPopTip _view;
        private UIModelPopTip _model;

        #endregion

        #region Private

        private ModalWindow _modalWindow;
        private UIEventDefine.UIPopTipCall _message;

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
            InitUIListener();
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            _message = (UIEventDefine.UIPopTipCall)userData;
            ResetUIComponent();

            PlayUIEffect(AppConst.AssetPathConst.WarningSound);
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
        }

        #region UI

        private void InitUIComponent()
        {
            _modalWindow = _view.tsWindow.GetComponent<ModalWindow>();
        }

        private void ResetUIComponent()
        {
            _modalWindow.TitleValue = _message.title;
            _modalWindow.DescriptionValue = _message.content;
            _modalWindow.OnConfirm.RemoveAllListeners();
            _modalWindow.OnConfirm.AddListener(() =>
            {
                OnCloseHandle();
                _message.confirm?.Invoke();
            });
            _modalWindow.OnCancel.RemoveAllListeners();
            _modalWindow.OnCancel.AddListener(() =>
            {
                OnCloseHandle();
                _message.cancel?.Invoke();
            });
            _modalWindow.ShowModalWindow();
        }

        private void InitUIListener()
        {
            _modalWindow.OnCancel.AddListener(OnCloseHandle);
        }

        private void OnCloseHandle()
        {
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);
            _uGuiForm.Close();
        }

        #endregion

    }
}
