using Wx.Runtime.Http;

namespace Hotfix
{
    public class LoginRequest : RequestDataBase
    {
        public string account;
        public string password;
    }

    public class LoginResponse : ResponseDataBase
    {
        public string account;
        public string password;
    }

    public struct LoginPost
    {
        public string Account;
    }

    public struct LoginResoultUserInfo
    {
        public string Account;
        public string Password;
        public string UserName;
        public int MainID;
    }

    /// <summary>
    /// 返回结构
    /// </summary>
    public struct ResponseLogin
    {
        /// <summary>
        /// 错误码，1000为正常，其他全部视为有错
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public LoginResoultUserInfo infos;
        /// <summary>
        /// 对code的文字解释
        /// </summary>
        public string msg;
    }
}
