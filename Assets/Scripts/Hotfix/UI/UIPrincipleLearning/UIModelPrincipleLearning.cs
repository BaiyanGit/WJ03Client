using Hotfix.ExcelData;
using System.Collections.Generic;

using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 原理学习界面
    /// </summary>
    public class UIModelPrincipleLearning : IUIModel
    {
        /// <summary>
        /// 主结构ID-子结构配置列表
        /// </summary>
        private Dictionary<int, List<PrincipleConfig>> principleConfigs;

        public Dictionary<int, List<PrincipleConfig>> PrincipleConfigs
        {
            get
            {
                if (principleConfigs == null)
                {
                    principleConfigs = new();

                    foreach (var config in MainStructureConfigTable.Instance.dataList)
                    {
                        principleConfigs.Add(config.Id, new());
                    }

                    foreach (var item in PrincipleConfigTable.Instance.dataList)
                    {
                        principleConfigs[item.Type].Add(item);
                    }
                }

                return principleConfigs;
            }
        }
    }
}