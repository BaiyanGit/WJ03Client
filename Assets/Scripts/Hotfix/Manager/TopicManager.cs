using System;
using System.Collections.Generic;
using Hotfix.ExcelData;
using Hotfix.NetServer.Net.NetServer;
using Hotfix.UI;
using UI.NetworkUI;
using UnityEngine;
using Wx.Runtime.Singleton;

namespace Hotfix
{
    public class TopicManager : SingletonInstance<TopicManager>, ISingleton
    {
        private List<TopicResoultInfo> topicInfos = new();

        private float userTime;

        public string BeginTime;
        public string EndTime;
        public float score = 100;

        public int EvaluationScore;

        public float Score
        {
            get
            {
                int tmpEvaluationScore = Mathf.Clamp(EvaluationScore, 0, 100);
                float tmpScore = (score * tmpEvaluationScore) == 0 ? 0 : ((score * .3f) + (tmpEvaluationScore * .7f));
                tmpScore = Mathf.Clamp(tmpScore, 0, 100);
                return tmpScore;
            }
            set { score = value; }
        }

        public int TrainType = 1;
        public List<TopicResoultInfo> resoultInfos;
        public TopicResoultInfo currentTopicInfo;


        public string UseTime
        {
            get { return ConvertSecondsToTime(userTime); }
        }

        private bool _topicStart;

        /// <summary>
        /// 所有的检测点
        /// </summary>
        Dictionary<int, CheckPointState> checkPointDic = new();

        /// <summary>
        /// 当前检测项的检测点
        /// </summary>
        Dictionary<int, FaultCheckConfig4th> currentCheckPointDic = new();

        /// <summary>
        /// 当前检测点
        /// </summary>
        int currentCheckPoint = 0;

        int currentKey;

        public void OnCreate(object createParam)
        {
            Ctrl_MessageCenter.AddMsgListener<int>("ChangeSinglechipValue", ComparisonKey);
            Ctrl_MessageCenter.AddMsgListener<UIDataSceneOpenType>("OnUIDataScreenOpen", OnUIDataScreenOpen);
            Ctrl_MessageCenter.AddMsgListener<UIDataSceneOpenType>("OnUIDataScreenClose", OnUIDataScreenClose);
        }

        /// <summary>
        /// 比对数据
        /// </summary>
        /// <param name="currentKey"></param>
        public void ComparisonKey(int currentKey)
        {
            WLog.Log("ChangeSinglechipValue" + currentKey);

            if (currentCheckPointDic.ContainsKey(currentCheckPoint) && currentCheckPointDic[currentCheckPoint].SinglechipPort != currentKey)
            {
                WLog.Log("出现错误了" + currentCheckPoint);
                ResponseRecordExamInfo recordExamInfo = new ResponseRecordExamInfo();
                recordExamInfo.PointID = currentCheckPointDic[currentCheckPoint].Id;
                recordExamInfo.CreateTime = DateTime.Now.ToString();
                recordExamInfo.Score = -(100 / checkPointDic.Count);

                Score += recordExamInfo.Score;
                //TODO:处理错误结果
                SettlementManager.Instance.ErrorList.Add(recordExamInfo);
                //SettlementManager.Instance.ErrorList.Add(currentCheckPointDic[currentCheckPoint]);
                return;
            }

            //if (!currentCheckPointDic.ContainsKey(currentCheckPoint) /*|| this.currentKey == currentKey */|| currentCheckPointDic[currentCheckPoint].SinglechipPort != currentKey || currentCheckPoint > currentCheckPointDic.Count || !_topicStart) return;

            this.currentKey = currentKey;
            //WLog.Log("ChangeSinglechipValue" + currentKey);
            bool isCurremtKey = currentCheckPointDic[currentCheckPoint].SinglechipPort == currentKey;
            Ctrl_MessageCenter.SendMessage<bool, int>("Comparison", isCurremtKey, currentCheckPointDic[currentCheckPoint].Id);
            checkPointDic[currentCheckPointDic[currentCheckPoint].Id].isCompleted = true;
            currentCheckPoint++;

            ////TODO:是否需要对之后的数据进行对比（似乎不在需要了）
            //for (int i = 0; i < currentCheckPointDic.Count; i++)
            //{
            //    //if (checkPointDic[currentCheckPointDic[currentCheckPoint].SinglechipPort])
            //    //{

            //    //}
            //}
        }

