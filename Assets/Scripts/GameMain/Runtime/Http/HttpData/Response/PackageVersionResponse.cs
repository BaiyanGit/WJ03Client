using System;
using Wx.Runtime.Http;

namespace GameMain.Runtime
{
    [Serializable]
    public class PackageVersionResponse : ResponseDataBase
    {
        public string version;
    }
}
