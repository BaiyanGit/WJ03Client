using UnityEngine;
using Wx.Runtime.UI;
using GameMain.Runtime.UI;
using System.Collections.Generic;
using UnityEngine.UI;
using XCharts.Runtime;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;
using UI.NetworkUI;

namespace Hotfix.UI
{
    /// <summary>
    /// 传感器监控
    /// </summary>
    public class UIDataScreen : UIForm<UIViewDataScreen, UIModelDataScreen>
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
        /// 当前的主分支类型
        /// </summary>
        private int currentMainType;

        #endregion

        #region Component

        private UIViewDataScreen _view;
        private UIModelDataScreen _model;

        List<Transform> infoItems = new();


        Dictionary<int, List<string>> tmpInfoItems = new Dictionary<int, List<string>>
        {
            { 0, new List<string> { "正常", "发动机数据正常" } },
            { 1, new List<string> { "正常", "底盘数据正常" } },
            { 2, new List<string> { "正常", "电气数据正常" } }
        };

        List<GameObject> _WaiKe = new();

        //缓存Slider组件
        private Slider sTemperatureSlider;
        private Slider sNoiseSlider;

        /// <summary>
        /// 当前激活的界面
        /// </summary>
        private GameObject _currentView;

        /// <summary>
        /// 界面打开数据缓存用于将界面多用化
        /// </summary>
        private object userDataCache;

        /// <summary>
        /// 辅助时间间隔的计数
        /// </summary>
        private float timeSinceStartup;

        private DataCheckConfig currentConfig;

        private int pageIndex;

        #endregion

        public override void OnInit(string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, UIFormLogic handle)
        {
            base.OnInit(uiFormAssetName, uiGroup, pauseCoveredUIForm, handle);

            _uGuiForm = (UGuiForm)Handle;
            _view = GetView();
            _model = GetModel();

            //注册关闭逻辑
            _view.btnClose.onClick.AddListener(() => { OnCloseAction(); });

            //按大类定义预设大类的点击反应
            for (int i = 0; i < _view.tsToggleList.childCount; i++)
            {
                int numID = i;
                _view.tsToggleList.GetChild(i).GetComponent<ToggleWithID>().InitID(numID, ChangeScreen);
            }

            //注册检查点点击的回调（点击检测点打开新的子界面）
            Ctrl_MessageCenter.AddMsgListener<DataCheckConfig, bool>("OnDataCheckPointClick", SetTipItem);

            //数据回调
            Ctrl_MessageCenter.AddMsgListener<float>("缸温", OnGangWenChange);
            Ctrl_MessageCenter.AddMsgListener<float>("气压", OnQiYaChange);
            Ctrl_MessageCenter.AddMsgListener<float>("压力", OnYaLiChange);
            Ctrl_MessageCenter.AddMsgListener<float>("油压", OnYouYaChange);
            Ctrl_MessageCenter.AddMsgListener<float>("油温", OnYouWenChange);
            Ctrl_MessageCenter.AddMsgListener<float>("油量", OnYouLiangChange);
            Ctrl_MessageCenter.AddMsgListener<float, float, float>("震动频率", OnZhenDongPinLvChange);
            Ctrl_MessageCenter.AddMsgListener<float>("温度", OnWenDuChange);
            Ctrl_MessageCenter.AddMsgListener<float>("噪声", OnZaoShengChange);
            Ctrl_MessageCenter.AddMsgListener<int, int, int>("Speed-Mileage-Electricity", OnZhenDongPinLvChange);
            Ctrl_MessageCenter.AddMsgListener<float>("指针旋转-angle", OnUpdateNeedleRotationAngle);
            Ctrl_MessageCenter.AddMsgListener<float>("电压", OnVoltageChange);

            //RemoteControl();

            //缓存Slider组件
            sTemperatureSlider = _view.tsTemperatureSlider.GetComponent<Slider>();
            sNoiseSlider = _view.tsNoiseSlider.GetComponent<Slider>();
        }

        #region 被控消息扩展

