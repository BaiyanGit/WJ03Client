using Wx.Runtime.UI;
using System;
using System.Collections.Generic;
using Hotfix.ExcelData;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIModelDataScreen : IUIModel
    {
        private Dictionary<int, List<DataCheckConfig>> dataCheckConfigCacheDic = new();

        /// <summary>
        /// 根据类型返回数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<DataCheckConfig> GetDataCheckConfigs(int type = 1)
        {
            if (dataCheckConfigCacheDic.ContainsKey(type))
            {
                return dataCheckConfigCacheDic[type];
            }

            var temp = new List<DataCheckConfig>();

            for (int i = 0; i < DataCheckConfigTable.Instance.dataList.Count; i++)
            {
                if (DataCheckConfigTable.Instance.dataList[i].Type == type)
                {
                    temp.Add(DataCheckConfigTable.Instance.dataList[i]);
                }
            }

            dataCheckConfigCacheDic.Add(type, temp);

            return temp;
        }


        public string GetMainStructureConfigTableTitle(int id)
        {
            string tmpTitle = null;
            foreach (var item in MainStructureConfigTable.Instance.dataList)
            {
                if (item.Id == id - 1)
                {
                    tmpTitle = item.Title;
                }
            }
            return tmpTitle;
        }
    }
}