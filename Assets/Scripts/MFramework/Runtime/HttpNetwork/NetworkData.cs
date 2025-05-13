using System;

namespace Wx.Runtime.Http
{
    [Serializable]
    public class RequestDataBase
    {
    }

    [Serializable]
    public class ResponseDataBase
    {
        public int code;
        public string msg;
    }
    
}