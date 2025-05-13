using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using Hotfix.ExcelData;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Video;
using System.Linq;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;

namespace Hotfix.UI
{
    /// <summary>
    /// 故障考核界面
    /// </summary>
    public class UIAssessment : UIForm<UIViewAssessment, UIModelAssessment>
    {
        private UGuiForm _uGuiForm;

        #region 控制消息扩展

        private List<int> uiLevel;

        /// <summary>
        /// 操作层级索引
        /// 0-页签索引，1-主内容，2-子内容
        /// </summary>
        private int uiAreaType;

        /// <summary>
        /// 当前选中的索引
        /// </summary>
        private int optionIndex = -1;

        /// <summary>
        /// 本模块主配置
        /// </summary>
        private ModuleConfig moduleConfig;

        #endregion

        #region Component

        private UIViewAssessment _view;
        private UIModelAssessment _model;

        private int _currentIndex;
        private List<VideoClip> _videoClips = new();
        private VideoClip defaultClip;

        #endregion

        private ToggleExtend _toggleExtend1;
        private ToggleExtend _toggleExtend2;
        private ToggleExtend _toggleExtend3;
        private ToggleExtend _toggleExtend4;

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitUIListener();

            showPauseAndResumeAction = true;

            defaultClip = _view.videoPlayer.clip;

            #region 控制和被控消息扩展

