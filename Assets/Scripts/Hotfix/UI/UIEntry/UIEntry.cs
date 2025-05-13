using GameMain.Runtime.UI;
using Hotfix.Event;
using UnityEngine;
using Wx.Runtime.Event;
using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 初始界面
    /// </summary>
    public class UIEntry : UIForm<UIViewEntry, UIModelEntry>
    {
        private UGuiForm _uGuiForm;
        
        #region Component
        
        private UIViewEntry _view;
        private UIModelEntry _model;

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

            InitButtonListener();
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
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

        private void InitButtonListener()
        {
            _view.btnSimulate.onClick.AddListener(OnSimulateHandle);
            _view.btnNet.onClick.AddListener(OnNetHandle);
        }

        private void OnNetHandle()
        {
            UIEventDefine.UIEntrySelectModel.SendMessage(1);
            _uGuiForm.Close();
        }

        private void OnSimulateHandle()
        {
            UIEventDefine.UIEntrySelectModel.SendMessage(0);
            _uGuiForm.Close();
        }
    }
}
