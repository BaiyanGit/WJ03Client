
using Wx.Runtime.Http;

namespace Hotfix
{
    public class RegisterRequest : RequestDataBase
    {
        public string account;
        public string password;
    }

    public class RegisterResponse : ResponseDataBase
    {
        public string account;
        public string password;
    }
}