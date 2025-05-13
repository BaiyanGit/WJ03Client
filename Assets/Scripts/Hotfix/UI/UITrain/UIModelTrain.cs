using Hotfix.ExcelData;
using System.Collections.Generic;
using Wx.Runtime.UI;
using System.Linq;
using System;

namespace Hotfix.UI
{
    /// <summary>
    /// 故障训练界面
    /// </summary>
    public class UIModelTrain : IUIModel
    {
        private Dictionary<int, List<FaultCheckConfig2nd>> faultCheckConfig2ndCacheDic = new();
        private Dictionary<int, List<FaultCheckConfig3rd>> faultCheckConfig3rdCacheDic = new();
        private Dictionary<int, List<FaultCheckConfig4th>> faultCheckConfig4rdCacheDic = new();


        /// <summary>
        /// 根据主ID获取所有的次配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FaultCheckConfig2nd> GetFaultCheckConfig2nds(int id)
        {
            if (faultCheckConfig2ndCacheDic.ContainsKey(id))
            {
                return faultCheckConfig2ndCacheDic[id];
            }

            var temp = new List<FaultCheckConfig2nd>();

            for (int i = 0; i < FaultCheckConfig2ndTable.Instance.dataList.Count; i++)
            {
                if (FaultCheckConfig2ndTable.Instance.dataList[i].Type == id)
                {
                    temp.Add(FaultCheckConfig2ndTable.Instance.dataList[i]);
                }
            }

            faultCheckConfig2ndCacheDic.Add(id, temp);

            return temp;
        }

        /// <summary>
        /// 根据网络获取所有配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FaultCheckConfig2nd> GetFaultCheckConfig2nds()
        {
            //if (faultCheckConfig2ndCacheDic.ContainsKey(id))
            //{
            //    return faultCheckConfig2ndCacheDic[id];
            //}

            var temp = new List<FaultCheckConfig2nd>();

            for (int i = 0; i < FaultCheckConfig2ndTable.Instance.dataList.Count; i++)
            {
                //if (FaultCheckConfig2ndTable.Instance.dataList[i].Type == NetWork.ID)
                //{
                //    temp.Add(FaultCheckConfig2ndTable.Instance.dataList[i]);
                //}
            }

            //faultCheckConfig2ndCacheDic.Add(id, temp); 

            return temp;
        }

        /// <summary>
        /// 根据指定页数和每页数量获取配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageIndex">从0开始</param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public List<FaultCheckConfig2nd> GetFaultCheckConfig2nds(int pageIndex, int pageCount)
        {
            var temp = new List<FaultCheckConfig2nd>();

            var list = FaultCheckConfig2ndTable.Instance.dataList;

            int startIndex = pageIndex * pageCount;

            int endIndex = startIndex + pageCount;

            int finalIndex = Math.Min(endIndex, list.Count);

            for (int i = startIndex; i < finalIndex; i++)
            {
                temp.Add(list[i]);
            }

            return temp;
        }

        /// <summary>
        /// 根据次ID获取所有的次次配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FaultCheckConfig3rd> GetFaultCheckConfig3rds(int id)
        {
            if (faultCheckConfig3rdCacheDic.ContainsKey(id))
            {
                return faultCheckConfig3rdCacheDic[id];
            }

            var temp = new List<FaultCheckConfig3rd>();

            for (int i = 0; i < FaultCheckConfig3rdTable.Instance.dataList.Count; i++)
            {
                if (FaultCheckConfig3rdTable.Instance.dataList[i].Type == id)
                {
                    temp.Add(FaultCheckConfig3rdTable.Instance.dataList[i]);
                }
            }

            faultCheckConfig3rdCacheDic.Add(id, temp);

            return temp;
        }

        /// <summary>
        /// 根据次ID获取所有的次次配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FaultCheckConfig4th> GetFaultCheckConfig4rds(int id)
        {
            if (faultCheckConfig4rdCacheDic.ContainsKey(id))
            {
                return faultCheckConfig4rdCacheDic[id];
            }

            var temp = new List<FaultCheckConfig4th>();

            for (int i = 0; i < FaultCheckConfig4thTable.Instance.dataList.Count; i++)
            {
                if (FaultCheckConfig4thTable.Instance.dataList[i].Type == id)
                {
                    temp.Add(FaultCheckConfig4thTable.Instance.dataList[i]);
                }
            }

            faultCheckConfig4rdCacheDic.Add(id, temp);

            return temp;
        }
    }
}