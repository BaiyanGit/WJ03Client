using System.Collections.Generic;
using Wx.Runtime.Singleton;

namespace Hotfix
{
    /// <summary>
    /// 结算管理器
    /// </summary>
    public class SettlementManager : SingletonInstance<SettlementManager>, ISingleton
    {
        private string _modelName;
        private int _errorCount;
        private string _time;
        private List<ResponseRecordExamInfo> _errorList;
        public EvaluationPost _evaluationPost;

        public string ModelName
        {
            get { return _modelName; }
            set { _modelName = value; }
        }

        public int ErrorCount
        {
            get { return _errorCount; }
            set { _errorCount = value; }
        }

        public string Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public List<ResponseRecordExamInfo> ErrorList
        {
            get { return _errorList; }
            set { _errorList = value; }
        }

        public void OnCreate(object createParam)
        {
            _modelName = string.Empty;
            _errorCount = 0;
            _time = string.Empty;
            _errorList = new List<ResponseRecordExamInfo>();
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLateUpdate()
        {
        }

        public void OnDestroy()
        {
        }


        /// <summary>
        /// 上传作业数据
        /// </summary>
        public async void UploadData()
        {
            var recordExamData = new RecordExamData();
            recordExamData.UserID = UserManager.Instance.MainID;
            recordExamData.BeginTime = TopicManager.Instance.BeginTime;
            recordExamData.EndTime = TopicManager.Instance.EndTime;
            recordExamData.Score = (int)TopicManager.Instance.Score;
            recordExamData.ItemID = TopicManager.Instance.currentTopicInfo.ItemID;
            recordExamData.Type = (int)GameManager.Instance.AssessmentMode;
            recordExamData.TaskID = TopicManager.Instance.currentTopicInfo.ID; 
            recordExamData.ItemList = _errorList;
            recordExamData.TeacherEvaluation = _evaluationPost;
            await HtttpWebRequestManager.HttpPostAsync<ExamInfoResoultData>(AppConst.Protocol.AddExamRecord, recordExamData, (response) =>
            {
                if (response.code != 1000)
                {
                    WLog.Log("上传数据失败");
                }
                else
                {
                    WLog.Log(response.msg);
                    //topicInfos = response.infos;
                }
            });

            //HtttpWebRequestManager.HttpPost<ExamInfoResoultData>(AppConst.Protocol.AddExamRecord, recordExamData, (response) =>
            //{
            //    if (response.code != 1000)
            //    {
            //        WLog.Log("上传数据失败");
            //    }
            //    else
            //    {
            //        WLog.Log(response.msg);
            //        //topicInfos = response.infos;
            //    }
            //});
        }
    }
}