using UnityEngine;

namespace Hotfix
{
    /// <summary>
    /// ����
    /// </summary>
    public struct EvaluationPost
    {
        public string UserName;
        public int TaskID;
        public string EvaluationContent;
        public string EvaluationScore;
    }

    /// <summary>
    /// ���ؽṹ
    /// </summary>
    public struct ResponseEvaluation
    {
        /// <summary>
        /// �����룬1000Ϊ����������ȫ����Ϊ�д�
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public int infos;
        /// <summary>
        /// ��code�����ֽ���
        /// </summary>
        public string msg;
    }
}