        void OnControlMessage(MsgUINavigationData msg)
        {
            //平板点击的主模块（发动机，底盘，电气）
            if (msg.uiLevel.Count == 2)
            {
                ChangeScreen(msg.optionIndex, true, false);
            }
            //平板点击的子模块
            else if (msg.uiLevel.Count == 3)
            {
                //用ChangeScreen修改过的optionIndex变量
                var dataList = _model.GetDataCheckConfigs(currentMainType + 1);
                if (dataList.Count > msg.optionIndex)
                {
                    SetTipItem(dataList[msg.optionIndex], false);
                }
                else
                {
                    Debug.LogErrorFormat("平板点击的子模块超出范围：{0}", msg.optionIndex);
                }
            }
            //平板点击传感器子模块（采集）
            else if (msg.uiLevel.Count == 4)
            {
                Ctrl_MessageCenter.SendMessage("PadOnSensorInteraction", msg);
            }

            ServNet.Instance.SetCacheMsgBaseState(msg);
        }

        void OnPadCloseAction(MsgBase msg)
        {
            //_view.btnClose.onClick.Invoke();
            switch (pageIndex)
            {
                case 3:
                    OnBackTo(currentConfig, false);
                    break;
                case 2:
                    OnBackToLast(false);
                    break;
                case 1:
                    OnCloseToMain(false);
                    break;
            }

            ServNet.Instance.SetCacheMsgBaseState(msg);
        }

        #endregion

