using Hotfix.ExcelData;
using System.Collections.Generic;
using Wx.Runtime.UI;

namespace Hotfix.UI
{
    /// <summary>
    /// 设备监测界面
    /// </summary>
    public class UIModelEquipmentMonitoring : IUIModel
    {
        private Dictionary<int, List<EquipmentCheckConfig2nd>> equipmentCheckConfig2ndCacheDic = new();

        public List<EquipmentCheckConfig2nd> GetEquipmentCheckConfig2nds(int id)
        {
            if (equipmentCheckConfig2ndCacheDic.ContainsKey(id))
            {
                return equipmentCheckConfig2ndCacheDic[id];
            }

            var temp = new List<EquipmentCheckConfig2nd>();

            for (int i = 0; i < EquipmentCheckConfig2ndTable.Instance.dataList.Count; i++) {

                if (EquipmentCheckConfig2ndTable.Instance.dataList[i].Type == id)
                {
                    temp.Add(EquipmentCheckConfig2ndTable.Instance.dataList[i]);
                }
            }

            equipmentCheckConfig2ndCacheDic.Add(id, temp);
            return temp;
        }

    }
}