            moduleConfig = ModuleConfigTable.Instance.Get(10);
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
                //switch (msg.uiLevel[2])
                switch (msg.optionIndex)
                {
                    case 0:
                        OnSelfExamHandle(true, false);
                        break;
                    case 1:
                        OnAssessmentHandle(true, false);
                        break;
                    case 2:
                        OnTrainHandle(true, false);
                        break;
                    case 3:
                        OnTaskHandle(true, false);
                        break;
                }
            }
        }

        #endregion

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            optionIndex = -1;

            ResetUIComponent();
            ShowCommonPage();
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);

            ResetUIComponent();
        }

        #region UI

        private void ResetUIComponent()
        {
            _view.togCustomItem.isOn = false;
            _view.togTeacherItem.isOn = false;
            _view.togTrainItem.isOn = false;
            _view.togTaskItem.isOn = false;

            GameManager.Instance.ShowAllParts();
            GameManager.Instance.ShowAllRenderer();

            _view.videoPlayer.loopPointReached += ChangeVideoClip;
        }

        private void InitUIListener()
        {
            //扩展区分是否被动触发
            // _view.togCustomItem.onValueChanged.AddListener(OnSelfExamHandle);
            // _view.togTeacherItem.onValueChanged.AddListener(OnAssessmentHandle);
            // _view.togTrainItem.onValueChanged.AddListener(OnTrainHandle);
            // _view.togTaskItem.onValueChanged.AddListener(OnTaskHandle);

            _view.togCustomItem.onValueChanged.AddListener((isOn) => { OnSelfExamHandle(isOn, true); });
            _view.togTeacherItem.onValueChanged.AddListener((isOn) => { OnAssessmentHandle(isOn, true); });
            _view.togTrainItem.onValueChanged.AddListener((isOn) => { OnTrainHandle(isOn, true); });
            _view.togTaskItem.onValueChanged.AddListener((isOn) => { OnTaskHandle(isOn, true); });

            _toggleExtend1 = _view.togCustomItem?.GetComponent<ToggleExtend>();
            _toggleExtend2 = _view.togTeacherItem?.GetComponent<ToggleExtend>();
            _toggleExtend3 = _view.togTrainItem?.GetComponent<ToggleExtend>();
            _toggleExtend4 = _view.togTaskItem?.GetComponent<ToggleExtend>();

            _toggleExtend1.Init(ModuleConfigTable.Instance.Get(12).Title, true, true, TrainType.UnderpanType);
            _toggleExtend2.Init(ModuleConfigTable.Instance.Get(13).Title, true, true, TrainType.EngineType);
            _toggleExtend3.Init(ModuleConfigTable.Instance.Get(14).Title, true, true, TrainType.ElectricalType);
            _toggleExtend4.Init(ModuleConfigTable.Instance.Get(15).Title, true, true, TrainType.RenWuFaBu);

            SetUpTooglePointListener(_toggleExtend1);
            SetUpTooglePointListener(_toggleExtend2);
            SetUpTooglePointListener(_toggleExtend3);
            SetUpTooglePointListener(_toggleExtend4);

            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(12).Description;
        }

        private void SetUpTooglePointListener(ToggleExtend togE)
        {
            togE.OnPointerEnterAction = OnPointerEnterActionInvoke;
            togE.OnPointerExitAction = OnPointerExitActionInvoke;
        }

        private void OnPointerEnterActionInvoke(Toggle tog)
        {
            int id = 12;
            if (tog == _view.togTeacherItem)
            {
                id = 13;
            }
            else if (tog == _view.togTrainItem)
            {
                id = 14;
            }
            else if (tog == _view.togTaskItem)
            {
                id = 15;
            }

            _videoClips.Clear();
            _view.videoPlayer.clip = null;
            _videoClips = (Resources.LoadAll<VideoClip>("Video/AssessmentVideo/" + id)).ToList();

            _view.videoPlayer.clip = _videoClips.Count > 0 ? _videoClips[0] : defaultClip;

            Sprite tmp = Resources.Load<Sprite>("BG/" + tog.name.Split('_')[1]);
            if (tmp != null)
            {
                _view.imgBg.sprite = tmp;
            }

            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(id).Description;
            PlayUIEffect(AppConst.AssetPathConst.HoverSound);
        }

        private void OnPointerExitActionInvoke(Toggle tog)
        {
            _view.tmptxtIntroduce.text = ModuleConfigTable.Instance.Get(-1).Description;
        }

        private void ShowCommonPage()
        {
            var userData = new UICommonPageUserData
            {
                TabTitles = new[] { "首页", "虚拟实训" },

                // 新增按钮公共方法返回主菜单页面，Owner: 王柏雁 2025-04-12
                BackLastPageAction = backLastPage,
                BackMainPageAction = backMainPage,
                LabelToggleRespondClickAction = labelToggleRespondOnClickAction,
                IsBackLastDialog = true,
                IsShowAssessment = true
            };

            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, userData);
            return;


            #region UI标签事件功能处理 Owner: 王柏雁 2025-4-12

            // 标签切换响应点击事件
            void labelToggleRespondOnClickAction(ELabelToggleType labelType, bool isHand)
            {
                switch (labelType)
                {
                    case ELabelToggleType.Label1:
                        backMainPage();
                        break;
                    case ELabelToggleType.Label2:
                        // Tips: 已在当前标签页面，不需要做其它处理，可以做一些UI上的数据刷新
                        break;
                }

                uiAreaType = 0;
                optionIndex = (int)labelType;
                int temp = optionIndex + 1;
                HandleMsg.Instance.UILevelBuilder(uiLevel,2);
                HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");
                if (isHand)
                {
                    SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
                }
            }

            void backLastPage()
            {
                backMainPage();
            }

            void backMainPage()
            {
                Debug.Log("返回主菜单");
                _uGuiForm.Close();
                GameEntry.UI.OpenUIFormSync<UIMainMenu>();
            }

            #endregion
        }

        private void OnTaskHandle(bool state, bool isHand)
        {
            //不再只走ToggleExtend的OnPointerEnter
            GameManager.Instance.TrainType = TrainType.RenWuFaBu;
            if (!state) return;

            var data = new UIAssessmentSubjectData()
            {
                modelId = AppConst.IDConst.AssessmentSubjectId,
            };

            OnCloseHandle();

            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            uiAreaType = 1;
            optionIndex = 3;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3, 3);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        private void OnTrainHandle(bool state, bool isHand)
        {
            //不再只走ToggleExtend的OnPointerEnter
            GameManager.Instance.TrainType = TrainType.ElectricalType;
            if (!state) return;

            var data = new UIAssessmentSubjectData()
            {
                modelId = AppConst.IDConst.AssessmentSubjectId,
            };
            //GameManager.Instance.AssessmentMode = EnumAssessmentMode.Teacher;
            OnCloseHandle();
            //GameEntry.UI.OpenUIFormAsync<UITopicList>(userData: data).Forget();
            //GameEntry.UI.OpenUIFormSync<UITopicList>();
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            uiAreaType = 1;
            optionIndex = 2;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3, 2);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        private void OnAssessmentHandle(bool state, bool isHand)
        {
            //不再只走ToggleExtend的OnPointerEnter
            GameManager.Instance.TrainType = TrainType.UnderpanType;
            if (!state) return;
            var data = new UIAssessmentSubjectData()
            {
                modelId = AppConst.IDConst.AssessmentSubjectId,
            };

            //GameManager.Instance.AssessmentMode = EnumAssessmentMode.Teacher;
            OnCloseHandle();
            //GameEntry.UI.OpenUIFormAsync<UITopicList>(userData: data).Forget();
            //GameEntry.UI.OpenUIFormSync<UITopicList>();
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            uiAreaType = 1;
            optionIndex = 1;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3, 1);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        private void OnSelfExamHandle(bool state, bool isHand)
        {
            //不再只走ToggleExtend的OnPointerEnter
            GameManager.Instance.TrainType = TrainType.EngineType;
            if (!state) return;
            var data = new UIAssessmentSubjectData()
            {
                modelId = AppConst.IDConst.SelfExamSubjectId,
            };

            OnCloseHandle();
            //GameManager.Instance.AssessmentMode = EnumAssessmentMode.Self;
            //GameEntry.UI.OpenUIFormSync<UITopicList>();
            //GameEntry.UI.OpenUIFormAsync<UITopicList>(userData: data).Forget();
            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            uiAreaType = 1;
            optionIndex = 0;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3, 0);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        private void OnCloseHandle()
        {
            _uGuiForm.Close();
            ProcessEventDefine.SelectTopicCall.SendMessage(5);
            //GameEntry.UI.OpenUIFormSync<UITopicList>();
        }

        #endregion

        private void ChangeVideoClip(VideoPlayer videoPlayer)
        {
            _currentIndex++;
            VideoClip videoClip = null;

            _currentIndex = _currentIndex < _videoClips.Count ? _currentIndex++ : 1;

            if (_currentIndex > _videoClips.Count)
            {
                //Debug.LogError($"当前视频索引超出范围：{_currentIndex} > {_videoClips.Count}");
                return;
            }

            videoClip = _videoClips[_currentIndex];
            _view.videoPlayer.clip = videoClip;
        }
    }
}