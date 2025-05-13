using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.ExcelData;
using System.Collections.Generic;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;
using Object = UnityEngine.Object;

namespace Hotfix.UI
{
    /// <summary>
    /// 课题列表界面
    /// </summary>
    public class UITopicList : UIForm<UIViewTopicList, UIModelTopicList>
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

        private UIViewTopicList _view;
        private UIModelTopicList _model;

        private int mainPageIndex = 0;

        private int maxPageIndex = Mathf.FloorToInt(FaultCheckConfig2ndTable.Instance.dataList.Count / AppConst.UIConst.TrainMainCount);

        private int _showIndex = 0;
        private int _showMaxIndex;
        //List<Button> toggles = new();

        private bool isNetGet;
        private List<FaultCheckConfig2nd> faultCheckConfig2ndCacheList;

        #endregion

        private void InitField()
        {
            //_subjectItems = new ListEx<SubjectItem>();

            Canvas canvas = _view.imgBg.GetComponentInParent<Canvas>();
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;
        }

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();
            InitField();

            _view.btnPageUp.onClick.AddListener(OnUpPageBtnClicked);
            _view.btnNextPage.onClick.AddListener(OnNextPageBtnClicked);
        }

        #region 被控消息扩展

        void OnControlMessage(MsgUINavigationData msg)
        {
            //平板点击的主模块
            if (_view.tsContentList.childCount > msg.optionIndex % AppConst.UIConst.TrainMainCount)
            {
                var faultCheckConfig2 = faultCheckConfig2ndCacheList[msg.optionIndex];
                On1stSelectedAction(msg.optionIndex, faultCheckConfig2, false);
                ServNet.Instance.SetCacheMsgBaseState(msg);
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

            Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>(moduleConfig.ClassName, (msg) => { OnControlMessage(msg); });

            #endregion

            base.OnOpen(userData);
            optionIndex = -1;
            _showIndex = 0;
            _showMaxIndex = 0;

            //初始化缓存列表数据
            isNetGet = (GameManager.Instance.MonitorMode == EnumMonitorMode.Assessment && GameManager.Instance.AssessmentMode == EnumAssessmentMode.Teacher);
            //var faultCheckConfig2ndCacheList = _model.GetFaultCheckConfig2nds(mainPageIndex, AppConst.UIConst.TrainMainCount);

            faultCheckConfig2ndCacheList = isNetGet ? _model.GetFaultCheckConfig2nds() : _model.GetFaultCheckConfig2nds((int)GameManager.Instance.TrainType);
            maxPageIndex = faultCheckConfig2ndCacheList.Count;

            InitDatas();
            ShowCommonPage();
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);

            ShowCommonPage();
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);

            CloseHandle();
        }

        private void ShowCommonPage()
        {
            var userData = new UICommonPageUserData
            {
                TabTitles = new[] { "首页", "课题列表" },
                BackLastPageAction = backLastPage,
                BackMainPageAction = backMainPage,
                LabelToggleRespondClickAction = labelToggleRespondOnClickAction
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
                int temp = optionIndex + 2;
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
                GameEntry.UI.OpenUIFormSync<UIAssessment>();
            }

            void backMainPage()
            {
                _uGuiForm.Close();
                GameEntry.UI.OpenUIFormSync<UIMainMenu>();
            }

            #endregion
        }


        private void OnNextPageBtnClicked()
        {
            if (_showMaxIndex + AppConst.UIConst.TrainMainCount < maxPageIndex)
            {
                _showMaxIndex += AppConst.UIConst.TrainMainCount;
            }

            InitDatas();
        }

        private void OnUpPageBtnClicked()
        {
            if (_showMaxIndex >= AppConst.UIConst.TrainMainCount)
            {
                _showMaxIndex -= AppConst.UIConst.TrainMainCount;
            }

            InitDatas();
        }

        private void InitDatas()
        {
            for (int i = 0; i < _view.tsContentList.childCount; i++)
            {
                Object.Destroy(_view.tsContentList.GetChild(i).gameObject);
            }

            _showIndex = _showMaxIndex;

            int contentCount = faultCheckConfig2ndCacheList.Count - _showMaxIndex < AppConst.UIConst.TrainMainCount ? faultCheckConfig2ndCacheList.Count - _showMaxIndex : AppConst.UIConst.TrainMainCount;

            for (int i = 0; i < contentCount; i++)
            {
                Transform item = null;
                if (faultCheckConfig2ndCacheList.Count > _showIndex)
                    item = Object.Instantiate(_view.toggleItem, _view.tsContentList, false);

                TopicItem trainItem = item.GetComponent<TopicItem>();
                trainItem = trainItem ? trainItem : item.gameObject.AddComponent<TopicItem>();

                FaultCheckConfig2nd faultCheckConfig2 = faultCheckConfig2ndCacheList[_showIndex];
                trainItem.Init(_showIndex, faultCheckConfig2, On1stSelectedAction);
                //toggles.Add(trainItem.GetComponent<Button>());
                _showIndex++;
            }
        }

        private void On1stSelectedAction(int index, FaultCheckConfig2nd config, bool isHand)
        {
            var userData = config;
            _uGuiForm.Close();

            //缓存索引，供与Pad端交互用
            TopicManager.Instance.HelperIndex = index;

            GameEntry.UI.OpenUIFormSync<UITrain>(false, userData);

            TopicManager.Instance.TopicStart(config.Id);

            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            //useless,这个界面只存在于没点击的时候

            uiAreaType = 1;
            optionIndex = index;

            HandleMsg.Instance.UILevelBuilder(uiLevel, 4, index);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");

            if (isHand)
            {
                //在这里发送交互的索引
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, -1);
            }
        }

        void CloseHandle()
        {
            for (int i = 0; i < _view.tsContentList.childCount; i++)
            {
                Object.Destroy(_view.tsContentList.GetChild(i).gameObject);
            }
        }
    }
}