using System.Collections.Generic;
using Wx.Runtime.Http;

namespace Hotfix
{
    public class MonitorData : ResponseDataBase
    {
        public int id;
        public string name;             
    }
    
    public class MonitorDataListRequest : RequestDataBase
    {
    }
    
    public class MonitorDataListResponse : ResponseDataBase
    {
        public List<MonitorData> dataList; 
    }
}