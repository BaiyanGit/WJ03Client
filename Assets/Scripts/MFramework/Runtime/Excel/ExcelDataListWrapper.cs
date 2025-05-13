using System;
using System.Collections.Generic;

namespace Wx.Runtime.Excel
{
    /// <summary>
    /// 表示所需的JSON结构
    /// </summary>
    [Serializable]
    public class ExcelDataListWrapper<T> where T : ExcelObject
    {
        public List<T> dataList;
    }
}