        /// <summary>
        /// 辅助回撤界面
        /// </summary>
        public int HelperIndex;

        private void OnUIDataScreenOpen(UIDataSceneOpenType type)
        {
            if (type == UIDataSceneOpenType.Help)
            {
                Ctrl_MessageCenter.AddMsgListener<MsgBase>("PadSubmitScore", OnRenWuFaBuTaskComplete);
            }
        }

        private void OnUIDataScreenClose(UIDataSceneOpenType type)
        {
            if (type == UIDataSceneOpenType.Help)
            {
                Ctrl_MessageCenter.RemoveMsgListener<MsgBase>("PadSubmitScore", OnRenWuFaBuTaskComplete);
                
                var iUIForm = GameEntry.UI.GetUIForm<UITrain>();
                if (iUIForm != null)
                {
                    var uiTrain = iUIForm as UITrain;

                    //发送切界面消息
                    //SendMsgManager.SendUINavigationMsg(new List<int>() { -1, 3, 3, HelperIndex }, 1, -1);

                    var uiTrainView = uiTrain.UIView as UIViewTrain;
                    uiTrainView.btnPageUp.transform.parent.gameObject.SetActive(true);
                    uiTrainView.tsContentList.gameObject.SetActive(true);
                    uiTrainView.tsShowItem.gameObject.SetActive(false);
                    uiTrain.ShowCommonPage();
                    uiTrain.ClearCheckPoints();
                    GameEntry.UI.CloseUIForm<UITrain>();
                    GameEntry.UI.OpenUIFormSync<UITopicList>();
                }
            }
        }

        public void OnDestroy()
        {
        }

        public void OnFixedUpdate()
        {
            //计时
            if (_topicStart)
            {
                userTime += Time.deltaTime;
            }
        }

        public void OnLateUpdate()
        {
        }

        public void OnUpdate()
        {
        }

        /// <summary>
        /// 统计全部需要检测的点
        /// </summary>
        /// <param name="config"></param>
        public void InitDic(List<FaultCheckConfig4th> config)
        {
            currentCheckPoint = 0;
            currentCheckPointDic.Clear();
            this.currentKey = 0;
            for (int i = 0; i < config.Count; i++)
            {
                currentCheckPointDic.Add(i, config[i]);

                if (!checkPointDic.ContainsKey(config[i].Id))
                {
                    checkPointDic.Add(config[i].Id, new CheckPointState(false, false));
                    //WLog.Log(checkPointDic.Count);
                }
            }
        }

        /// <summary>
        /// 点位检测完成
        /// </summary>
        /// <param name="singlePort"></param>
        /// <returns></returns>
        public CheckPointState CheckIsComplete(int singlePort)
        {
            CheckPointState errorOrWin = new CheckPointState(false, false);
            if (checkPointDic[singlePort].isCompleted)
                errorOrWin = checkPointDic[singlePort];

            if (errorOrWin.isCompleted)
            {
                Debug.Log("检测完成");
                currentCheckPoint++;
                WLog.Log(currentCheckPoint);
            }

            return errorOrWin;
        }

        /// <summary>
        /// 装载训练模式下检测设置完成
        /// </summary>
        /// <param name="singlePort"></param>
        /// <returns></returns>
        public void TrainCheckIsComplete(int singlePort)
        {
            if (checkPointDic.ContainsKey(singlePort))
            {
                checkPointDic[singlePort].isCompleted = true;
            }
        }

        public void TopicStart(int itemID)
        {
            if (resoultInfos != null && resoultInfos.Count > 0)
            {
                for (int i = 0; i < resoultInfos.Count; i++)
                {
                    if (resoultInfos[i].ItemID == itemID)
                        currentTopicInfo = resoultInfos[i];
                }
            }

            currentCheckPoint = 0;
            BeginTime = DateTime.Now.ToString();

            userTime = 0;
            _topicStart = true;
            Score = 100;
        }

        public void TopicEnd()
        {
            currentCheckPointDic.Clear();
            EndTime = DateTime.Now.ToString();
            _topicStart = false;
            checkPointDic.Clear();
        }

