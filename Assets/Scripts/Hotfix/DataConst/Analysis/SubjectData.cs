using System.Collections.Generic;
using Wx.Runtime.Http;

namespace Hotfix
{
    public class SubjectData
    {
        public int subjectId;
        public string name;
        public string describe;
    }
    
    public class SubjectRequest : RequestDataBase
    {
        public int modelId;
    }
    
    public class SubjectListResponse : ResponseDataBase
    {
        public List<SubjectData> dataList;
    }
}