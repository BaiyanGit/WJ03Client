using UnityEngine;
using Wx.Runtime.Http;

namespace Hotfix
{
    public struct StudentPost
    {
        public string Name;
        public string ID;
    }

    /// <summary>
    /// ���ؽṹ
    /// </summary>
    public struct ResponseStudent
    {
        /// <summary>
        /// �����룬1000Ϊ����������ȫ����Ϊ�д�
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public LoginResoultUserInfo infos;
        //public string infos;
        /// <summary>
        /// ��code�����ֽ���
        /// </summary>
        public string msg;
    }
}

