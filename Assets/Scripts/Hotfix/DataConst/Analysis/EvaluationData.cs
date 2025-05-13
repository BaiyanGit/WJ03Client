using UnityEngine;

namespace Hotfix
{
    /// <summary>
    /// 评价
    /// </summary>
    public struct EvaluationPost
    {
        public string UserName;
        public int TaskID;
        public string EvaluationContent;
        public string EvaluationScore;
    }

    /// <summary>
    /// 返回结构
    /// </summary>
    public struct ResponseEvaluation
    {
        /// <summary>
        /// 错误码，1000为正常，其他全部视为有错
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public int infos;
        /// <summary>
        /// 对code的文字解释
        /// </summary>
        public string msg;
    }
}
