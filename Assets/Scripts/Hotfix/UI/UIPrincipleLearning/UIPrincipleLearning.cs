using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using Hotfix.ExcelData;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using System.Collections.Generic;
using System;
using System.Linq;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;
using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 原理学习界面
    /// </summary>
    public class UIPrincipleLearning : UIForm<UIViewPrincipleLearning, UIModelPrincipleLearning>
    {
        private UGuiForm _uGuiForm;

        #region Component

        private UIViewPrincipleLearning _view;
        private UIModelPrincipleLearning _model;

        #endregion

        private bool isDragTimeSlider;
        private float currentPlaybackRatio;
        private InputAction videoPlayAction;

        private List<string> videoPaths = new();
        private List<string> modelPaths = new();
        private UICommonPageUserData _userData;

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

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm,
            UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            InitUIComponent();

            #region 控制和被控消息扩展

            moduleConfig = ModuleConfigTable.Instance.Get(4);
            HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
            Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>(moduleConfig.ClassName, (msg) => { OnControlMessage(msg); });

            #endregion
        }

        #region 被控消息扩展

        void OnControlMessage(MsgUINavigationData msg)
        {
            if (msg.uiAreaType == 1)
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
                else if (msg.uiLevel.Count == 3)
                {
                    if (_view.StructuralToggleGroup.transform.childCount > msg.optionIndex)
                    {
                        var temp = _view.StructuralToggleGroup.transform.GetChild(msg.optionIndex);
                        var item = temp.GetComponent<PrincipleItem>();
                        ShowPrincipleAll(true, item, false);
                        var tog = temp.GetComponent<Toggle>();
                        tog.SetIsOnWithoutNotify(true);
                        ServNet.Instance.SetCacheMsgBaseState(msg);
                    }
                }
            }
            //平板点击的子子模块
            else
            {
                if (_view._tsRight.childCount > msg.optionIndex)
                {
                    var temp = _view._tsRight.GetChild(msg.optionIndex);
                    var item = temp.GetComponent<PrincipleItem>();
                    OnPrincipleChange(true, item, false);
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
            optionIndexExtend = -1;
            // Debug.Log("<color=magenta>[原理]</color> 打开列表界面");
            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = null;
            // GameManager.Instance.RestCinemachineCamera();
            GameManager.Instance.RestCinemachineCameraPrincip("原理仿真");

            ResetUIComponent();
            ShowCommonPage();
        }

        public override void Close(object userData = null)
        {
            base.Close(userData);
            ModelCameraPositionTable.Instance.SaveData();
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (videoPlayAction.triggered)
            {
                ShowVideoPlayerState();
            }
        }

        public override void OnRefocus(object userData = null)
        {
            base.OnRefocus(userData);

            Debug.LogError("OnRefocus");
            ShowCommonPage();
        }

        #region UI

        private void InitUIComponent()
        {
            //主项目
            foreach (var config in MainStructureConfigTable.Instance.dataList)
            {
                var goCache = Object.Instantiate(_view.MainStructuralItemPrefab,
                    _view.MainStructuralToggleGroup.transform, false);
                var structureItem = goCache.GetComponent<MainStructureItem>();
                structureItem.InitData(config, _view.MainStructuralToggleGroup, OnMainStructureChange);
            }

            _view.videoStateBtn.onClick.AddListener(OnClickVideoRawImage);
            _view.videoVolumeSlider.onValueChanged.AddListener(OnVideoVolumeChange);
            _view.videoFastBackBtn.onClick.AddListener(OnVideoFastBackBtn);
            _view.videoPlayBtn.onClick.AddListener(OnVideoPlayBtn);
            _view.videoPauseBtn.onClick.AddListener(OnVideoPauseBtn);
            _view.videoFastForwardBtn.onClick.AddListener(OnVideoFastForwardBtn);

            _view.videoDragHelper.BeginDrag = OnTimeSliderBeginDrag;
            _view.videoDragHelper.EndDrag = OnTimeSliderEndDrag;

            videoPlayAction = InputSystem.actions.FindAction("VideoPlay");
            // DetailListPanelActive(false);
        }

        private void ResetUIComponent()
        {
            foreach (var item in _view.MainStructuralToggleGroup.ActiveToggles())
            {
                item.isOn = false;
            }

            foreach (var item in _view.StructuralToggleGroup.ActiveToggles())
            {
                item.isOn = false;
            }

            _view.StructuralPanel.gameObject.SetActive(false);
            _view.videoSlider.value = 0;
            _view.rawVideo.color = Color.clear;
            _view.videoSlider.gameObject.SetActive(false);
            _view.videoStateImage.gameObject.SetActive(false);
            _view.videoVolumeSlider.value = 0.87f;
            //_view.videoVolume.gameObject.SetActive(false);
            _view.videoPlayBtn.gameObject.SetActive(false);
            _view.videoPauseBtn.gameObject.SetActive(true);
            currentPlaybackRatio = 1.0f;
            _view.ratioText.text = string.Empty;
            _view.videoPlayer.playbackSpeed = 1.0f;
            isDragTimeSlider = false;
        }

        private void ShowCommonPage()
        {
            _userData.TabTitles = new[] { "首页", "原理仿真" };
            _userData.BackLastPageAction = () =>
            {
                _uGuiForm.Close();
                GameEntry.UI.OpenUIFormSync<UIMainMenu>();
                DetailListPanelActive(false);
                GameManager.Instance.InitModelAnim();
            };
            _userData.BackMainPageAction = () =>
            {
                _uGuiForm.Close();
                GameManager.Instance.InitModelAnim();
            };

            _userData.LabelToggleRespondClickAction = labelToggleRespondOnClickAction;
            _userData.IsNotShowDownLine = true;
            _userData.IsBackLastDialog = false;
            _userData.IsBackMainDialog = false;
            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, _userData);
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
                        DetailListPanelActive(false);
                        GameManager.Instance.InitModelAnim();
                        break;
                    case ELabelToggleType.Label2:
                        // 激活模型渲染
                        GameManager.Instance.InitModelAnim();
                        GameManager.Instance.ShowAllParts();
                        GameManager.Instance.ShowAllRenderer();
                        GameManager.Instance.RestCinemachineCameraPrincip("原理仿真");

                        // 重置UI界面
                        ResetUIComponent();
                        MainStructuralToggleGroupActive(true);
                        SetStructuralPanelActive(false);
                        DetailListPanelActive(false);
                        _userData.TabTitles = new[] { "首页", "原理仿真" };
                        var commonPageUI = GameEntry.UI.GetUIForm<UICommonPage>() as UICommonPage;
                        commonPageUI?.UpdateLabelActiveState(_userData);
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

                HandleMsg.Instance.DebugUILevel(uiLevel, "TabUILevel");

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
            optionIndexExtend = -1;
            if (!state)
            {
                Debug.Log($"<color=blue>[原理]</color> 关闭小部件列表，重置相机位置");
                _view.DescriptionText.text = string.Empty;
                _view.StructuralPanel.gameObject.SetActive(false);
                _view.videoSlider.value = 0;
                _view.rawVideo.color = Color.clear;
                _view.videoSlider.gameObject.SetActive(false);
                _view.videoStateImage.gameObject.SetActive(false);
                //_view.videoVolume.gameObject.SetActive(false);
                _view.videoPlayBtn.gameObject.SetActive(false);
                _view.videoPauseBtn.gameObject.SetActive(true);
                _view.videoPlayer.targetCameraAlpha = 0f;
                currentPlaybackRatio = 1.0f;
                _view.ratioText.text = string.Empty;
                _view.videoPlayer.playbackSpeed = 1.0f;
                isDragTimeSlider = false;
                _userData.BackLastPageAction = new Action(() => { _userData.LabelToggleRespondClickAction.Invoke(ELabelToggleType.Label1, isHand); });
                return;
            }

            var list = _model.PrincipleConfigs[itemC.StructureConfigData.Id];
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
                    item = Object.Instantiate(_view.StructuralItemPrefab.transform,
                        _view.StructuralToggleGroup.transform, false);
                }

                if (list.Count > i)
                {
                    var structureItem = item.GetComponent<PrincipleItem>();

                    //structureItem.InitData(list[i], _view.StructuralToggleGroup, OnPrincipleChange);
                    structureItem.InitData(list[i], _view.StructuralToggleGroup, ShowPrincipleAll);
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

            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            #region 标签显示  Owner: 王柏雁 2025-4-9

            GameEntry.UI.CloseUIForm<UICommonPage>();
            _userData.TabTitles = new[] { "首页", "原理仿真", itemC.StructureConfigData.Title };
            _userData.IsNotShowDownLine = true;
            _userData.BackLastPageAction = new Action(() => { _userData.LabelToggleRespondClickAction.Invoke(ELabelToggleType.Label2, false); });
            GameEntry.UI.OpenUIFormSync<UICommonPage>(false, _userData);

            #endregion

            uiAreaType = 1;
            optionIndex = itemC.transform.GetSiblingIndex();
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3, optionIndex);
            HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, -1);
            }
        }

        private void ShowPrincipleAll(bool state, PrincipleItem itemC, bool isHand)
        {
            optionIndex = -1;
            optionIndexExtend = -1;
            videoPaths.Clear();

            if (_view._tsRight.childCount > 0)
            {
                // for (int i = 0; i < _view._tsRight.childCount; i++)
                // {
                //     //Destroy不会立即重置SiblingIndex
                //     Object.DestroyImmediate(_view._tsRight.GetChild(i).gameObject);
                // }

                for (int i = _view._tsRight.childCount - 1; i >= 0; i--)
                {
                    //Destroy不会立即重置SiblingIndex
                    Object.DestroyImmediate(_view._tsRight.GetChild(i).gameObject);
                }
            }

            if (!state)
            {
                //_view._tsBottom.gameObject.SetActive(false);
                _view.DescriptionText.text = string.Empty;
                _view.videoSlider.value = 0;
                _view.rawVideo.color = Color.clear;
                _view.videoSlider.gameObject.SetActive(false);
                _view.videoStateImage.gameObject.SetActive(false);
                //_view.videoVolume.gameObject.SetActive(false);
                _view.videoPlayBtn.gameObject.SetActive(false);
                _view.videoPauseBtn.gameObject.SetActive(true);
                _view.videoPlayer.targetCameraAlpha = 0f;
                currentPlaybackRatio = 1.0f;
                _view.ratioText.text = string.Empty;
                _view.videoPlayer.playbackSpeed = 1.0f;
                isDragTimeSlider = false;
                return;
            }

            //string filePath = itemC.PrincipleConfigData.VideoPath;
            //Debug.Log(filePath);
            //if (filePath.Contains("|"))
            //{
            //    videoPaths = filePath.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //}
            //else videoPaths.Add(filePath);

            videoPaths = GetPaths(itemC.PrincipleConfigData.VideoPath);
            modelPaths = GetPaths(itemC.PrincipleConfigData.ModelPath);

            // _view._tsBottom.gameObject.SetActive(true);
            DetailListPanelActive(true);

            // 打开右侧部件列表
            if (videoPaths.Count > 0)
            {
                for (int i = 0; i < videoPaths.Count; i++)
                {
                    Transform item;
                    item = Object.Instantiate(_view.StructuralItemPrefab.transform, _view._tsRight, false);
                    item.GetComponent<Toggle>().isOn = false;
                    var structureItem = item.GetComponent<PrincipleItem>();

                    structureItem.InitData(itemC.PrincipleConfigData, _view.BottomToggleGroup, OnPrincipleChange,
                        videoPaths[i], modelPaths[i]);
                    structureItem.gameObject.SetActive(true);
                }
            }

            uiAreaType = 1;
            optionIndex = itemC.transform.GetSiblingIndex();
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3);
            HandleMsg.Instance.DebugUILevel(uiLevel, "SubUILevel");
            optionIndexExtend = optionIndex;
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        List<string> GetPaths(string path)
        {
            List<string> paths = new();
            if (path.Contains("|"))
            {
                paths = path.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else paths.Add(path);

            return paths;
        }

        private void OnPrincipleChange(bool state, PrincipleItem itemC, bool isHand)
        {
            optionIndex = -1;
            if (!state)
            {
                _view.DescriptionText.text = string.Empty;
                return;
            }

            // Debug.Log($"<color=green>[原理]</color> 打开结构：{itemC.PrincipleConfigData.Title}");

            _view.DescriptionText.text = itemC.PrincipleConfigData.Description;

            //播放视频
            if (_view.videoPlayer.isPlaying)
            {
                _view.videoPlayer.Stop();
                _view.videoSlider.value = 0;
            }

            //_view.videoPlayer.url = string.Format("{0}/{1}{2}", Application.streamingAssetsPath, AppConst.AssetPathConst.VideoPath, itemC.PrincipleConfigData.VideoPath);
            //_view.videoPlayer.clip = Resources.Load<VideoClip>(AppConst.AssetPathConst.ResourceVideoPath + itemC.PrincipleConfigData.VideoPath);
            _view.videoPlayer.clip = Resources.Load<VideoClip>(AppConst.AssetPathConst.ResourceVideoPath + itemC.videoName);
            _view.rawVideo.color = Color.white;
            _view.videoSlider.gameObject.SetActive(true);
            _view.videoPlayer.targetCameraAlpha = 0.5f;
            //_view.videoVolume.gameObject.SetActive(true);
            currentPlaybackRatio = 1.0f;
            _view.ratioText.text = string.Empty;
            _view.videoPlayer.playbackSpeed = 1.0f;
            _view.videoPlayer.Play();

            //Debug.Log(itemC.modelPath);

            // 打开模型
            if (!string.IsNullOrEmpty(itemC.modelPath) && itemC.modelPath != "")
            {
                // Debug.Log($"<color=green>[原理]</color> 打开模型：{itemC.text}");
                // 模型对应的相机位置 Owner: 王柏雁 2025-4-11 
                GameManager.Instance.RestCinemachineCameraPrincip(itemC.text);
                GameManager.Instance.ModelAnimController(itemC.modelPath);
            }
            else
            {
                // Tips: 左侧列表点击回调
                Debug.Log($"<color=green>[原理]</color> 左侧列表：{itemC.PrincipleConfigData.Title}");
            }

            PlayUIEffect(AppConst.AssetPathConst.ClickSound);

            uiAreaType = 2;
            optionIndex = itemC.transform.GetSiblingIndex();
            HandleMsg.Instance.UILevelBuilder(uiLevel, 3);
            HandleMsg.Instance.DebugUILevel(uiLevel, "SubSubUILevel");

            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (_view.videoPlayer.isPlaying && !isDragTimeSlider)
            {
                _view.videoSlider.value = (float)_view.videoPlayer.frame / _view.videoPlayer.frameCount;
            }
        }

        private void OnClickVideoRawImage()
        {
            ShowVideoPlayerState();
        }

        private void OnVideoVolumeChange(float value)
        {
            for (ushort i = 0; i < _view.videoPlayer.audioTrackCount; i++)
            {
                _view.videoPlayer.SetDirectAudioVolume(i, value);
            }
        }

        private void OnVideoFastBackBtn()
        {
            currentPlaybackRatio -= AppConst.UIConst.DeltaPlaybackRatio;
            SetPlayBackRatio();
        }

        private void SetPlayBackRatio()
        {
            currentPlaybackRatio = Mathf.Clamp(currentPlaybackRatio, AppConst.UIConst.DeltaPlaybackRatio,
                AppConst.UIConst.MaxPlaybackRatio);
            _view.videoPlayer.playbackSpeed = currentPlaybackRatio;

            if (currentPlaybackRatio != 1.0f)
            {
                _view.ratioText.text = string.Format("{0}X", currentPlaybackRatio);
            }
            else
            {
                _view.ratioText.text = string.Empty;
            }
        }

        private void OnVideoPlayBtn()
        {
            ShowVideoPlayerState();
        }

        private void OnVideoPauseBtn()
        {
            ShowVideoPlayerState();
        }

        private void OnVideoFastForwardBtn()
        {
            currentPlaybackRatio += AppConst.UIConst.DeltaPlaybackRatio;
            SetPlayBackRatio();
        }

        private void ShowVideoPlayerState()
        {
            if (_view.videoPlayer.isPlaying)
            {
                _view.videoPlayer.Pause();
                _view.videoStateImage.gameObject.SetActive(true);
                _view.videoPlayBtn.gameObject.SetActive(true);
                _view.videoPauseBtn.gameObject.SetActive(false);
            }
            else
            {
                _view.videoPlayer.Play();
                _view.videoStateImage.gameObject.SetActive(false);
                _view.videoPlayBtn.gameObject.SetActive(false);
                _view.videoPauseBtn.gameObject.SetActive(true);
            }
        }

        private void OnTimeSliderBeginDrag()
        {
            isDragTimeSlider = true;
        }

        private void OnTimeSliderEndDrag()
        {
            _view.videoPlayer.frame = (long)(_view.videoSlider.value * _view.videoPlayer.frameCount);

            //TODO等待缓存完成
            //_view.videoPlayer.frameDropped = 
            isDragTimeSlider = false;
        }

        /// <summary>
        /// 细节部件结构视图激活状态 Owner: 王柏雁 2025-4-9 
        /// </summary>
        /// <param name="state"></param>
        public void SetStructuralPanelActive(bool state)
        {
            // Debug.Log($"细节部件结构视图激活状态: {state}");
            _view.StructuralPanel.gameObject.SetActive(state);
        }

        /// <summary>
        /// 主模块结构视图激活状态 Owner: 王柏雁 2025-4-9 
        /// </summary>
        /// <param name="state"></param>
        public void MainStructuralToggleGroupActive(bool state)
        {
            // Debug.Log($"主模块结构视图激活状态: {state}");
            _view.MainStructuralToggleGroup.gameObject.SetActive(state); // 关闭主结构切视图

            // 关闭主结构面板里的项选中状态
            foreach (var item in _view.MainStructuralToggleGroup.ActiveToggles())
            {
                item.isOn = false;
            }

            DetailListPanelActive(false); // 关闭细节列表视图
        }

        /// <summary>
        /// 细节列表视图激活状态 Owner: 王柏雁 2025-4-9 
        /// </summary>
        /// <param name="state"></param>
        private void DetailListPanelActive(bool state)
        {
            // Debug.Log($"细节列表视图激活状态: {state}");
            _view._tsRight.transform.parent.gameObject.SetActive(state);
        }
    }
}