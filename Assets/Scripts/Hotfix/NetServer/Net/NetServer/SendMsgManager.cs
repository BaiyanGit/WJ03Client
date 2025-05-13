using System.Collections.Generic;
using Hotfix;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;

namespace UI.NetworkUI
{
    using UnityEngine;

    public enum CommonBtnMsgType
    {
        [InspectorName("退出")] ExitGame = -1,
        [InspectorName("登录")] Login = 0,
        [InspectorName("首页")] Home = 1,
        [InspectorName("信号切换")] SwitchSignal = 2,
        [InspectorName("考核")] Assess = 3,
        [InspectorName("提交")] SubmitScore = 4,
        [InspectorName("返回上页")] BackLastPage = 5,
        [InspectorName("退出应用")] ExitApp = 6,
        [InspectorName("重置")] Reset = 7,
        [InspectorName("显示")] Show = 8,
        [InspectorName("隐藏")] Hide = 9,
        [InspectorName("左移")] MoveLeft = 10,
        [InspectorName("右移")] MoveRight = 11,
        [InspectorName("上移")] MoveUp = 12,
        [InspectorName("下移")] MoveDown = 13,
        [InspectorName("左转")] RotateLeft = 14,
        [InspectorName("右转")] RotateRight = 15,
        [InspectorName("上转")] RotateUp = 16,
        [InspectorName("下转")] RotateDown = 17,
        [InspectorName("放大")] ZoomIn = 18,
        [InspectorName("缩小")] ZoomOut = 19,
        [InspectorName("退出考核")] ExitAssess = 20,
        [InspectorName("提交用户信息按钮")] ConfirmInputUserInfo = 21,
    }

    public static class SendMsgManager
    {
        /// <summary>
        /// 发送心跳包
        /// </summary>
        public static void SendHeartbeatMsg()
        {
            var msg = new MsgHeartbeatData();
            //NetManager.Send(msg);
            ServNet.Instance.Broadcast(msg);
        }

        /// <summary>
        /// 发送通用单个按钮操作数据
        /// </summary>
        /// <param name="btnType">按钮操作类型，-1:退出 0:登录 1:首页 2:信号切换 4:提交 5:返回上页 6:退出应用 7:重置 8:显示 9:隐藏 20:退出考核 21:提交用户信息按钮</param>
        public static void SendCommonBtnMsg(CommonBtnMsgType btnType)
        {
            var msg = new MsgCommonBtnOperationData
            {
                btnType = (int)btnType
            };
            // NetManager.Send(msg);
            ServNet.Instance.Broadcast(msg);

            //特殊处理输入缓存数据的清理
            if (btnType == CommonBtnMsgType.ConfirmInputUserInfo)
            {
                ClearInputInfo();
            }

            Debug.LogFormat("SendCommonBtnMsg: btnType={0}", btnType);
        }

        /// <summary>
        /// 发送UI导航消息
        /// </summary>
        /// <param name="uiLevel">UI层级</param>
        /// <param name="conn">当前列表选中项的索引</param>
        /// <param name="uiAreaType">当前列表选中项的索引</param>
        /// <param name="optionIndex">定向连接</param>
        public static void SendUINavigationMsg(List<int> uiLevel, int uiAreaType, int optionIndex, Conn conn = null)
        {
            var msg = new MsgUINavigationData
            {
                uiLevel = uiLevel,
                uiAreaType = uiAreaType,
                optionIndex = optionIndex
            };
            // NetManager.Send(msg);

            if (conn != null)
            {
                ServNet.Instance.Send(conn, msg);
            }
            else
            {
                ServNet.Instance.Broadcast(msg);
            }

            HandleMsg.Instance.DebugUILevel(msg.uiLevel, "发送跳转UI信息： uiAreaType = " + msg.uiAreaType + "  optionIndex = " + msg.optionIndex + " ");
        }

        /// <summary>
        /// 缓存用户名输入
        /// </summary>
        private static string userNameCache;

        /// <summary>
        /// 缓存用户编号输入
        /// </summary>
        private static string userNumCache;

        /// <summary>
        /// 缓存教员评语
        /// </summary>
        private static string userEvaluationCache;

        /// <summary>
        /// 缓存教员评分
        /// </summary>
        private static string userScoreCache;

        /// <summary>
        /// 发送UI输入用户信息
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userNum">用户编号</param>
        public static void SendUIInputUserInfoMsg(string userName, string userNum, string userEvaluation, string userScore)
        {
            userNameCache = userName;
            userNumCache = userNum;
            userEvaluationCache = userEvaluation;
            userScoreCache = userScore;

            var msg = new MsgInputUserInfoData
            {
                userName = userName,
                userNum = userNum,
                userEvaluation = userEvaluation,
                userScore = userScore
            };
            // NetManager.Send(msg);
            ServNet.Instance.Broadcast(msg);

            //Debug.LogFormat("SendUIInputUserInfoMsg: userName={0}.userNum={1}.userEvaluation={2}.userScore={3}", userName, userNum, userEvaluation, userScore);
        }

        /// <summary>
        /// 同步缓存的输入信息
        /// </summary>
        public static void SendCachedUIInputUserInfoMsg(Conn coon)
        {
            var msg = new MsgInputUserInfoData
            {
                userName = userNameCache,
                userNum = userNumCache,
                userEvaluation = userEvaluationCache,
                userScore = userScoreCache
            };

            ServNet.Instance.Send(coon, msg);
        }

        /// <summary>
        /// 清除输入缓存
        /// </summary>
        public static void ClearInputInfo()
        {
            userNameCache = "";
            userNumCache = "";
            userEvaluationCache = "";
            userScoreCache = "";
        }

        /// <summary>
        /// 发送模型操作数据
        /// </summary>
        /// <param name="btnType">按钮操作类型，10:左移 11:右移 12:上移 13:下移 14:左转 15:右转 16:上转 17:下转 18:放大 19:缩小</param>
        /// <param name="isPressed">按下true，松开false</param>
        public static void SendOperationData(CommonBtnMsgType btnType, bool isPressed)
        {
            var msg = new MsgOperationData
            {
                btnType = (int)btnType,
                state = isPressed
            };
            //NetManager.Send(msg);
            ServNet.Instance.Broadcast(msg);
        }

        /// <summary>
        /// 发送结算信息
        /// </summary>
        /// <param name="useTime"></param>
        /// <param name="score"></param>
        /// <param name="examInfos"></param>
        /// <param name="conn">定向连接</param>
        public static void SendSettlementInfo(string useTime, string score, List<ResponseRecordExamInfo> examInfos, Conn conn = null)
        {
            var msg = new MsgUISettlementInfoData();
            msg.totalTime = useTime;
            msg.totalScore = score;
            msg.detailList = new();

            foreach (var examInfo in examInfos)
            {
                var record = new MsgUISettlementDetailData();
                record.subScoreType = examInfo.PointID.ToString();
                record.subScoreValue = examInfo.Score.ToString();
                record.subScoreReason = examInfo.CreateTime;
                msg.detailList.Add(record);
            }

            if (conn == null)
            {
                ServNet.Instance.Broadcast(msg);
            }
            else
            {
                ServNet.Instance.Send(conn, msg);
            }
        }
    }
}