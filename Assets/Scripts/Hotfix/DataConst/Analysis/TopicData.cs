using System.Collections.Generic;
using NUnit.Framework;
using Wx.Runtime.Http;

namespace Hotfix
{
    public class TopicData
    {
    }

    /// <summary>
    /// 检查数据
    /// </summary>
    public class CheckPointState
    {
        public bool isCompleted;
        /// <summary>
        /// true是Win，false是Error
        /// </summary>
        public bool isErrorOrWin;

        public CheckPointState(bool completed, bool errorOrWin)
        {
            isCompleted = completed;
            isErrorOrWin = errorOrWin;
        }
    }

    public struct TopicAccount
    {
        public int ID;
    }

    public struct TopicResoultInfo
    {
        public int ID;
        /// <summary>
        /// 对应TaskID
        /// </summary>
        public int ItemID;
    }

    public struct ResponseTopicData
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int code;
        /// <summary>
        /// 课题列表
        /// </summary>
        public List<TopicResoultInfo> infos;
        /// <summary>
        /// 
        /// </summary>
        public string msg;
    }

    #region 增加考核记录
    public struct RecordExamData
    {
        public int UserID;
        public string BeginTime;
        public string EndTime;
        public int Score;
        /// <summary>
        /// 对应TopicResoultInfo中的ItemID
        /// </summary>
        public int ItemID;
        public int Type;
        /// <summary>
        /// 对应TopicResoultInfo中的ID
        /// </summary>
        public int TaskID;
        /// <summary>
        /// 扣分列表
        /// </summary>
        public List<ResponseRecordExamInfo> ItemList;
        /// <summary>
        /// 评价
        /// </summary>
        public EvaluationPost TeacherEvaluation;
    }

    public struct ResponseRecordExamInfo
    {
        public int PointID;
        public int Score;
        public string CreateTime;
    }

    public struct ExamInfoResoultData
    {
        public int code;
        public int infos;
        public string msg;
    }
    #endregion
}

