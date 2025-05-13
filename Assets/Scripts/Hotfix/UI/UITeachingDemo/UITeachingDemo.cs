using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using TMPro;
using Hotfix.ExcelData;
using UnityEngine.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 教学演示界面
    /// </summary>
    public class UITeachingDemo : UIForm<UIViewTeachingDemo, UIModelTeachingDemo>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private UIViewTeachingDemo _view;
        private UIModelTeachingDemo _model;

        #endregion

        private ToggleExtend _toggleExtend1;
        private ToggleExtend _toggleExtend2;
        private ToggleExtend _toggleExtend3;

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

            InitBtnListener();

            showPauseAndResumeAction = true;
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            ResetUIComponent();
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

            ResetUIComponent();
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

        #region UIComponent

        private void ResetUIComponent()
        {
            //_view.btnEquipmentMonitoring.gameObject.SetActive(GameManager.Instance.gameMode == EnumGameMode.Net &&
            //                                                  HardwareManager.Instance.connectHardware);

            _view.togStructuralCognition.isOn = false;
            _view.togPrincipleLearning.isOn = false;
            _view.togEquipmentMonitoring.isOn = false;
        }

        private void InitBtnListener()
        {
            //_view.btnClose.onClick.AddListener(OnCloseHandle);
            _view.togStructuralCognition.onValueChanged.AddListener((isOn) => { if (isOn) OnStructuralCognitionHandle(); });
            _view.togPrincipleLearning.onValueChanged.AddListener((isOn) => { if (isOn) OnPrincipleLearningHandle(); });
            _view.togEquipmentMonitoring.onValueChanged.AddListener((isOn) => { if (isOn) OnEquipmentMonitoringHandle(); });

            _toggleExtend1 = _view.togStructuralCognition?.GetComponent<ToggleExtend>();
            _toggleExtend2 = _view.togPrincipleLearning?.GetComponent<ToggleExtend>();
            _toggleExtend3 = _view.togEquipmentMonitoring?.GetComponent<ToggleExtend>();

            _toggleExtend1.Init(ModuleConfigTable.Instance.Get(3).Title);
            _toggleExtend2.Init(ModuleConfigTable.Instance.Get(4).Title);
            _toggleExtend3.Init(ModuleConfigTable.Instance.Get(5).Title);

            SetUpTooglePointListener(_toggleExtend1);
            SetUpTooglePointListener(_toggleExtend2);
            SetUpTooglePointListener(_toggleExtend3);

            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(0).Description;
        }

        private void SetUpTooglePointListener(ToggleExtend togE)
        {
            togE.OnPointerEnterAction = OnPointerEnterActionInvoke;
            togE.OnPointerExitAction = OnPointerExitActionInvoke;
        }

        private void OnPointerEnterActionInvoke(Toggle tog)
        {
            int id = 3;
            if (tog == _view.togPrincipleLearning)
            {
                id = 4;
            }
            else if (tog == _view.togEquipmentMonitoring)
            {
                id = 5;
            }

            PlayUIEffect(AppConst.AssetPathConst.HoverSound);
            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(id).Description;
        }

        private void OnPointerExitActionInvoke(Toggle tog)
        {
            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(-1).Description;
        }

        private void ShowCommonPage()
        {
            var userData = new UICommonPageUserData();
            userData.TabTitles = new string[] { "首页", "教学演示" };
            userData.BackLastPageAction = () => { _uGuiForm.Close(); GameEntry.UI.OpenUIFormSync<UIMainMenu>(); };
            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, userData);
        }

        private void OnEquipmentMonitoringHandle()
        {
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);
            var data = new UIEquipmentMonitoringData() { checkPointId = -1 };
            GameEntry.UI.OpenUIFormAsync<UIEquipmentMonitoring>(true, data).Forget();
        }

        private void OnPrincipleLearningHandle()
        {
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);
            GameEntry.UI.OpenUIFormAsync<UIPrincipleLearning>().Forget();
        }

        private void OnStructuralCognitionHandle()
        {
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);
            GameEntry.UI.OpenUIFormAsync<UIStructuralCognition>().Forget();
        }

        private void OnCloseHandle()
        {
            _uGuiForm.Close();
            ProcessEventDefine.SelectTopicCall.SendMessage(3);
        }

        #endregion
    }
}
