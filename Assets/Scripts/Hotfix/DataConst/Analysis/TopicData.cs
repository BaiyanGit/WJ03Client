using System.Collections.Generic;
using NUnit.Framework;
using Wx.Runtime.Http;

namespace Hotfix
{
    public class TopicData
    {
    }

    /// <summary>
    /// �������
    /// </summary>
    public class CheckPointState
    {
        public bool isCompleted;
        /// <summary>
        /// true��Win��false��Error
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
        /// ��ӦTaskID
        /// </summary>
        public int ItemID;
    }

    public struct ResponseTopicData
    {
        /// <summary>
        /// ������
        /// </summary>
        public int code;
        /// <summary>
        /// �����б�
        /// </summary>
        public List<TopicResoultInfo> infos;
        /// <summary>
        /// 
        /// </summary>
        public string msg;
    }

    #region ���ӿ��˼�¼
    public struct RecordExamData
    {
        public int UserID;
        public string BeginTime;
        public string EndTime;
        public int Score;
        /// <summary>
        /// ��ӦTopicResoultInfo�е�ItemID
        /// </summary>
        public int ItemID;
        public int Type;
        /// <summary>
        /// ��ӦTopicResoultInfo�е�ID
        /// </summary>
        public int TaskID;
        /// <summary>
        /// �۷��б�
        /// </summary>
        public List<ResponseRecordExamInfo> ItemList;
        /// <summary>
        /// ����
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

