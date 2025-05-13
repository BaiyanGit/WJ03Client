using System;
using System.Collections.Generic;
using System.Reflection;
using Hotfix.ExcelData;
using UI.NetworkUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hotfix.NetServer.Net.NetServer
{
    /// <summary>
    /// 处理Pad端传来的消息
    /// </summary>
    public class HandleMsg : MonoBehaviour
    {
        public static HandleMsg Instance;

        /// <summary>
        /// 是否允许平板控制
        /// </summary>
        [FormerlySerializedAs("IsAllowPadControl")]
        public bool isAllowPadControl = true;

        public float Ratio = 0.2f;

        private void Awake()
        {
            Instance = this;

            Ctrl_MessageCenter.AddMsgListener<string, object>("UIFormOpen", OnUIFormOpen);
            Ctrl_MessageCenter.AddMsgListener<string, object>("UIFormClose", OnUIFormClose);
            Ctrl_MessageCenter.AddMsgListener<string, object>("UIFormOpenAsync", OnUIFormOpenAsync);
        }

        private void Start()
        {
            ServNet.Instance.OnSomeOneConnect += OnSomeOneConnect;
            ServNet.Instance.AddMsgListener("MsgCommonBtnOperationData", OnMsgCommonBtnOperationData);
            ServNet.Instance.AddMsgListener("MsgInputUserInfoData", OnMsgInputUserInfoData);
            ServNet.Instance.AddMsgListener("MsgOperationData", OnModelOperationData);
            ServNet.Instance.AddMsgListener("MsgUINavigationData", OnMsgUINavigationData);
            ServNet.Instance.AddMsgListener("MsgHeartbeatData", OnMsgHeartbeatData);
            ServNet.Instance.AddMsgListener("MsgRequestUINavigationData", OnMsgRequestUINavigationData);
            ServNet.Instance.AddMsgListener("MsgMonitorSwitchData", OnMsgMonitorSwitchData);
        }

        /// <summary>
        /// 通用单个按钮操作数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msgBase"></param>
        private void OnMsgCommonBtnOperationData(Conn conn, MsgBase msgBase)
        {
            var msg = (MsgCommonBtnOperationData)msgBase;
            var type = (CommonBtnMsgType)msg.btnType;

            Debug.LogFormat("OnMsgCommonBtnOperationData: {0}", type);
            switch (type)
            {
                //0:登录
                case CommonBtnMsgType.Login:
                    Ctrl_MessageCenter.SendMessage("PadLogin", msgBase);
                    break;
                //1:首页 
                case CommonBtnMsgType.Home:
                    Ctrl_MessageCenter.SendMessage("Pad2Main", msgBase);
                    break;
                //2:信号切换 
                case CommonBtnMsgType.SwitchSignal:
                    OnBigScreenLayoutChange(msgBase);
                    break;
                //4:提交 
                case CommonBtnMsgType.SubmitScore:
                    Ctrl_MessageCenter.SendMessage("PadSubmitScore", msgBase);
                    break;
                //5:返回上页 
                case CommonBtnMsgType.BackLastPage:
                    Ctrl_MessageCenter.SendMessage("PadChange2LastPage", msgBase);
                    break;
                //-1:退出登录
                //6:退出应用
                case CommonBtnMsgType.ExitGame:
                case CommonBtnMsgType.ExitApp:
                    //Ctrl_MessageCenter.SendMessage("PadExitApp", msgBase);
                    //直接过掉消息，不阻塞消息流程
                    ServNet.Instance.SetCacheMsgBaseState(msgBase);
                    break;
                //7：视角重置
                case CommonBtnMsgType.Reset:
                    OnHnadleViewReset(msg);
                    break;
                //8：模型显示
                case CommonBtnMsgType.Show:
                    Ctrl_MessageCenter.SendMessage("PadShowOrHideModel", false, msgBase);
                    break;
                //9：模型隐藏
                case CommonBtnMsgType.Hide:
                    Ctrl_MessageCenter.SendMessage("PadShowOrHideModel", true, msgBase);
                    break;
                //20:退出考核（结算界面关闭）
                case CommonBtnMsgType.ExitAssess:
                    Ctrl_MessageCenter.SendMessage("PadExitAssess", msgBase);
                    Instance.ForceSyncAfterSettlementUI();
                    break;
                //21:确认输入用户信息
                case CommonBtnMsgType.ConfirmInputUserInfo:
                    Ctrl_MessageCenter.SendMessage("PadConfirmInputUserInfo", msgBase);
                    //清除缓存的消息
                    SendMsgManager.ClearInputInfo();
                    break;
            }
        }

        private void OnMsgInputUserInfoData(Conn conn, MsgBase msgBase)
        {
            Ctrl_MessageCenter.SendMessage("PadInputUserInfo", msgBase);
        }

        private void OnModelOperationData(Conn conn, MsgBase msgBase)
        {
            var ratio = Ratio * 1f;
            var ratio2 = ratio * 0.1f;
            var ratio3 = ratio2 * 0.2f;
            var msg = (MsgOperationData)msgBase;
            var type = (CommonBtnMsgType)msg.btnType;
            switch (type)
            {
                //左移 
                case CommonBtnMsgType.MoveLeft:
                    _lastPanX = !msg.state ? 0f : ratio2;
                    break;
                //右移 
                case CommonBtnMsgType.MoveRight:
                    _lastPanX = !msg.state ? 0f : -ratio2;
                    break;
                //上移 
                case CommonBtnMsgType.MoveUp:
                    _lastPanY = !msg.state ? 0f : ratio2;
                    break;
                //下移
                case CommonBtnMsgType.MoveDown:
                    _lastPanY = !msg.state ? 0f : -ratio2;
                    break;
                //左转 
                case CommonBtnMsgType.RotateLeft:
                    _lastRotationX = !msg.state ? 0f : ratio;
                    break;
                //右转 
                case CommonBtnMsgType.RotateRight:
                    _lastRotationX = !msg.state ? 0f : -ratio;
                    break;
                //上转
                case CommonBtnMsgType.RotateUp:
                    _lastRotationY = !msg.state ? 0f : ratio;
                    break;
                //下转 
                case CommonBtnMsgType.RotateDown:
                    _lastRotationY = !msg.state ? 0f : -ratio;
                    break;
                //放大
                case CommonBtnMsgType.ZoomIn:
                    _lastZoom = !msg.state ? 0f : ratio3;
                    break;
                //缩小
                case CommonBtnMsgType.ZoomOut:
                    _lastZoom = !msg.state ? 0f : -ratio3;
                    break;
                //考核状态切换
                case CommonBtnMsgType.Assess:
                    Ctrl_MessageCenter.SendMessage("PadChangeTrainMode", msg);
                    break;
            }

            //视角相关的状态才更新相机控制
            if (type != CommonBtnMsgType.Assess)
            {
                CameraControlAdapter(msg);
            }
        }

        private void OnMsgUINavigationData(Conn conn, MsgBase msgBase)
        {
            // if (!IsAllowPadControl)
            // {
            //     return;
            // }

            MsgUINavigationData msg = (MsgUINavigationData)msgBase;

            DebugUILevel(msg.uiLevel, "接收到跳转UI信息： uiAreaType = " + msg.uiAreaType + "  optionIndex = " + msg.optionIndex + " ");

            UIFormControl(msg);
        }

        /// <summary>
        /// 心跳包
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msgBase"></param>
        private void OnMsgHeartbeatData(Conn conn, MsgBase msgBase)
        {
            ServNet.Instance.Broadcast(msgBase);
            ServNet.Instance.SetCacheMsgBaseState(msgBase);
        }

        private void OnMsgRequestUINavigationData(Conn conn, MsgBase msgBase)
        {
            MsgRequestUINavigationData msg = (MsgRequestUINavigationData)msgBase;
            Debug.Log("收到Pad端-主动请求UI导航消息");
            ServNet.Instance.SetCacheMsgBaseState(msgBase);
        }

        private void OnMsgMonitorSwitchData(Conn conn, MsgBase msgBase)
        {
            MsgMonitorSwitchData msg = (MsgMonitorSwitchData)msgBase;
            Debug.LogFormat("当前大屏布局索引：{0}", msg.monitorIndex);

            SplicingScreensControl.Instance.ChangeLayout(msg.monitorIndex);
            ServNet.Instance.SetCacheMsgBaseState(msgBase);
        }

        void Update()
        {
            CameraControl();
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                //主界面
                //结构认知
                //OnUIControlFakeTest(-1);
                OnTestSomeUIForm(3);
                //原理学习
                //OnUIControlFakeTest(-1, 1, 1, "测试");
                //OnTestSomeUIForm(4);
                //状态监测
                //OnUIControlFakeTest(-1, 1, 2, "测试");
                //OnTestSomeUIForm(9);
                //虚拟实训
                //OnUIControlFakeTest(-1, 1, 3, "测试");
                //OnTestSomeUIForm(10);
                //实训（UITopicList）{发动机-底盘-电气-任务发布}
                //OnTestSomeUIForm(12);
                //公用虚拟实训训练（UITrain）
                //OnTestSomeUIForm(17);
                //提交成绩
                //OnTestSomeUIForm(18);
                //结算
                //OnTestSomeUIForm(19);
            }

            // if (Input.GetKeyDown(KeyCode.KeypadPlus))
            // {
            //     OnRequestCurrentUIControlInfos();
            // }

            // if (Input.GetKeyDown(KeyCode.KeypadMinus))
            // {
            //     Ctrl_MessageCenter.SendMessage("PadChange2LastPage", new MsgBase());
            // }
        }

        #region 控制界面

        private void UIFormControl(MsgBase msg)
        {
            var temp = (MsgUINavigationData)msg;

            if (temp.uiAreaType == 0)
            {
                //给公用界面切页签
                Ctrl_MessageCenter.SendMessage("PadUICommonPageTab", temp);
            }
            else
            {
                //给专属界面切按钮
                var currentPageTag = PadTag2CurrentPageTag(temp.uiLevel);

                if (!GameEntry.UI.HasUIForm(currentPageTag))
                {
                    Debug.LogWarningFormat("界面还未打开：{0}", currentPageTag);
                }
                else
                {
                    //给当值界面切按钮
                    Ctrl_MessageCenter.SendMessage(currentPageTag, temp);
                }
            }
        }

        /// <summary>
        /// 与Pad端界面ID适配器
        /// </summary>
        /// <param name="sourceTag">源标签，Pad端传过来的标签</param>
        /// <returns></returns>
        private string PadTag2CurrentPageTag(List<int> sourceTag)
        {
            string temp = "";
            List<string> list = new();
            //收集所有可能的转换标签
            for (int i = 0; i < sourceTag.Count; i++)
            {
                list.Add(temp += sourceTag[i]);
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                //查找唯一的界面
                foreach (var item in ModuleConfigTable.Instance.dataList)
                {
                    if (item.AdapterParam.Equals(list[i]))
                    {
                        return item.ClassName;
                    }
                }
            }

            Debug.LogErrorFormat("PadTag2CurrentPageTag: 没有找到适配的界面，sourceTag = {0}", sourceTag);

            return string.Empty;
        }

        #endregion

        #region 摄像机操作

        private float _lastRotationX;
        private float _lastRotationY;
        private float _lastZoom;
        private float _lastPanX;
        private float _lastPanY;

        private bool _isRotation;
        private bool _isZoom;
        private bool _isPan;

        /// <summary>
        /// 相机控制适配器
        /// </summary>
        private void CameraControlAdapter(MsgBase msgBase)
        {
            _isRotation = _lastRotationX != 0f || _lastRotationY != 0f;
            _isZoom = _lastZoom != 0f;
            _isPan = _lastPanX != 0f || _lastPanY != 0f;

            ServNet.Instance.SetCacheMsgBaseState(msgBase);
        }

        /// <summary>
        /// 摄像机控制
        /// </summary>
        private void CameraControl()
        {
            if (_isRotation)
            {
                GameManager.Instance.CinemachineCameraController.HandleRotation(_lastRotationX, _lastRotationY);
            }

            if (_isZoom)
            {
                GameManager.Instance.CinemachineCameraController.HandleZoom(_lastZoom);
            }

            if (_isPan)
            {
                GameManager.Instance.CinemachineCameraController.HandlePan(_lastPanX, _lastPanY);
            }
        }

        #endregion

        #region 界面操作痕迹缓存

        private List<string> _currentUIForms = new List<string>();

        private void OnUIFormOpen(string uiFormName, object userData)
        {
            // Debug.Log("OnUIFormOpen: " + uiFormName);
            CacheCurrentUIForm(uiFormName, userData);
        }

        private void OnUIFormClose(string uiFormName, object userData)
        {
            // Debug.Log("OnUIFormClose: " + uiFormName);
            ClearACache(uiFormName);
        }

        private void OnUIFormOpenAsync(string uiFormName, object userData)
        {
            // Debug.Log("OnUIFormOpenAsync: " + uiFormName);
            CacheCurrentUIForm(uiFormName, userData);
        }

        private void CacheCurrentUIForm(string uiFormName, object userData)
        {
            switch (uiFormName)
            {
                //特殊处理状态监控界面和公用界面
                case "UICommonPage":
                case "UILoading":
                    return;
                case "UIDataScreen" when userData != null:
                {
                    if (!_currentUIForms.Contains(uiFormName))
                    {
                        _currentUIForms.Add(uiFormName);
                    }

                    break;
                }
                default:
                    _currentUIForms.Add(uiFormName);
                    break;
            }
        }

        private void ClearACache(string uiFormName)
        {
            if (_currentUIForms.Contains(uiFormName))
            {
                _currentUIForms.Remove(uiFormName);
            }
        }

        /// <summary>
        /// 收集当前界面的操作信息
        /// </summary>
        private void OnRequestCurrentUIControlInfos(Conn conn)
        {
            if (_currentUIForms.Count == 0)
            {
                return;
            }

            List<int> uiLevel = new();
            int uiAreaType = -100, optionIndex = -100, optionIndexExtend = -100;

            foreach (var variable in _currentUIForms)
            {
                var iUIForm = GameEntry.UI.GetUIForm(variable);

                if (iUIForm != null)
                {
                    //赋值当前的状态数据
                    Type type = iUIForm.GetType();
                    //访问私有字段
                    var privateField1 = type.GetField("uiLevel", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (privateField1 != null) uiLevel = (List<int>)privateField1.GetValue(iUIForm);
                    var privateField2 = type.GetField("uiAreaType", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (privateField2 != null) uiAreaType = (int)privateField2.GetValue(iUIForm);
                    var privateField3 = type.GetField("optionIndex", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (privateField3 != null) optionIndex = (int)privateField3.GetValue(iUIForm);
                    var privateField4 = type.GetField("optionIndexExtend", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (privateField4 != null) optionIndexExtend = (int)privateField4.GetValue(iUIForm);
                }
            }

            //当界面是多列表状态时，主动推送上一个页签的信息
            if (optionIndexExtend >= 0)
            {
                SendMsgManager.SendUINavigationMsg(uiLevel, 1, optionIndexExtend);
            }

            SendMsgManager.SendUINavigationMsg(uiLevel, uiAreaType, optionIndex, conn);
            //发送考核状态
            SendMsgManager.SendOperationData(CommonBtnMsgType.Assess, GameManager.Instance.AssessmentMode == EnumAssessmentMode.Self);
        }

        #endregion

        #region 界面-特殊操作

        /// <summary>
        /// 当检查点变化
        /// </summary>
        private void OnCheckPointChange()
        {
            //监听事件，转化单片机线序为index
            //Ctrl_MessageCenter.AddMsgListener<bool, int>("Comparison", ComparisonSingle);
        }

        /// <summary>
        /// 当传感器数值变化？
        /// </summary>
        private void OnSensorValueChange()
        {
            //监听事件
            //DataCheckTopicManager.Instance.RefreshData;
        }

        /// <summary>
        /// 当传感器交互
        /// </summary>
        private void OnSensorCheck()
        {
        }

        #endregion

        #region 大屏信号控制

        private int _currentScreenIndex = 1;

        private void OnBigScreenLayoutChange(MsgBase msgBase)
        {
            _currentScreenIndex++;

            if (_currentScreenIndex > AppConst.Constant.BigScreenLayoutIndexMax)
            {
                _currentScreenIndex = 1;
            }

            Debug.LogFormat("当前大屏布局索引：{0}", _currentScreenIndex);

            SplicingScreensControl.Instance.ChangeLayout(_currentScreenIndex);
            ServNet.Instance.SetCacheMsgBaseState(msgBase);
        }

        #endregion

        #region FakeTest

        private void OnTestSomeUIForm(int parentID)
        {
            //主分类
            OnUIControlFakeTest(new List<int>() { -1, 0, 1 });
            //子分类
            //OnUIControlFakeTest(parentID, 2, 0, "测试");
            //子子分类（原理学习）
            //OnUIControlFakeTest(parentID, 3, 1, "测试");

            //TODO: 页签切换时要不要修改操作痕迹的长度？
            //页签
            //第一页签（首页）
            //OnUIControlFakeTest(parentID, 0, 0, "测试");
            //第二页签
            //OnUIControlFakeTest(parentID, 0, 1, "测试");
            //第三页签（无反应）
            //OnUIControlFakeTest(parentID, 0, 2, "测试");
        }

        private void OnUIControlFakeTest(List<int> uiLevel)
        {
            MsgUINavigationData msg = new()
            {
                uiLevel = uiLevel
            };

            UIFormControl(msg);
        }

        #endregion

        /// <summary>
        /// 当有端接入
        /// </summary>
        /// <param name="conn"></param>
        private void OnSomeOneConnect(Conn conn)
        {
            var temp = false;
            //直到初始化完成过
            while (temp == false)
            {
                if (GameManager.Instance.IsLoginNormal)
                {
                    //发送当前界面的信息
                    OnRequestCurrentUIControlInfos(conn);

                    //如果当前界面是学员信息提交界面，则同步输入信息
                    if (_currentUIForms.Contains("UIStudentInfoRecord"))
                    {
                        SendMsgManager.SendCachedUIInputUserInfoMsg(conn);
                    }
                    //如果当前界面是结算界面，则同步成绩信息
                    else if (_currentUIForms.Contains("UISettlement"))
                    {
                        SendMsgManager.SendSettlementInfo(TopicManager.Instance.UseTime, TopicManager.Instance.Score.ToString(), SettlementManager.Instance.ErrorList, conn);
                    }

                    temp = true;
                }
            }
        }

        /// <summary>
        /// 构造UILevel结构
        /// </summary>
        /// <param name="uiLevel"></param>
        /// <param name="desireLength"></param>
        /// <param name="desireIndex"></param>
        public void UILevelBuilder(List<int> uiLevel, int desireLength, int desireIndex = -1)
        {
            if (uiLevel.Count > desireLength)
            {
                uiLevel.RemoveRange(desireLength, uiLevel.Count - desireLength);
            }

            if (uiLevel.Count == desireLength && desireIndex != -1)
            {
                uiLevel[desireLength - 1] = desireIndex;
            }

            if (uiLevel.Count < desireLength && desireIndex != -1)
            {
                uiLevel.Add(desireIndex);
            }
        }

        /// <summary>
        /// uiLevel的Debug输出
        /// </summary>
        /// <param name="uiLevel"></param>
        /// <param name="tag"></param>
        public void DebugUILevel(List<int> uiLevel, string tag)
        {
            string temp = String.Empty;
            for (int i = 0; i < uiLevel.Count; i++)
            {
                temp += (i == 0 ? "" : ".") + uiLevel[i];
            }

            Debug.LogFormat("{0}.UILevel: {1}", tag, temp);
        }

        /// <summary>
        /// 强制同步结算界面后续
        /// </summary>
        public void ForceSyncAfterSettlementUI()
        {
            int temp = (int)GameManager.Instance.TrainType - 1;
            temp = Mathf.Clamp(temp, 0, 3);
            SendMsgManager.SendUINavigationMsg(new List<int>() { -1, 3, (int)GameManager.Instance.TrainType - 1 }, -100, -100);
        }

        /// <summary>
        /// pad端适配器参数转uiLevel
        /// </summary>
        /// <param name="uiLevel"></param>
        /// <param name="adapterParam"></param>
        public void PadAdpaterParmsToUILevel(ref List<int> uiLevel, string adapterParam)
        {
            if (uiLevel == null)
            {
                uiLevel = new();
            }
            else
            {
                uiLevel.Clear();
            }

            for (int i = 1; i < adapterParam.Length; i++)
            {
                uiLevel.Add(int.Parse(adapterParam[i].ToString()));
            }

            uiLevel[0] *= -1;
        }

        /// <summary>
        /// 当视角重置
        /// </summary>
        private void OnHnadleViewReset(MsgBase msgBase)
        {
            if (_currentUIForms.Contains("UIStructureCognition"))
            {
                GameManager.Instance.RestCinemachineCameraPrincip("结构认知");
            }

            if (_currentUIForms.Contains("UIPrincipleLearning"))
            {
                GameManager.Instance.RestCinemachineCameraPrincip("原理仿真");
            }

            ServNet.Instance.SetCacheMsgBaseState(msgBase);
        }
    }
}