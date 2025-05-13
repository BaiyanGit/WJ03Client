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
    /// 返回结构
    /// </summary>
    public struct ResponseStudent
    {
        /// <summary>
        /// 错误码，1000为正常，其他全部视为有错
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public LoginResoultUserInfo infos;
        //public string infos;
        /// <summary>
        /// 对code的文字解释
        /// </summary>
        public string msg;
    }
}

