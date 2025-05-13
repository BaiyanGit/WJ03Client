using Hotfix.ExcelData;
using System.Collections.Generic;
using System;
using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIModelTopicList : IUIModel
    {
        private Dictionary<int, List<FaultCheckConfig2nd>> faultCheckConfig2ndCacheDic = new();

        /// <summary>
        /// 根据网络获取所有配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FaultCheckConfig2nd> GetFaultCheckConfig2nds()
        {
            List<TopicResoultInfo> resoultInfos = TopicManager.Instance.DisposeHttpTopic();
            TopicManager.Instance.resoultInfos = resoultInfos;

            var temp = new List<FaultCheckConfig2nd>();
            for (int i = 0; i < FaultCheckConfig2ndTable.Instance.dataList.Count; i++)
            {
                FaultCheckConfig2nd faultCheck = FaultCheckConfig2ndTable.Instance.dataList[i];
                for (int j = 0; j < resoultInfos.Count; j++)
                {
                    if (faultCheck.Id == resoultInfos[j].ItemID)
                    {
                        temp.Add(faultCheck);
                    }
                }
            }
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
        /// 根据主ID获取所有的次配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<FaultCheckConfig2nd> GetFaultCheckConfig2nds(int id)
        {
            //特殊处理任务发布的课题
            if ((TrainType)id == TrainType.RenWuFaBu)
            {
                return GetFaultCheckConfig2ndsByTrainType();
            }

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
        /// 获取任务发布的所有数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<FaultCheckConfig2nd> GetFaultCheckConfig2ndsByTrainType(int type = 1)
        {
            var temp = new List<FaultCheckConfig2nd>();

            for (int i = 0; i < FaultCheckConfig2ndTable.Instance.dataList.Count; i++)
            {
                if (FaultCheckConfig2ndTable.Instance.dataList[i].TrainType == type)
                {
                    temp.Add(FaultCheckConfig2ndTable.Instance.dataList[i]);
                }
            }

            return temp;
        }
    }
}