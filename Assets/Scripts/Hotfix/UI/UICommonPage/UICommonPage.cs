using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using Cysharp.Threading.Tasks;
using RainbowArt.CleanFlatUI;
using UI.NetworkUI;
using UnityEngine.SceneManagement;

namespace Hotfix.UI
{
    using System.Linq;
    using UnityEngine.Events;

    /// <summary>
    /// 公用界面
    /// </summary>
    public class UICommonPage : UIForm<UIViewCommonPage, UIModelCommonPage>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private UIViewCommonPage _view;
        private UIModelCommonPage _model;

        #endregion

        private UICommonPageUserData _userData;

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitUIListener();

            #region Pad端控制事件监听

            //当接收到平板端切换页签的消息
            Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>("PadUICommonPageTab", (msg) =>
            {
                OnControlMessage(msg.optionIndex);
                ServNet.Instance.SetCacheMsgBaseState(msg);
            });
            Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadSubmitScore", (value) =>
            {
                if (_view.btnTaskComplete.gameObject.activeInHierarchy) OnTaskCompleteHandle(false);
                ServNet.Instance.SetCacheMsgBaseState(value);
            });
            Ctrl_MessageCenter.AddMsgListener<MsgOperationData>("PadChangeTrainMode", (value) =>
            {
                if (_view.togIsAssessment.gameObject.activeInHierarchy) ChangeTrainMode(value.state, false);
                ServNet.Instance.SetCacheMsgBaseState(value);
            });
            Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadChange2LastPage", (value) =>
            {
                if (_view.btnCallBack.gameObject.activeInHierarchy) OnBackLastHandle(false);
                ServNet.Instance.SetCacheMsgBaseState(value);
            });
            // Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadExitApp", (value) =>
            // {
            //     OnCloseHandle(false);
            //     ServNet.Instance.SetCacheMsgBaseState(value);
            // });
            Ctrl_MessageCenter.AddMsgListener<bool, MsgBase>("PadShowOrHideModel", (value, msg) =>
            {
                if (_view.togModelShowOrHide.gameObject.activeInHierarchy) OnToggleModelShowOrHide(value, false);
                ServNet.Instance.SetCacheMsgBaseState(msg);
            });
            Ctrl_MessageCenter.AddMsgListener<MsgBase>("Pad2Main", (value) =>
            {
                if(_view.btnBackMain.gameObject.activeInHierarchy) OnBackMainHandle(false);
                ServNet.Instance.SetCacheMsgBaseState(value);
            });

            #endregion
        }

        private void OnControlMessage(int index)
        {
            switch (index)
            {
                case 0:
                    OnTogBackMainHandle(true, false);
                    break;
                case 1:
                    OnTogBackLastHandle(true, false);
                    break;
                case 2:
                    OnTogBackBreakHandle(true, false);
                    break;
            }
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            InitUserData(userData);
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);

            InitUserData(userData);
        }

        private void InitUIListener()
        {
            _view.btnExit.onClick.AddListener(OnLogoutHandle);
            _view.btnClose.onClick.AddListener(() => { OnCloseHandle(true); });
            _view.btnCallBack.onClick.AddListener(() => { OnBackLastHandle(true); });
            _view.btnBackMain.onClick.AddListener(() => { OnBackMainHandle(true); });
            _view.btnSetting.onClick.AddListener(OnSettingHandle);
            _view.btnTaskComplete.onClick.AddListener(() => { OnTaskCompleteHandle(true); });

            _view.togIsAssessment.onValueChanged.AddListener((value) => { ChangeTrainMode(value, true); });
            _view.togModelShowOrHide.onValueChanged.AddListener((value) => { OnToggleModelShowOrHide(value, true); });

            #region 2025-4-9 新增标签点击功能 Owner:王柏雁

            _view.togMenu.onValueChanged.RemoveAllListeners();
            _view.togSecond.onValueChanged.RemoveAllListeners();
            _view.togThird.onValueChanged.RemoveAllListeners();
            //区分 本机自主点击 还是 平板被控
            //_view.togMenu.onValueChanged.AddListener(OnTogBackMainHandle); // Toggle标签_1
            //_view.togSecond.onValueChanged.AddListener(OnTogBackLastHandle); // Toggle标签_2
            //_view.togThird.onValueChanged.AddListener(OnTogBackBreakHandle); // Toggle标签_3

            _view.togMenu.onValueChanged.AddListener((isOn) => { OnTogBackMainHandle(isOn, true); });
            _view.togSecond.onValueChanged.AddListener((isOn) => { OnTogBackLastHandle(isOn, true); });
            _view.togThird.onValueChanged.AddListener((isOn) => { OnTogBackBreakHandle(isOn, true); });

            #endregion

            _view.imgBg.transform.Find("UpButShow").GetComponent<TitleExtend>().Init(_view.imgBg.transform.Find("Title"));
        }

        /// <summary>
        /// 更新UI标签激活状态
        /// </summary>
        public void UpdateLabelActiveState(object userData)
        {
            if (userData == null) return;
            _userData = (UICommonPageUserData)userData;

            var dataTitles = _userData.TabTitles;

            for (var i = 0; i < _view.ToggleList.Count; i++)
            {
                if (dataTitles.Length > i)
                {
                    _view.ToggleList[i].gameObject.SetActive(true);
                    _view.ToggleTextList[i].text = dataTitles[i];
                }
                else
                {
                    _view.ToggleList[i].gameObject.SetActive(false);
                }
            }
        }

        private void InitUserData(object userData)
        {
            if (userData == null) return;
            _userData = (UICommonPageUserData)userData;

            //页签
            var dataTitles = _userData.TabTitles;

            for (int i = 0; i < _view.ToggleList.Count; i++)
            {
                if (dataTitles.Length > i)
                {
                    _view.ToggleList[i].gameObject.SetActive(true);
                    _view.ToggleTextList[i].text = dataTitles[i];
                    // _view.ToggleList[i].isOn = true;
                }
                else
                {
                    _view.ToggleList[i].gameObject.SetActive(false);
                }
            }

            // 默认选中最后一个
            //_view.ToggleList[dataTitles.Length - 1].isOn = true;

            int lastIndex = dataTitles.Length - 1;
            for (int i = 0; i < _view.ToggleList.Count; i++)
            {
                var isLast = i == lastIndex;
                _view.ToggleList[i].SetIsOnWithoutNotify(isLast);
                _view.ToggleList[i].GetComponentInChildren<ToggleSwap>().ToggleValueChanged(isLast);
            }

            OnControlMessage(lastIndex);

            //背景
            _view.imgBg.enabled = _userData.IsShowBackground;
            //底部标线
            _view.bottomLine.gameObject.SetActive(!_userData.IsNotShowDownLine);
            //回上页按钮和回调事件
            _view.btnCallBack.gameObject.SetActive(_userData.BackLastPageAction != null);
            //返回主页按钮
            _view.btnBackMain.gameObject.SetActive(_userData.BackMainPageAction != null);
            //用户名称
            _view.userNameText.text = UserManager.Instance.UserName;

            _view.btnSetting.gameObject.SetActive(!_userData.IsNotShowSetting);

            _view.btnTaskComplete.gameObject.SetActive(_userData.TaskCompleteAction != null);
            //_view.btnSetting

            _view.togIsAssessment.gameObject.SetActive(_userData.IsShowAssessment);

            bool togModelShowOrHide = _userData.ModelShowOrHideAction != null;
            _view.togModelShowOrHide.gameObject.SetActive(togModelShowOrHide);
            if (togModelShowOrHide)
            {
                _view.togModelShowOrHide.isOn = false;
                _view.txtModelShowOrHide.text = "隐藏";
                Ctrl_MessageCenter.AddMsgListener<bool>("OnOtherModelShowModeChange", OnOtherModelShowModeChange);
            }
        }

        /// <summary>
        /// 隐藏结构Toggle列表 Owner:王柏雁 2025-4-9
        /// </summary>
        /// <param name="togIndex"></param>
        /// <param name="tabTitles"></param>
        private void SwitchTogHandle(int togIndex, string[] tabTitles)
        {
            if (tabTitles.Length <= 1) return;

            for (var i = 0; i < _view.ToggleList.Count; i++)
            {
                _view.ToggleList[i].gameObject.SetActive(i <= togIndex);
            }
        }

        private static void RefreshUIStructuralPanel()
        {
            var iUIForm = GameEntry.UI.GetUIForm<UIStructuralCognition>();
            var uiStructuralCognition = iUIForm as UIStructuralCognition;
            uiStructuralCognition?.SetStructuralPanelActive(false);
            uiStructuralCognition?.MainStructuralToggleGroupActive(true);
        }

        private static void RefreshUIPrincipleLearningPanel()
        {
            var iUIForm = GameEntry.UI.GetUIForm<UIPrincipleLearning>();
            var uiStructuralCognition = iUIForm as UIPrincipleLearning;
            uiStructuralCognition?.SetStructuralPanelActive(false);
            uiStructuralCognition?.MainStructuralToggleGroupActive(true);
        }

        // 通用刷新界面方法
        public void RefreshUI()
        {
            var tabTitles = _userData.TabTitles;
            if (tabTitles.Contains("结构认知"))
            {
                RefreshUIStructuralPanel();
            }
            else if (tabTitles.Contains("原理仿真"))
            {
                RefreshUIPrincipleLearningPanel();
            }
            else if (tabTitles.Contains("虚拟实训"))
            {
                // Debug.Log("刷新界面 > 虚拟训练");
                GameEntry.UI.OpenUIFormSync<UICommonPage>();
            }
            else if (tabTitles.Contains("课题列表"))
            {
                // Debug.Log("刷新界面 > 课题列表");
            }
            else if (tabTitles.Contains("检查列表"))
            {
                // Debug.Log("刷新界面 > 检查列表");
            }
        }

        /// <summary>
        /// Toggle返回主页 Owner:王柏雁 2025-4-9
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="isHand"></param>
        private void OnTogBackMainHandle(bool isOn, bool isHand)
        {
            if (!isOn) return;
            _userData.LabelToggleRespondClickAction?.Invoke(ELabelToggleType.Label1, isHand);
            
            if (isHand)
            {
                SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.Home);
            }
        }

        /// <summary>
        /// Toggle第二个标签点击处理 Owner:王柏雁 2025-4-9
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="isHand"></param>
        private void OnTogBackLastHandle(bool isOn, bool isHand)
        {
            if (!isOn) return;
            _userData.LabelToggleRespondClickAction?.Invoke(ELabelToggleType.Label2, isHand);
        }

        /// <summary>
        /// Toggle第三个标签点击处理 Owner:王柏雁 2025-4-9
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="isHand"></param>
        private void OnTogBackBreakHandle(bool isOn, bool isHand)
        {
            if (!isOn) return;
            _userData.LabelToggleRespondClickAction?.Invoke(ELabelToggleType.Label3, isHand);
        }

        private void OnBackMainHandle(bool isHand)
        {
            if (_userData.IsBackMainDialog)
            {
                UIEventDefine.UIPopTipCall.SendMessage(() => { BackMain(); }, "提示", "确定回到主界面吗？");
            }
            else
            {
                RefreshUI();
                BackMain();
                PlayUIEffect(AppConst.AssetPathConst.ClickSound);
            }
            
            if (isHand)
            {
                SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.Home);
            }
        }

        private void OnBackLastHandle(bool isHand)
        {
            if (_userData.IsBackMainDialog)
            {
                UIEventDefine.UIPopTipCall.SendMessage(() => { BackLast(); }, "提示", "确定返回上一页吗？");
            }
            else
            {
                RefreshUI();
                BackLast();
                PlayUIEffect(AppConst.AssetPathConst.ClickSound);
            }

            if (isHand)
            {
                SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.BackLastPage);
            }
        }

        private void OnLogoutHandle()
        {
            UIEventDefine.UIPopTipCall.SendMessage(() =>
            {
                _uGuiForm.Close();
                GameEntry.UI.OpenUIFormSync<UILogin>();
            }, "提示", "确定登出吗？");
        }

        private void OnCloseHandle(bool isHand)
        {
            UIEventDefine.UIPopTipCall.SendMessage(() =>
            {
                if (isHand)
                {
                    SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.ExitGame);
                }
                GameManager.Instance.QuitApplication();
            }, "提示", "确定关闭软件吗？");
        }

        private void OnTaskCompleteHandle(bool isHand)
        {
            _userData.TaskCompleteAction?.Invoke();

            TopicManager.Instance.OnTaskCompleteHandle();

            if (isHand)
            {
                SendMsgManager.SendCommonBtnMsg(CommonBtnMsgType.SubmitScore);
            }
        }

        private void BackMain()
        {
            _uGuiForm.Close();
            _userData.BackMainPageAction?.Invoke();
            GameEntry.UI.OpenUIFormSync<UIMainMenu>();
        }

        private void BackLast()
        {
            //_uGuiForm.Close();

            if (_userData.BackLastPageAction != null)
            {
                Debug.Log("回上一页");
            }
            else
            {
                Debug.Log("回上一页无回调");
            }

            _userData.BackLastPageAction?.Invoke();
        }

        private async void OnSettingHandle()
        {
            _uGuiForm.Close();
            _userData.BackLastPageAction?.Invoke();
            GameEntry.UI.OpenUIFormSync<UISetting>();
            await LoadMainScene();
        }

        //TODO:需要调整位置
        private async UniTask LoadMainScene()
        {
            await GameEntry.UI.OpenUIFormAsync<UILoading>();
            UIEventDefine.UILoadingShowPro.SendMessage(1);

            await GameEntry.Scene.UnLoadSceneAsync((int)EnumScene.MainScene, null, true);

            await GameEntry.Scene.LoadSceneAsync((int)EnumScene.SettingScene, null, true, LoadSceneMode.Additive);

            await GameEntry.Scene.UnLoadSceneAsync((int)EnumScene.SubScene, null, true);

            GameEntry.UI.CloseUIForm<UILoading>();
            GameEntry.UI.CloseUIForm<UIMainMenu>();

            GameManager.Instance.InitCarMaterial();
        }

        /// <summary>
        /// 切换实训课题模式（训练|考核）
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="isHand"></param>
        private void ChangeTrainMode(bool isOn, bool isHand)
        {
            //Debug.Log(isOn);
            GameManager.Instance.AssessmentMode = isOn ? EnumAssessmentMode.Self : EnumAssessmentMode.Train;

            if (isHand)
            {
                SendMsgManager.SendOperationData(CommonBtnMsgType.Assess, isOn);
            }
        }

        #region 结构认知 模型显隐开关

        private void OnToggleModelShowOrHide(bool isOn, bool isHand)
        {
            _userData.ModelShowOrHideAction?.Invoke(isOn);
            _view.txtModelShowOrHide.text = isOn ? "显示" : "隐藏";

            if (isHand)
            {
                var type = !isOn ? CommonBtnMsgType.Show : CommonBtnMsgType.Hide;
                SendMsgManager.SendCommonBtnMsg(type);
            }
        }

        private void OnOtherModelShowModeChange(bool state)
        {
            _view.togModelShowOrHide.gameObject.SetActive(state);
        }

        #endregion
    }

    /// <summary>
    /// 公用界面自定义数据结构
    /// </summary>
    public struct UICommonPageUserData
    {
        /// <summary>
        /// 页签数组
        /// </summary>
        public string[] TabTitles;

        /// <summary>
        /// 回上页界面回调 及 回上页界面按钮显示与否
        /// </summary>
        public System.Action BackLastPageAction;

        /// <summary>
        /// 是否弹出回上页面确认对话框
        /// </summary>
        public bool IsBackLastDialog;

        /// <summary>
        /// 是否展示背景
        /// </summary>
        public bool IsShowBackground;

        /// <summary>
        /// 回主页面回调 及 回主页面按钮显示有否
        /// </summary>
        public System.Action BackMainPageAction;

        /// <summary>
        /// 是否显示回主页确认对话框
        /// </summary>
        public bool IsBackMainDialog;

        /// <summary>
        /// 是否不显示底部标线
        /// </summary>
        public bool IsNotShowDownLine;

        /// <summary>
        /// 是否显示设置
        /// </summary>
        public bool IsNotShowSetting;

        /// <summary>
        /// 点击提交回调  及 提交按钮显示有否
        /// </summary>
        public System.Action TaskCompleteAction;

        /// <summary>
        /// 是否显示考核模式
        /// </summary>
        public bool IsShowAssessment;

        /// <summary>
        /// 模型展示与否
        /// </summary>
        public System.Action<bool> ModelShowOrHideAction;

        /// <summary>
        /// 标签切换响应点击操作  Owner:王柏雁 2025-4-12
        /// </summary>
        public UnityAction<ELabelToggleType, bool> LabelToggleRespondClickAction;
    }
}

public enum ELabelToggleType
{
    [InspectorName("标签1")] Label1 = 0,
    [InspectorName("标签2")] Label2,
    [InspectorName("标签3")] Label3,
    [InspectorName("标签4")] Label4,
    [InspectorName("标签5")] Label5,
    [InspectorName("标签6")] Label6,
}