        private void OnCloseAction()
        {
            _uGuiForm.Close();
        }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Object.Destroy(Handle.gameObject);
        }

        public override void OnOpen(object userData)
        {
            userDataCache = userData;

            UIDataSceneOpenType type = UIDataSceneOpenType.Pure;
            //显隐大类列表
            if (userDataCache != null)
            {
                type = (UIDataSceneOpenType)userDataCache;
                if (type == UIDataSceneOpenType.Help)
                {
                    _view.tsToggleList.gameObject.SetActive(false);
                }
            }
            else
            {
                _view.tsToggleList.gameObject.SetActive(true);
            }

            base.OnOpen(userData);
            optionIndex = -1;
            pageIndex = 1;
            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = GameManager.Instance.MainTarget.Find("DataScreenLookPos");

            //GameManager.Instance.ViewMainCamera.ForceCameraPosition(new Vector3(5f, 1.923f, 7.45f), Quaternion.Euler(new Vector3(36f, -159.1f, 0)));

            GameManager.Instance.CinemachineCameraController.ResetCameraPosition(new Vector3(3.54f, 4.4f, 4.67f),
                new Vector3(36f, -159.1f, 0), 7.5f, new Vector2(2, 10), Vector3.zero);

            GameManager.Instance.ShowAllParts();
            GameManager.Instance.ShowAllRenderer();

            _WaiKe.Clear();
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("waike").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("neishi").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("Wheels").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("YibiaoDianlu").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("DianLu").gameObject);
            _WaiKe.Add(GameManager.Instance.MainTarget.Find("qiDongDianLu").gameObject);
            foreach (var item in _WaiKe)
            {
                item.gameObject.SetActive(false);
            }

            //初始化
            //关掉当前界面
            _currentView?.SetActive(false);
            //打开默认界面
            _view.tsAllView.gameObject.SetActive(true);

            InitLogDataItem();

            //发送数据监控界面打开事件
            Ctrl_MessageCenter.SendMessage("OnUIDataScreenOpen", type);

            #region 控制和被控消息扩展

            moduleConfig = ModuleConfigTable.Instance.Get(9);
            if (userDataCache == null)
            {
                HandleMsg.Instance.PadAdpaterParmsToUILevel(ref uiLevel, moduleConfig.AdapterParam);
                Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>(moduleConfig.ClassName, OnControlMessage);
                Ctrl_MessageCenter.AddMsgListener<int>("OnAnyCheckItemChecked", OnAnyCheckItemChecked);
                //模拟页签点击
                Ctrl_MessageCenter.AddMsgListener<MsgUINavigationData>("PadUICommonPageTab", (msg) =>
                {
                    switch (msg.optionIndex)
                    {
                        case 0:
                            OnCloseToMain(false);
                            break;
                        case 1:
                            OnBackToLast(false);
                            break;
                        case 2:
                            OnBackTo(currentConfig, false);
                            break;
                    }

                    ServNet.Instance.SetCacheMsgBaseState(msg);
                });
            }

            Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadChange2LastPage", OnPadCloseAction);

            #endregion
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            TimeNowShow();
        }

        public override void Close(object userData = null)
        {
            #region 控制和被控消息扩展

            if (userDataCache != null)
            {
                Ctrl_MessageCenter.RemoveMsgListener<MsgUINavigationData>(moduleConfig.ClassName, OnControlMessage);
                Ctrl_MessageCenter.RemoveMsgListener<int>("OnAnyCheckItemChecked", OnAnyCheckItemChecked);
            }

            Ctrl_MessageCenter.RemoveMsgListener<MsgBase>("PadChange2LastPage", OnPadCloseAction);

            #endregion

            base.Close(userData);
            foreach (var item in _WaiKe)
            {
                item.gameObject.SetActive(true);
            }

            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = null;
            GameManager.Instance.RestCinemachineCamera();

            #region 实训-任务发布扩展适配

            UIDataSceneOpenType type = UIDataSceneOpenType.Pure;
            if (userDataCache != null)
            {
                type = (UIDataSceneOpenType)userDataCache;
            }

            #endregion

            //初始化
            //关掉当前界面
            _currentView?.SetActive(false);
            //打开默认界面
            _view.tsAllView.gameObject.SetActive(true);

            //发送数据监控界面关闭事件
            Ctrl_MessageCenter.SendMessage("OnUIDataScreenClose", type);
        }

        /// <summary>
        /// 初始化日志数据
        /// </summary>
        void InitLogDataItem()
        {
            foreach (var item in infoItems)
            {
                Object.Destroy(item.gameObject);
            }

            infoItems.Clear();

            List<DataCheckConfig> defaultValue = DataCheckTopicManager.Instance.DataCheckDefaultValue;
            Debug.Log(defaultValue.Count);

            Dictionary<int, List<string>> keyValuePairs = new Dictionary<int, List<string>>();
            defaultValue.ForEach(config =>
            {
                string tmpTitle = _model.GetMainStructureConfigTableTitle(config.Type);
                if (!keyValuePairs.ContainsKey(config.Type - 1))
                {
                    keyValuePairs.Add(config.Type - 1, new List<string>() { "错误", tmpTitle + "出现故障" });
                }
            });

            for (int i = 0; i < tmpInfoItems.Count; i++)
            {
                Transform infoItem = Object.Instantiate(_view.tsInfoItem, _view.tsInfoItem.parent);
                DataScreenInfoItem dataScreen = infoItem.gameObject.AddComponent<DataScreenInfoItem>();
                if (!keyValuePairs.ContainsKey(i))
                {
                    dataScreen.InitData(tmpInfoItems[i][0], tmpInfoItems[i][1]);
                }
                else
                {
                    dataScreen.InitData(keyValuePairs[i][0], keyValuePairs[i][1]);
                }

                infoItems.Add(infoItem);
            }

            _currentView = _view.tsAllView.gameObject;
        }

        void TimeNowShow()
        {
            if (Time.realtimeSinceStartup - timeSinceStartup >= 0.7f)
            {
                _view.tmpTimeNow.text = System.DateTime.Now.ToString();
                timeSinceStartup = Time.realtimeSinceStartup;
            }
        }

        private void OnGangWenChange(float value)
        {
            DataCheckTopicManager.Instance.UpdateLineData(_view._cylinderTemperatureChart, "缸温", value);
        }

        private void OnQiYaChange(float value)
        {
            DataCheckTopicManager.Instance.UpdateLineData(_view._kPALineChart, "气压", value);

            //_view._kPALineChart.AddData("气压", value);
            //_view._electricKPALineChart.AddData("气压", value);
        }

        private void OnYaLiChange(float value)
        {
            DataCheckTopicManager.Instance.UpdateLineData(_view._aPSLineChart, "压力", value);
        }

        private void OnYouYaChange(float value)
        {
            DataCheckTopicManager.Instance.UpdateLineData(_view._oilKPAChart, "油压", value);
        }

        private void OnYouWenChange(float value)
        {
            DataCheckTopicManager.Instance.UpdateLineData(_view._oilTemperatureChart, "油温", value);
            //UpdateLineData(_view._oilTemperatureChart, "油温", value);
        }

        private void OnYouLiangChange(float value)
        {
            _view._oilChart.AddData("油量", value);
        }

        private void OnZhenDongPinLvChange(float x, float y, float z)
        {
            _view.tmptxt_OffsetX.text = x.ToString("F1");
            _view.tmptxt_OffsetY.text = y.ToString("F1");
            _view.tmptxt_OffsetZ.text = z.ToString("F1");
        }

        private void OnWenDuChange(float value)
        {
            sTemperatureSlider.value = value / 180;
        }

        private void OnZaoShengChange(float value)
        {
            sNoiseSlider.value = value / 150;
        }

        private void OnZhenDongPinLvChange(int x, int y, int z)
        {
            _view.tmptxt_Speed.text = x.ToString();
            _view.tmptxt_Mileage.text = y.ToString();

            //_view.tmptxt_Electricity.text = z.ToString();
        }

        /// <summary>
        /// 设置电压
        /// </summary>
        /// <param name="value"></param>
        private void OnVoltageChange(float value)
        {
            _view.tmptxt_Electricity.text = value.ToString("f0");
        }

        private void OnUpdateNeedleRotationAngle(float angle)
        {
            // 设置指针的旋转
            _view.imgPointer.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// 切换屏幕布局
        /// </summary>
        void ChangeScreen(int id, bool isOn, bool isHand)
        {
            optionIndex = -1;
            pageIndex = 2;
            currentMainType = id;
            if (isOn)
            {
                _currentView.SetActive(false);

                switch (id)
                {
                    case 0:
                        _view.tsEngineView.gameObject.SetActive(true);
                        _currentView = _view.tsEngineView.gameObject;
                        break;
                    case 1:
                        _view.tsChassisView.gameObject.SetActive(true);
                        _currentView = _view.tsChassisView.gameObject;
                        break;
                    case 2:
                        _view.tsElectricView.gameObject.SetActive(true);
                        _currentView = _view.tsElectricView.gameObject;
                        break;
                }

                DataCheckTopicManager.Instance.HideTips(id);

                _view.btnClose.onClick.RemoveAllListeners();
                _view.btnClose.onClick.AddListener(() => { OnBackToLast(true); });
                //SplicingScreensControl.Instance.ChangeLayout(id + 1);
                ChangeCameraLook(id);

                uiAreaType = 1;
                optionIndex = id;
                HandleMsg.Instance.UILevelBuilder(uiLevel, 3, id);
                HandleMsg.Instance.DebugUILevel(uiLevel, "MainUILevel");

                if (isHand)
                {
                    //Pad端由于有两个单列表，所以第一个单列表的索引必须是-1，否则会直接选中第一个列表的任意一个元素跳转到第二个列表内
                    SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, -1);
                }
            }
        }

        void OnCloseToMain(bool isHand)
        {
            _uGuiForm.Close();

            //不是从任务发布退出界面
            if (userDataCache == null)
            {
                GameEntry.UI.OpenUIFormSync<UIMainMenu>();
                HandleMsg.Instance.UILevelBuilder(uiLevel, 1);
                HandleMsg.Instance.DebugUILevel(uiLevel, "Fake2MainUILevel");

                if (isHand)
                {
                    SendMsgManager.SendUINavigationMsg(uiLevel, 0, -1);
                }
            }
        }

        void OnBackToLast(bool isHand)
        {
            pageIndex = 1;
            _view.tsAllView.gameObject.SetActive(true);
            _currentView.SetActive(false);
            _currentView = _view.tsAllView.gameObject;
            //大项的页签选中状态重置
            for (int i = 0; i < _view.tsToggleList.childCount; i++)
            {
                _view.tsToggleList.GetChild(i).GetComponent<Toggle>().isOn = false;
            }

            DataCheckTopicManager.Instance.HideTips(-1);
            InitDataModelShow();
            _view.btnClose.onClick.RemoveAllListeners();
            _view.btnClose.onClick.AddListener(() => { OnCloseToMain(true); });

            //Main
            optionIndex = -1;
            currentMainType = -1;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 2);
            HandleMsg.Instance.DebugUILevel(uiLevel, "FakeMainTabUILevel");
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }

        /// <summary>
        /// 移除0号位数据
        /// </summary>
        /// <param name="chart"></param>
        void RemoveIndexData(BaseChart chart, string serieName, float value)
        {
            chart.RemoveSerie(0);

            chart.AddData(serieName, value);
        }

        /// <summary>
        /// 切换相机look点，隐藏模型
        /// </summary>
        /// <param name="type">大类按钮索引</param>
        void ChangeCameraLook(int type)
        {
            Vector3 cameraPos, cameraRot;
            Transform cameraLookAt = null;

            var dataList = _model.GetDataCheckConfigs(type + 1);

            Transform parent = GameManager.Instance.MainTarget;

            List<Transform> trans = new List<Transform>();
            //把必要模型显示出来
            for (int i = 0; i < dataList.Count; i++)
            {
                for (int j = 0; j < dataList[i].ModelPaths.Length; j++)
                {
                    var temp = GameManager.Instance.MainTarget.Find(dataList[i].ModelPaths[j]);
                    if (temp == null)
                    {
                        Debug.LogErrorFormat("未查询到路径模型：{0}", dataList[i].ModelPaths[j]);
                        continue;
                    }

                    temp.gameObject.SetActive(true);
                    trans.Add(temp);
                }
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                if (!trans.Contains(parent.GetChild(i)))
                {
                    parent.GetChild(i).gameObject.SetActive(false);
                }
            }

            var config = MainStructureConfigTable.Instance.Get(type);
            cameraLookAt = GameManager.Instance.MainTarget.Find(config.ModelPath);
            cameraPos = new Vector3(config.TargetPos[0], config.TargetPos[1], config.TargetPos[2]);
            cameraRot = new Vector3(config.TargetRot[0], config.TargetRot[1], config.TargetRot[2]);

            cameraLookAt.gameObject.SetActive(true);

            GameManager.Instance.ViewMainCamera.Target.TrackingTarget = cameraLookAt;

            GameManager.Instance.ViewMainCamera.ForceCameraPosition(cameraPos, Quaternion.Euler(cameraRot));
        }

        /// <summary>
        /// 返回的时候初始化模型显示
        /// </summary>
        void InitDataModelShow()
        {
            Transform parent = GameManager.Instance.MainTarget;
            //for (int i = 0; i < parent.childCount; i++)
            //{
            //    parent.GetChild(i).gameObject.SetActive(true);
            //}

            //foreach (var item in _WaiKe)
            //{
            //    item.gameObject.SetActive(false);
            //}
        }

        /// <summary>
        /// 设置提示点内容
        /// </summary>
        void SetTipItem(DataCheckConfig config, bool isHand)
        {
            optionIndex = -1;
            GameObject uiParentModel = null;

            switch (config.Type)
            {
                case 1:
                    uiParentModel = _view.tsEngineView.gameObject;
                    break;
                case 2:
                    uiParentModel = _view.tsChassisView.gameObject;
                    break;
                case 3:
                    uiParentModel = _view.tsElectricView.gameObject;
                    break;
            }

            if (uiParentModel != _currentView)
            {
                return;
            }

            Transform uiModel = _view.imgBg.transform.Find(config.UIModelName);
            currentConfig = config;
            CommonProcess(config, uiModel, isHand);

            uiAreaType = 1;
            var dataList = _model.GetDataCheckConfigs(config.Type);
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i].Id == config.Id)
                {
                    optionIndex = i;
                    break;
                }
            }

            HandleMsg.Instance.UILevelBuilder(uiLevel, 4, optionIndex);
            HandleMsg.Instance.DebugUILevel(uiLevel, "SubUILevel");

            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, -1);
            }
        }

        /// <summary>
        /// tip提示点通用点处理
        /// </summary>
        /// <param name="config">配置数据</param>
        /// <param name="uiModel">UIName</param>
        void CommonProcess(DataCheckConfig config, Transform uiModel, bool isHand)
        {
            if (_currentView != uiModel.gameObject)
            {
                pageIndex = 3;
                _currentView.SetActive(false);
                uiModel.gameObject.SetActive(true);

                _view.btnClose.onClick.AddListener(() => { OnBackTo(config, isHand); });
            }

            uiModel.GetComponent<UIDataScreenCommon>().InitValue(config);

            _currentView = uiModel.gameObject;
        }

        /// <summary>
        /// 当某个采集按钮被点击跟Pad端交互
        /// </summary>
        /// <param name="index"></param>
        private void OnAnyCheckItemChecked(int index)
        {
            HandleMsg.Instance.UILevelBuilder(uiLevel, 4);
            SendMsgManager.SendUINavigationMsg(uiLevel, 1, index);
        }

        private void OnBackTo(DataCheckConfig config, bool isHand)
        {
            ChangeScreen(config.Type - 1, true, isHand);

            //Sub
            optionIndex = -1;
            HandleMsg.Instance.UILevelBuilder(uiLevel, 4);
            HandleMsg.Instance.DebugUILevel(uiLevel, "FakeSubTabUILevel");
            if (isHand)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex);
            }
        }
    }


    /// <summary>
    /// 打开类型
    /// </summary>
    public enum UIDataSceneOpenType
    {
        //单纯使用
        Pure,

        //辅助显示
        Help
    }
}