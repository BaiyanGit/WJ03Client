using Hotfix.ExcelData;

using System.Collections.Generic;

using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 结构认知界面
    /// </summary>
    public class UIModelStructuralCognition : IUIModel
    {
        /// <summary>
        /// 主结构ID-子结构配置列表
        /// </summary>
        private Dictionary<int, List<StructureConfig>> structureConfigs;

        public Dictionary<int, List<StructureConfig>> StructureConfigs
        {
            get
            {
                if (structureConfigs == null)
                {
                    structureConfigs = new();

                    foreach (var config in MainStructureConfigTable.Instance.dataList)
                    {
                        structureConfigs.Add(config.Id, new());
                    }

                    foreach (var item in StructureConfigTable.Instance.dataList)
                    {
                        structureConfigs[item.Type].Add(item);
                    }
                }

                return structureConfigs;
            }
        }
    }
}