using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameMain.Runtime;
using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.Event;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;
using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 故障训练界面
    /// </summary>
    public class UITrain : UIForm<UIViewTrain, UIModelTrain>
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

        /// <summary>
        /// 扩展选中索引
        /// </summary>
        private int optionIndexExtend = -1;

        #endregion

        #region Component

        private UIViewTrain _view;
        private UIModelTrain _model;
        private ScrollViewEx _scrollViewEx;
        List<GameObject> _WaiKe = new();

        #endregion

        #region Private

        private List<SubjectData> _subjectData;
        private ListEx<SubjectItem> _subjectItems;

        /// <summary>
        /// 当前页数
        /// </summary>
        private int mainPageIndex;

        /// <summary>
        /// 最大页数
        /// </summary>
        private int maxPageIndex;

        FaultCheckConfig2nd config2Nd;
        List<FaultCheckConfig3rd> faultCheckConfig3Rds;

        #endregion

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitField();
            InitUIComponent();
            InitUIListener();

            showPauseAndResumeAction = true;
        }

        #region 被控消息扩展

        void OnControlMessage(MsgUINavigationData msg)
        {
            //平板点击的主模块
            if (msg.uiAreaType == 1)
            {
                //区分Pad的不跨界面
                if (msg.uiLevel.Count > 3)
                {
                    var faultCheckConfig3 = faultCheckConfig3Rds[msg.optionIndex];

                    //自动切页
                    int desriedPageIndex = Mathf.FloorToInt(msg.optionIndex / AppConst.UIConst.TrainMainCount);
                    if (desriedPageIndex != mainPageIndex)
                    {
                        if (desriedPageIndex < mainPageIndex)
                            _view.btnPageUp.onClick.Invoke();
                        else _view.btnNextPage.onClick.Invoke();
                    }

                    //根据真实索引自动选中
                    int desiredIndex = msg.optionIndex % AppConst.UIConst.TrainMainCount;
                    if (_view.tsContentList.childCount > desiredIndex)
                    {
                        On2ndSelectedAction(desiredIndex, faultCheckConfig3, true, false);
                        var item = _view.tsContentList.GetChild(desiredIndex);
                        var tog = item.GetComponent<Toggle>();
                        tog.SetIsOnWithoutNotify(true);
                        ServNet.Instance.SetCacheMsgBaseState(msg);
                    }
                }
            }
            else if (msg.uiAreaType == 2)
            {
                if (togCheckPointItems.Count > msg.optionIndex)
                {
                    var checkPointItemGo = togCheckPointItems[msg.optionIndex];
                    var checkPointItem = checkPointItemGo.GetComponent<CheckPointItem>();
                    checkPointItem.OnToggleValueChange(true, false);
                    var tog = checkPointItemGo.GetComponent<Toggle>();
                    tog.SetIsOnWithoutNotify(true);
                    var item = checkPointItemGo.GetComponent<ToggleExtend>();
                    item.ToggleValueChanged(true);

                    ServNet.Instance.SetCacheMsgBaseState(msg);
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
            ReInit(userData);

            base.OnOpen(userData);

            ShowCommonPage();
        }

        private void ReInit(object userData)
        {
            #region 控制和被控消息扩展

            int id;
            switch (GameManager.Instance.TrainType)
            {
                case TrainType.EngineType:
                    id = 12;
                    break;
                case TrainType.UnderpanType:
                    id = 13;
                    break;
                case TrainType.ElectricalType:
                    id = 14;
                    break;
                case TrainType.RenWuFaBu:
                    id = 15;
                    break;
                default:
                    id = 12;
                    break;
            }

            moduleConfig = ModuleConfigTable.Instance.Get(id);
            HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
            //拼接上上次选择的索引
            HandleMsg.Instance.UILevelBuilder(uiLevel, 4, TopicManager.Instance.HelperIndex);

            //Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>(moduleConfig.ClassName, (msg) => { OnControlMessage(msg); });
            Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>("UITrain", (msg) => { OnControlMessage(msg); });

            #endregion

            optionIndex = -1;
            optionIndexExtend = -1;

            config2Nd = (FaultCheckConfig2nd)userData;
            faultCheckConfig3Rds = _model.GetFaultCheckConfig3rds(config2Nd.Id);
            maxPageIndex = Mathf.FloorToInt(faultCheckConfig3Rds.Count / AppConst.UIConst.TrainMainCount);
            mainPageIndex = 0;
            InitDatas();
            //UniTask.Void(async () =>
            //{
            //    await GetSubjectData();
            //    GenerateSubject();
            //    ResetUIComponent();
            //});

            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = GameManager.Instance.MainTarget.Find("DataScreenLookPos");
            GameManager.Instance.CinemachineCameraController.ResetCameraPosition(new Vector3(5f, 4.76f, 4.35f),
                new Vector3(36f, -167.1f, 0), 7f, new Vector2(2, 10), Vector3.zero);
        }

        public override void OnRefocus(object userData = null)
        {
            ReInit(userData);

            base.OnRefocus(userData);

            ShowCommonPage();
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);

            ClearCheckPoints();

            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i].isOn = false;
            }

            for (int i = 0; i < _view.tsContentList.childCount; i++)
            {
                Object.Destroy(_view.tsContentList.GetChild(i).gameObject);
            }

            _view.tsShowItem.gameObject.SetActive(false);

            faultCheckConfig3Rd = null;

            foreach (var item in _WaiKe)
            {
                item.gameObject.SetActive(true);
            }

            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = null;
            GameManager.Instance.RestCinemachineCamera();

            Ctrl_MessageCenter.RemoveMsgListener<MsgUINavigationData>("UITrain", (msg) => { OnControlMessage(msg); });
        }

        public void ClearCheckPoints()
        {
            if (togCheckPointItems != null && togCheckPointItems.Count > 0)
            {
                for (int i = togCheckPointItems.Count - 1; i >= 0; i--)
                {
                    Object.DestroyImmediate(togCheckPointItems[i]);
                }

                togCheckPointItems.Clear();
            }
        }

        #region UI

        private void InitUIComponent()
        {
            //_scrollViewEx = _view.tsScrollView.GetComponent<ScrollViewEx>();

            _view.tsShowItem.gameObject.SetActive(false);
            _view.togCheckPointItem.gameObject.SetActive(false);
        }

        private void ResetUIComponent()
        {
            //_scrollViewEx.ResetScrollEx();
        }

        public void ShowCommonPage()
        {
            var userData = new UICommonPageUserData
            {
                TabTitles = new[] { "首页", "检查列表" },
                BackLastPageAction = backLastPage,
                BackMainPageAction = backMainPage,
                LabelToggleRespondClickAction = labelToggleRespondOnClickAction,
                IsBackLastDialog = true,
                IsNotShowSetting = true,
                TaskCompleteAction = () => { TopicManager.Instance.TopicEnd(); }
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


                optionIndex = (int)labelType;
                int temp = optionIndex + 3;
                HandleMsg.Instance.UILevelBuilder(uiLevel, temp);
                HandleMsg.Instance.DebugUILevel(uiLevel, "TabUILevel");

                if (isHand)
                {
                    SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
                }
            }

            void backLastPage()
            {
                _uGuiForm.Close();
                GameEntry.UI.OpenUIFormSync<UITopicList>();
                TopicManager.Instance.TopicEnd();
            }

            void backMainPage()
            {
                _uGuiForm.Close();
                GameEntry.UI.OpenUIFormSync<UIMainMenu>();
            }

            #endregion
        }

        private void InitUIListener()
        {
            _view.btnPageUp.onClick.AddListener(OnUpPageBtnClicked);
            _view.btnNextPage.onClick.AddListener(OnNextPageBtnClicked);

            //_view.btnCheckPointRight.onClick.AddListener(OnRightBtnClicked);
            //_view.btnCheckPointLeft.onClick.AddListener(OnLeftBtnClicked);
        }

        private void OnUpPageBtnClicked()
        {
            mainPageIndex--;
            mainPageIndex = Mathf.Clamp(mainPageIndex, 0, maxPageIndex);

            InitDatas();
        }

        private void OnNextPageBtnClicked()
        {
            mainPageIndex++;
            mainPageIndex = Mathf.Clamp(mainPageIndex, 0, maxPageIndex);

            InitDatas();
        }

        private void OnCloseHandle()
        {
            _uGuiForm.Close();
            ProcessEventDefine.SelectTopicCall.SendMessage(3);

            GameManager.Instance.RestCinemachineCamera();
            GameManager.Instance.ShowAllParts();
            GameManager.Instance.ShowAllRenderer();
        }

        List<Toggle> toggles = new();

        private void InitDatas()
        {
            int desireCount = faultCheckConfig3Rds.Count - mainPageIndex * AppConst.UIConst.TrainMainCount;
            int contentCount = desireCount < AppConst.UIConst.TrainMainCount ? desireCount : AppConst.UIConst.TrainMainCount;

            for (int i = 0; i < AppConst.UIConst.TrainMainCount; i++)
            {
                Transform item = null;
                if (_view.tsContentList.childCount > i)
                    item = _view.tsContentList.GetChild(i);

                if (item == null)
                {
                    item = Object.Instantiate(_view.toggleItem, _view.tsContentList, false);
                }

                if (contentCount <= i)
                {
                    item.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    item.gameObject.SetActive(true);
                }

                TrainItem trainItem = item.GetComponent<TrainItem>();
                trainItem = trainItem != null ? trainItem : item.gameObject.AddComponent<TrainItem>();

                int desireIndex = mainPageIndex * AppConst.UIConst.TrainMainCount + i;
                FaultCheckConfig3rd faultCheckConfig3 = faultCheckConfig3Rds[desireIndex];
                trainItem.Init(faultCheckConfig3, On2ndSelectedAction);
                toggles.Add(trainItem.GetComponent<Toggle>());

                List<FaultCheckConfig4th> faultCheckConfig4Ths = _model.GetFaultCheckConfig4rds(faultCheckConfig3.Id);
                TopicManager.Instance.InitDic(faultCheckConfig4Ths);
            }

            GameManager.Instance.RestCinemachineCamera();
            GameManager.Instance.ShowAllParts();
            GameManager.Instance.ShowAllRenderer();

            _view.tsShowItem.gameObject.SetActive(false);

            //隐藏模型
            foreach (var item in _WaiKe)
            {
                item.gameObject.SetActive(false);
            }
        }

        List<GameObject> togCheckPointItems = new();
        FaultCheckConfig3rd faultCheckConfig3Rd;
        List<FaultCheckConfig4th> faultCheckConfig4ths;

        private void On2ndSelectedAction(int index, FaultCheckConfig3rd config, bool showPoint, bool isHand)
        {
            optionIndex = -1;
            optionIndexExtend = -1;
            _view.btnPageUp.transform.parent.gameObject.SetActive(true);
            _view.tsContentList.gameObject.SetActive(true);
            _view.tsShowItem.gameObject.SetActive(showPoint);
            if (!showPoint) return;
            if (faultCheckConfig3Rd != config)
            {
                ClearCheckPoints();

                //_view.tmptxtCheckPointTitle.text = config.Title;

                faultCheckConfig4ths = _model.GetFaultCheckConfig4rds(config.Id);

                TopicManager.Instance.InitDic(faultCheckConfig4ths);
                //Debug.LogError(config.Id + "________" + _showIndex + "<<<<<<<<" + faultCheckConfig4Ths.Count);
                for (int i = 0; i < faultCheckConfig4ths.Count; i++)
                {
                    Transform item = Object.Instantiate(_view.togCheckPointItem.transform, _view.togCheckPointItem.transform.parent, false);
                    item.gameObject.SetActive(true);
                    item.GetComponent<ToggleExtend>()?.Init(faultCheckConfig4ths[i].Title, false, GameManager.Instance.AssessmentMode == EnumAssessmentMode.Train);
                    item.GetComponent<ToggleExtend>()?.SetToggleInitState();
                    GameManager.Instance.InitModelParam(faultCheckConfig4ths[i]);
                    var checkPointItem = item.gameObject.AddComponent<CheckPointItem>();
                    checkPointItem.Init(faultCheckConfig4ths[i], _view._pointTip, OnCheckPointClickAction, GameManager.Instance.AssessmentMode != EnumAssessmentMode.Train);
                    togCheckPointItems.Add(item.gameObject);
                    //实训中的点不可点击
                    checkPointItem.SetTipClcikableState(false);
                }

                faultCheckConfig3Rd = config;
            }

            //处理任务发布的跳转逻辑
            if (GameManager.Instance.TrainType == TrainType.RenWuFaBu)
            {
                _view.btnPageUp.transform.parent.gameObject.SetActive(false);
                _view.tsContentList.gameObject.SetActive(false);
                _view.tsShowItem.gameObject.SetActive(false);
                if (GameEntry.UI.HasUIForm("UICommonPage"))
                {
                    GameEntry.UI.CloseUIForm<UICommonPage>();
                }

                //根据大类类型展示大类基本界面
                GameEntry.UI.OpenUIFormSync<UIDataScreen>(false, UIDataSceneOpenType.Help);
            }

            uiAreaType = 1;
            optionIndex = index;
            optionIndexExtend = optionIndex;
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel.Before");
            HandleMsg.Instance.UILevelBuilder(uiLevel, 4);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");

            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        /// <summary>
        /// 控制模型显隐
        /// </summary>
        private void OnCheckPointClickAction(CheckPointItem config4th, bool isOn, bool isHand)
        {
            //如果是不是训练模式不予理会
            if (GameManager.Instance.AssessmentMode != EnumAssessmentMode.Train)
            {
                return;
            }

            //显示必要的模型
            // GameManager.Instance.ShowAllParts();
            // //半透明化无关模型
            // //隐藏其他的模型
            // GameManager.Instance.HideAllWithoutTarget(config4th.ignoreList);
            ////半透明化无关模型
            //var temp = new List<Transform>();
            //for (int i = 0; i < config4th.CameraTargets.Count; i++)
            //{
            //    temp.Add(config4th.CameraTargets[i].TrackingTarget);
            //}
            //GameManager.Instance.ChangeAllMaterialWithTarget(temp);

            if (GameManager.Instance.AssessmentMode == EnumAssessmentMode.Train)
            {
                TopicManager.Instance.TrainCheckIsComplete(config4th.config.Id);
            }

            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            uiAreaType = 2;
            optionIndex = config4th.transform.GetSiblingIndex() - 1;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 6);
            HandleMsg.Instance.DebugUILevel(uiLevel, "SubUILevel");

            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        #endregion

        #region Field

        private void InitField()
        {
            //_subjectItems = new ListEx<SubjectItem>();

            Canvas canvas = _view.imgBg.GetComponentInParent<Canvas>();
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;

            _WaiKe.Clear();
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("waike").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("neishi").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("Wheels").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("YibiaoDianlu").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("DianLu").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("qiDongDianLu").gameObject);
        }

        #endregion

        #region InternalLogic

        private async UniTask GetSubjectData()
        {
            /*_subjectData = await HttpDownloader.GetSubjectList(AppConst.IDConst.TrainSubjectId,() =>
            {
                UniTask.Void(async () => { await GetSubjectData(); });
            });*/

            _subjectData = new List<SubjectData>()
            {
                new SubjectData()
                {
                    name = "故障1",
                    describe = "故障描述1"
                },
                new SubjectData()
                {
                    name = "故障2",
                    describe = "故障描述2"
                }
            };
        }

        private void GenerateSubject()
        {
            if (_subjectData == null || _subjectData.Count == 0) return;
            var goCache = GameEntry.Resource.BuildInResource.Load<GameObject>(AppConst.AssetPathConst.SubjectItem);

            //_subjectItems.UpdateItem(_subjectData, goCache, _view.tsContent);

            for (var i = 0; i < _subjectData.Count; i++)
            {
                //_subjectItems.self[i].InitData(_subjectData[i], EnumMonitorMode.Train);
                //_subjectItems.self[i].gameObject.SetActive(true);
            }
            //_scrollViewEx.Init();
            //_scrollViewEx.ResetScrollEx();
        }

        #endregion
    }
}