        public List<TopicResoultInfo> DisposeHttpTopic()
        {
            var topicAccount = new TopicAccount() { ID = 2 };
            HtttpWebRequestManager.HttpPost<ResponseTopicData>(AppConst.Protocol.GetTopicInfoByAccount, topicAccount, (response) =>
            {
                if (response.code != 1000)
                {
                    WLog.Log("课题列表获取失败");
                }
                else
                {
                    topicInfos = response.infos;
                }
            });

            return topicInfos;
        }

        public string ConvertSecondsToTime(float seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString("hh\\:mm\\:ss");
        }


        public bool GetIsComplete()
        {
            return currentCheckPoint == currentCheckPointDic.Count - 1;
        }

        /// <summary>
        /// 上传学生信息以及教员评价
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="evaluationContent"></param>
        /// <param name="evaluationScore"></param>
        public async void SubmitStudentInfo(string name, string id, string evaluationContent = null, string evaluationScore = null)
        {
            EvaluationScore = string.IsNullOrEmpty(evaluationScore) ? 0 : Mathf.Clamp(int.Parse(evaluationScore),0,100);

            var studentInfo = new StudentPost() { Name = name, ID = id };
            var EvaluationInfo = new EvaluationPost()
            {
                UserName = name,
                TaskID = TopicManager.Instance.currentTopicInfo.ID,
                EvaluationContent = evaluationContent,
                EvaluationScore = EvaluationScore.ToString()

            };
            SettlementManager.Instance._evaluationPost = EvaluationInfo;

            //Debug.Log(studentInfo);
            //Debug.Log(EvaluationInfo);

            await HtttpWebRequestManager.HttpPostAsync<ResponseStudent>(AppConst.Protocol.GetUserInfoByAccount, studentInfo, (response) =>
            {
                if (response.code != 1000)
                {
                    WLog.Log("学生信息上传失败");
                }
                else
                {
                    //topicInfos = response.infos;
                }
            });

            await HtttpWebRequestManager.HttpPostAsync<ResponseEvaluation>(AppConst.Protocol.AddEvaluation, EvaluationInfo, (response) =>
            {
                if (response.code != 1000)
                {
                    WLog.Log("评价信息上传失败");
                }
                else
                {
                    //topicInfos = response.infos;
                }
            });
        }

        /// <summary>
        /// 上传传感器数据
        /// </summary>
        public async void UploadSensorData(SensorPost sensorPost)
        {
            Debug.Log(sensorPost);
            await HtttpWebRequestManager.HttpPostAsync<ResponseSensor>(AppConst.Protocol.AddSensor, sensorPost, (response) =>
            {
                if (response.code != 1000)
                {
                    WLog.Log("传感数据上传失败");
                }
                else
                {
                    //topicInfos = response.infos;
                }
            });
        }

        /// <summary>
        /// 当课题结算时
        /// </summary>
        public void OnTaskCompleteHandle()
        {
            GameManager.Instance.RestCinemachineCamera();
            GameManager.Instance.ShowAllParts();
            GameManager.Instance.ShowAllRenderer();

            if (GameEntry.UI.HasUIForm("UITrain"))
            {
                GameEntry.UI.CloseUIForm<UITrain>();
            }

            GameEntry.UI.OpenUIFormSync<UIStudentInfoRecord>();
        }

        private void OnRenWuFaBuTaskComplete(MsgBase msg)
        {
            OnTaskCompleteHandle();

            if (GameEntry.UI.HasUIForm("UIDataScreen"))
            {
                GameEntry.UI.CloseUIForm<UIDataScreen>();
            }

            ServNet.Instance.SetCacheMsgBaseState(msg);
        }
    }
}

/// <summary>
/// 实训类型
/// </summary>
public enum TrainType
{
    None,

    /// <summary>
    /// 发动机
    /// </summary>
    EngineType,

    /// <summary>
    /// 底盘
    /// </summary>
    UnderpanType,

    /// <summary>
    /// 电气
    /// </summary>
    ElectricalType,

    /// <summary>
    /// 任务发布
    /// </summary>
    RenWuFaBu
}