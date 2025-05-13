using System.Collections.Generic;
using Wx.Runtime.Http;

namespace Hotfix
{
    /// <summary>
    /// 监测项信息
    /// </summary>
    public class CheckItemData
    {
        public int id;
        public int serialId;
        public int btnSerialId;
        public string name;
        public string referenceValue;
    }
    
    /// <summary>
    /// 监测项请求信息
    /// monitorDataId = -1 时，返回所有检测项信息
    /// </summary>
    public class CheckItemRequest : RequestDataBase
    {
        public int monitorDataId;
    }

    /// <summary>
    /// 监测项响应信息
    /// </summary>
    public class CheckItemResponse : ResponseDataBase
    {
        public List<CheckItemData> dataList;
    }
}