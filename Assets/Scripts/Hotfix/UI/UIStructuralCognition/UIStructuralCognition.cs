using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.ExcelData;
using System.Collections.Generic;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;
using UnityEngine.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 结构认知界面
    /// </summary>
    public class UIStructuralCognition : UIForm<UIViewStructuralCognition, UIModelStructuralCognition>
    {
        private UGuiForm _uGuiForm;
        private UICommonPageUserData userData;

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

        /// <summary>
        /// 上次点击的子控件
        /// </summary>
        private StructureItem lastStructureItem;

        /// <summary>
        /// 缓存上次改变模型显隐状态
        /// </summary>
        private bool lastShowOrHideToggleState;

        #region Component

        private UIViewStructuralCognition _view;
        private UIModelStructuralCognition _model;

        #endregion

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            //InitUIComponent();

            #region 控制和被控消息扩展

            moduleConfig = ModuleConfigTable.Instance.Get(3);
            HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
            Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>(moduleConfig.ClassName, (msg) => { OnControlMessage(msg); });

            #endregion
        }

        #region 被控消息扩展

        void OnControlMessage(MsgUINavigationData msg)
        {
            //平板点击的主模块
            if (msg.uiLevel.Count == 2)
            {
                if (_view.MainStructuralToggleGroup.transform.childCount > msg.optionIndex)
                {
                    var temp = _view.MainStructuralToggleGroup.transform.GetChild(msg.optionIndex).GetComponent<MainStructureItem>();
                    OnMainStructureChange(true, temp, false);
                    ServNet.Instance.SetCacheMsgBaseState(msg);
                }
            }
            //平板点击的子模块
            else
            {
                if (_view.StructuralToggleGroup.transform.childCount > msg.optionIndex)
                {
                    var temp = _view.StructuralToggleGroup.transform.GetChild(msg.optionIndex);
                    var item = temp.GetComponent<StructureItem>();
                    OnStructureChange(true, item, false);
                    var tog = temp.GetComponent<Toggle>();
                    tog.SetIsOnWithoutNotify(true);
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

        public override void OnOpen(object userDatas)
        {
            base.OnOpen(userDatas);

            optionIndex = -1;
            // Debug.Log("<color=cyan>[结构]</color> 打开列表界面 <结构认知>");

            InitUIComponent();
            ResetUIComponent();
            ShowCommonPage();
            // GameManager.Instance.RestCinemachineCamera();// 重置相机位置(注释) Owner: 王柏雁 2025-4-11
            GameManager.Instance.RestCinemachineCameraStructral("结构认知");
            Ctrl_MessageCenter.SendMessage("OnOtherModelShowModeChange", false);
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);

            ShowCommonPage();
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);
            
            lastStructureItem = null;
            ModelCameraPositionTable.Instance.SaveData();
            GameManager.Instance.LastCachedRenders.Clear();
        }

        #region UI

        private void InitUIComponent()
        {
            if (_view.MainStructuralToggleGroup.transform.childCount == MainStructureConfigTable.Instance.dataList.Count)
            {
                for (int i = 0; i < _view.MainStructuralToggleGroup.transform.childCount; i++)
                {
                    var structureItem = _view.MainStructuralToggleGroup.transform.GetChild(i).GetComponent<MainStructureItem>();
                    structureItem.InitData(MainStructureConfigTable.Instance.dataList[i], _view.MainStructuralToggleGroup, OnMainStructureChange);
                }

                return;
            }

            //主项目
            foreach (var config in MainStructureConfigTable.Instance.dataList)
            {
                var goCache = Object.Instantiate(_view.MainStructuralItemPrefab, _view.MainStructuralToggleGroup.transform, false);
                var structureItem = goCache.GetComponent<MainStructureItem>();
                structureItem.InitData(config, _view.MainStructuralToggleGroup, OnMainStructureChange);
            }
        }

        private void OnOtherModelShowModeChange(bool isOn)
        {
            lastShowOrHideToggleState = isOn;
            if (isOn)
            {
                GameManager.Instance.HideAllWithoutTarget(GameManager.Instance.LastCachedRenders);
            }
            else
            {
                GameManager.Instance.ShowAllParts();
                if (lastStructureItem != null)
                {
                    //隐藏其他的模型
                    GameManager.Instance.HideAllWithoutTarget(lastStructureItem.IgnoreList, false);
                    GameManager.Instance.ChangeAllMaterialWithTarget(lastStructureItem.IgnoreList, false);
                    GameManager.Instance.ChangeAllMaterialWithTarget(GameManager.Instance.LastCachedRenders);
                }

                //GameManager.Instance.ChangeAllMaterialWithTarget(GameManager.Instance.LastCachedRenders);
            }
        }

        private void ResetUIComponent()
        {
            Debug.Log("OnOpen  Is  Running");
            foreach (var item in _view.MainStructuralToggleGroup.ActiveToggles())
            {
                item.isOn = false;
            }

            foreach (var item in _view.StructuralToggleGroup.ActiveToggles())
            {
                item.isOn = false;
            }

            _view.StructuralPanel.gameObject.SetActive(false);

            GameManager.Instance.ShowAllParts();
            GameManager.Instance.ShowAllRenderer();
            //GameManager.Instance.ViewMainCamera.Target = GameManager.Instance.ViewMainTarget;
            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = GameManager.Instance.MainTarget.Find("InitLookPos");
            //TODO...重置模型
        }

        private void ShowCommonPage()
        {
            userData = new UICommonPageUserData
            {
                TabTitles = new[] { "首页", "结构认知" },
                BackLastPageAction = () =>
                {
                    Debug.Log("<color=cyan>[结构]</color> 关闭列表界面 <结构认知>");
                    _uGuiForm.Close();
                    GameEntry.UI.OpenUIFormSync<UIMainMenu>();
                    GameManager.Instance.ShowAllParts();
                    GameManager.Instance.ShowAllRenderer();
                    GameManager.Instance.RestCinemachineCamera();
                },
                BackMainPageAction = () => { _uGuiForm.Close(); },
                LabelToggleRespondClickAction = labelToggleRespondOnClickAction,
                IsNotShowDownLine = true,
                IsBackMainDialog = false,
                IsNotShowSetting = true,
                ModelShowOrHideAction = OnOtherModelShowModeChange
            };
            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, userData);
            return;

            #region UI标签事件功能处理 Owner: 王柏雁 2025-4-12

            //标签切换响应点击事件
            void labelToggleRespondOnClickAction(ELabelToggleType labelType, bool isHand)
            {
                switch (labelType)
                {
                    case ELabelToggleType.Label1:
                        _uGuiForm.Close();
                        GameEntry.UI.OpenUIFormSync<UIMainMenu>();
                        GameManager.Instance.ShowAllParts();
                        GameManager.Instance.ShowAllRenderer();
                       
                        break;
                    case ELabelToggleType.Label2:
                        ResetUIComponent();
                        GameManager.Instance.RestCinemachineCameraStructral("结构认知");
                        MainStructuralToggleGroupActive(true);
                        userData.TabTitles = new[] { "首页", "结构认知" };
                        var commonPageUI = GameEntry.UI.GetUIForm<UICommonPage>() as UICommonPage;
                        commonPageUI?.UpdateLabelActiveState(userData);
                        break;
                }

                uiAreaType = 0;
                optionIndex = (int)labelType;
                if (optionIndex < 2)
                {
                    HandleMsg.Instance.UILevelBuilder(uiLevel, 2);
                }
                else
                {
                    HandleMsg.Instance.UILevelBuilder(uiLevel, 3, 0);
                }

                HandleMsg.Instance.DebugUILevel(uiLevel, "TabLevel");

                if (isHand)
                {
                    SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
                }
            }

            #endregion
        }

        #endregion

        private void OnMainStructureChange(bool state, MainStructureItem itemC, bool isHand)
        {
            optionIndex = -1;
            if (!state)
            {
                // Debug.Log($"<color=blue>[认知]</color> 关闭小部件列表，重置相机位置");
                GameManager.Instance.RestCinemachineCameraStructral("结构认知"); // 重置相机位置 Owner: 王柏雁 2025-4-11
                _view.StructuralPanel.gameObject.SetActive(false);
                _view.DescriptionText.text = string.Empty;
                userData.BackLastPageAction = new System.Action(() => { userData.LabelToggleRespondClickAction.Invoke(ELabelToggleType.Label1, isHand); });
                Ctrl_MessageCenter.SendMessage("OnOtherModelShowModeChange", false);
                return;
            }

            var list = _model.StructureConfigs[itemC.StructureConfigData.Id];
            var count = _view.StructuralToggleGroup.transform.childCount;
            var helper = list.Count >= count ? list.Count : count;

            for (int i = 0; i < helper; i++)
            {
                Transform item;
                if (count > i)
                {
                    item = _view.StructuralToggleGroup.transform.GetChild(i);
                }
                else
                {
                    item = Object.Instantiate(_view.StructuralItemPrefab.transform, _view.StructuralToggleGroup.transform, false);
                }

                if (list.Count > i)
                {
                    var structureItem = item.GetComponent<StructureItem>();

                    structureItem.InitData(list[i], _view.StructuralToggleGroup, OnStructureChange, itemC.CameraTarget.TrackingTarget);
                    structureItem.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }

            //打开界面
            _view.StructuralPanel.gameObject.SetActive(true);
            _view.MainStructuralToggleGroup.gameObject.SetActive(false); // 关闭主结构切视图 Owner: 王柏雁 2025-4-9 
            _view.DescriptionText.text = itemC.StructureConfigData.Description;

            //锁定摄像机
            GameManager.Instance.ViewMainCamera.Target = itemC.CameraTarget;
            //显示必要的模型
            GameManager.Instance.ShowAllParts();
            if (lastShowOrHideToggleState)
            {
                GameManager.Instance.HideAllWithoutTarget(new List<Transform>() { itemC.CameraTarget.TrackingTarget });
            }
            else
            {
                //半透明化无关模型
                GameManager.Instance.ChangeAllMaterialWithTarget(new List<Transform>()
                    { itemC.CameraTarget.TrackingTarget });
            }

            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            GameEntry.UI.CloseUIForm<UICommonPage>();
            userData.TabTitles = new[] { "首页", "结构认知", itemC.StructureConfigData.Title };
            userData.BackLastPageAction = new System.Action(() => { userData.LabelToggleRespondClickAction.Invoke(ELabelToggleType.Label2, false); });
            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, userData);

            uiAreaType = 1;
            optionIndex = itemC.transform.GetSiblingIndex();

            HandleMsg.Instance.UILevelBuilder(uiLevel, 3, optionIndex);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainLevel");

            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, -1);
            }
        }

        private void OnStructureChange(bool state, StructureItem itemC, bool isHand)
        {
            if (!state)
            {
                _view.DescriptionText.text = string.Empty;
                lastStructureItem = null;
                Ctrl_MessageCenter.SendMessage("OnOtherModelShowModeChange", false);
                return;
            }

            // Debug.Log($"<color=yellow>[认知]</color> 打开结构：{itemC.StructureConfigData.Title}");
            GameManager.Instance.RestCinemachineCameraStructral(itemC.StructureConfigData.Title);
            lastStructureItem = itemC;
            Ctrl_MessageCenter.SendMessage("OnOtherModelShowModeChange", true);

            _view.DescriptionText.text = itemC.StructureConfigData.Description;

            //做摄像机的变换等 
            GameManager.Instance.ViewMainCamera.Target = itemC.CameraTargets[0];
            //隐藏其他的模型
            GameManager.Instance.HideAllWithoutTarget(itemC.IgnoreList);
            //半透明化无关模型
            var temp = new List<Transform>();
            for (int i = 0; i < itemC.CameraTargets.Count; i++)
            {
                temp.Add(itemC.CameraTargets[i].TrackingTarget);
            }

            if (lastShowOrHideToggleState)
            {
                GameManager.Instance.HideAllWithoutTarget(temp);
            }
            else
            {
                //半透明化无关模型
                GameManager.Instance.ChangeAllMaterialWithTarget(temp);
            }

            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            uiAreaType = 1;
            optionIndex = itemC.transform.GetSiblingIndex();
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3);
            HandleMsg.Instance.DebugUILevel(uiLevel, "SubLevel");

            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        /// <summary>
        /// 细节部件结构视图激活状态 Owner: 王柏雁 2025-4-9 
        /// </summary>
        /// <param name="state"></param>
        public void SetStructuralPanelActive(bool state)
        {
            _view.StructuralPanel.gameObject.SetActive(state);
        }

        /// <summary>
        /// 主模块结构视图激活状态 Owner: 王柏雁 2025-4-9 
        /// </summary>
        /// <param name="state"></param>
        public void MainStructuralToggleGroupActive(bool state)
        {
            _view.MainStructuralToggleGroup.gameObject.SetActive(state); // 关闭主结构切视图

            // 关闭柱结构项的选中状态
            foreach (var item in _view.MainStructuralToggleGroup.ActiveToggles())
            {
                item.isOn = false;
            }
        }
    }
}