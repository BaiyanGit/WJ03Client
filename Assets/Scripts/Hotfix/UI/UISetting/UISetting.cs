using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Hotfix.Event;
using UnityEngine.SceneManagement;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UISetting : UIForm<UIViewSetting, UIModelSetting>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private UIViewSetting _view;
        private UIModelSetting _model;

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
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            ShowCommonPage();
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

            ShowCommonPage();
        }

        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);
        }


        private void InitUIComponent()
        {
            //Materials
            _view.btnCamouflage.onClick.AddListener(OnCamouflageClick/*() => { GameManager.Instance.ChangeMaterial(0); }*/);
            _view.btnGreenishWhite.onClick.AddListener(() => { GameManager.Instance.ChangeMaterial("cheTi_02"); });

            //Audio
            _view.btnDown.onClick.AddListener(OnAudioDownClick);
            _view.btnUp.onClick.AddListener(OnAudioUpClick);

            //Quality
            _view.togPreformant.onValueChanged.AddListener(OnGroupValueChange);
            _view.togBalanced.onValueChanged.AddListener(OnGroupValueChange);
            _view.togHighFidelity.onValueChanged.AddListener(OnGroupValueChange);
        }

        private void OnCamouflageClick()
        {
            //Debug.Log("____________");
            GameManager.Instance.ChangeMaterial("cheTi_01");
        }

        private void OnAudioDownClick()
        {
            _view.audioSlider.value -= .1f;
            GameManager.Instance.ChangeAudioValue(_view.audioSlider.value);
        }

        private void OnAudioUpClick()
        {
            _view.audioSlider.value += .1f;
            GameManager.Instance.ChangeAudioValue(_view.audioSlider.value);
        }

        private void ShowCommonPage()
        {
            var userData = new UICommonPageUserData();
            userData.TabTitles = new string[] { "设置" };
            userData.IsBackLastDialog = true;
            userData.IsBackMainDialog = true;
            userData.IsNotShowDownLine = true;
            userData.BackLastPageAction = async () => { _uGuiForm.Close(); GameEntry.UI.OpenUIFormSync<UIMainMenu>(); await LoadMainScene(); };

            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, userData);
        }

        //TODO：需要调整位置
        private async UniTask LoadMainScene()
        {
            await GameEntry.UI.OpenUIFormAsync<UILoading>();
            UIEventDefine.UILoadingShowPro.SendMessage(1);

            await GameEntry.Scene.LoadSceneAsync((int)EnumScene.SubScene, null, true, LoadSceneMode.Additive);
            await GameEntry.Scene.UnLoadSceneAsync((int)EnumScene.SettingScene, null, true);
            await GameEntry.Scene.LoadSceneAsync((int)EnumScene.MainScene, null, true, LoadSceneMode.Additive);

            GameEntry.UI.CloseUIForm<UILoading>();
        }


        private void OnGroupValueChange(bool isSwitch)
        {
            //_view.qualityGroup.GetFirstActiveToggle().isOn = true;
            Toggle tmpTog = _view.qualityGroup.ActiveToggles().FirstOrDefault();

            Debug.Log(tmpTog);

            int index = tmpTog == _view.togPreformant ? 0 : (tmpTog == _view.togBalanced ? 1 : (tmpTog == _view.togHighFidelity ? 2 : 0));

            GameManager.Instance.ChangeQuality(index);
        }


    }
}
