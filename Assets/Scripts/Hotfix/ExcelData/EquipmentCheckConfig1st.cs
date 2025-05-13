using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Wx.Runtime.Excel;


namespace Hotfix.ExcelData
{
    /// <summary>
    /// 常量属性
    /// </summary>
    [System.Serializable]
    public partial class EquipmentCheckConfig1st : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 标题
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// 描述
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 单位
	/// </summary>
	public string Unit { get; set; }

	/// <summary>
	/// 标签图标
	/// </summary>
	public string Icon { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class EquipmentCheckConfig1stTable : ExcelTable
    {
        public static EquipmentCheckConfig1stTable Instance;
    
        public readonly List<EquipmentCheckConfig1st> dataList;
        private Dictionary<int, EquipmentCheckConfig1st> _dataDict = new Dictionary<int, EquipmentCheckConfig1st>();

        public EquipmentCheckConfig1stTable()
        {
            Instance = this;
        }

        public EquipmentCheckConfig1st Get(int id)
        {
            _dataDict.TryGetValue(id, out EquipmentCheckConfig1st value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(EquipmentCheckConfig1st)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, EquipmentCheckConfig1st> GetAll()
        {
            return _dataDict;
        }
    
        public override void EndInit()
        {
            foreach (var edItemBase in dataList)
            {
                edItemBase.EndInit();
                _dataDict.Add(edItemBase.Id, edItemBase);
            }
            AfterEndInit();
        }
    }
}
