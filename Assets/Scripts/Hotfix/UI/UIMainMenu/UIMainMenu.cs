using System.Collections.Generic;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;
using UnityEngine.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 主菜单界面
    /// </summary>
    public class UIMainMenu : UIForm<UIViewMainMenu, UIModelMainMenu>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private UIViewMainMenu _view;
        private UIModelMainMenu _model;

        #endregion

        private ToggleExtend _toggleExtend1;
        private ToggleExtend _toggleExtend2;
        private ToggleExtend _toggleExtend3;
        private ToggleExtend _toggleExtend4;
        //private ToggleExtend _toggleExtend5;

        #region 控制消息扩展

        private List<int> uiLevel;

        /// <summary>
        /// 本模块主配置
        /// </summary>
        private ModuleConfig moduleConfig;

        #endregion

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitBtnListener();

            #region 控制和被控消息扩展

            moduleConfig = ModuleConfigTable.Instance.Get(-1);
            HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
            Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>(moduleConfig.ClassName, (msg) =>
            {
                OnControlMessage(msg);
                ServNet.Instance.SetCacheMsgBaseState(msg);
            });

            #endregion
        }

        #region 被控消息扩展

        void OnControlMessage(MsgUINavigationData msg)
        {
            //平板点击的主模块
            if (msg.uiAreaType == 1)
            {
                //索引适配
                OnSelectTopicHandle(msg.optionIndex == 2 ? 4 : msg.optionIndex, false);
            }
        }

        #endregion

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userDatas)
        {
            base.OnOpen(userDatas);

            HandleMsg.Instance.UILevelBuilder(uiLevel, 1);

            ResetUIComponent();
            ShowCommonPage();
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);

            ShowCommonPage();
        }

        #region UI

        private void ResetUIComponent()
        {
            //_view.btnTeachingDemo.gameObject.SetActive(true);
            //_view.btnTrain.gameObject.SetActive(GameManager.Instance.gameMode == EnumGameMode.Net);
            //_view.btnAssessment.gameObject.SetActive(GameManager.Instance.gameMode == EnumGameMode.Net);

            _view.togTeach.isOn = false;
            _view.togFalutTrain.isOn = false;
            _view.togFaultExam.isOn = false;
            _view.togVirtualTrain.isOn = false;
        }

        private void InitBtnListener()
        {
            //_view.btnClose.onClick.AddListener(OnCloseHandle);
            _view.togTeach.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnSelectTopicHandle(0, true);
            });
            _view.togFalutTrain.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnSelectTopicHandle(1, true);
            });
            _view.togFaultExam.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnSelectTopicHandle(4, true);
            });
            _view.togVirtualTrain.onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnSelectTopicHandle(3, true);
            });
            //_view.togDigitalTwin.onValueChanged.AddListener((isOn) => { if (isOn) OnSelectTopicHandle(4); });

            _toggleExtend1 = _view.togTeach?.GetComponent<ToggleExtend>();
            _toggleExtend2 = _view.togFalutTrain?.GetComponent<ToggleExtend>();
            _toggleExtend3 = _view.togFaultExam?.GetComponent<ToggleExtend>();
            _toggleExtend4 = _view.togVirtualTrain?.GetComponent<ToggleExtend>();
            //_toggleExtend5 = _view.togDigitalTwin?.GetComponent<ToggleExtend>();

            _toggleExtend1.Init(ModuleConfigTable.Instance.Get(3).Title);
            _toggleExtend2.Init(ModuleConfigTable.Instance.Get(8).Title);
            _toggleExtend3.Init(ModuleConfigTable.Instance.Get(9).Title);
            _toggleExtend4.Init(ModuleConfigTable.Instance.Get(10).Title);
            //_toggleExtend5.Init(ModuleConfigTable.Instance.Get(11).Title);

            SetUpTooglePointListener(_toggleExtend1);
            SetUpTooglePointListener(_toggleExtend2);
            SetUpTooglePointListener(_toggleExtend3);
            SetUpTooglePointListener(_toggleExtend4);
            //SetUpTooglePointListener(_toggleExtend5);

            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(-1).Description;
        }

        private void ShowCommonPage()
        {
            var userData = new UICommonPageUserData();
            userData.TabTitles = new string[] { "首页" };
            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, userData);
        }

        private void SetUpTooglePointListener(ToggleExtend togE)
        {
            togE.OnPointerEnterAction = OnPointerEnterActionInvoke;
            togE.OnPointerExitAction = OnPointerExitActionInvoke;
        }

        private void OnPointerEnterActionInvoke(Toggle tog)
        {
            int id = 3;
            if (tog == _view.togFalutTrain)
            {
                id = 8;
            }
            else if (tog == _view.togFaultExam)
            {
                id = 9;
            }
            else if (tog == _view.togVirtualTrain)
            {
                id = 10;
            }

            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(id).Description;

            PlayUIEffect(AppConst.AssetPathConst.HoverSound);
        }

        private void OnPointerExitActionInvoke(Toggle tog)
        {
            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(-1).Description;
        }

        private void OnCloseHandle()
        {
            ProcessEventDefine.ChangeLoginMachineCall.SendMessage();
        }

        private void OnSelectTopicHandle(int selectIndex, bool isHand)
        {
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);
            _uGuiForm.Close();
            ProcessEventDefine.SelectTopicCall.SendMessage(selectIndex);

            //序号转换
            int index = selectIndex == 4 ? 2 : selectIndex;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 2, index);
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, 1, index);
            }
        }

        #endregion
    }
}