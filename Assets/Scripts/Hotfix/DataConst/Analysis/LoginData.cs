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
    /// ���ؽṹ
    /// </summary>
    public struct ResponseLogin
    {
        /// <summary>
        /// �����룬1000Ϊ����������ȫ����Ϊ�д�
        /// </summary>
        public int code;
        /// <summary>
        /// ToDo:JsonString
        /// </summary>
        public LoginResoultUserInfo infos;
        /// <summary>
        /// ��code�����ֽ���
        /// </summary>
        public string msg;
    }
}
