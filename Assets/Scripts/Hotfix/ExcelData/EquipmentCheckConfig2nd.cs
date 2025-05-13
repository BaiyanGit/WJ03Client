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
    public partial class EquipmentCheckConfig2nd : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 展示类型（1-油量2-胎压3-电压 etc...EquipmentCheckConfig1st）
	/// </summary>
	public int Type { get; set; }

	/// <summary>
	/// 标题
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// 描述
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 传感器标识
	/// </summary>
	public string SensorTag { get; set; }

	/// <summary>
	/// 正常值范围
	/// </summary>
	public float[] TargetValues { get; set; }

	/// <summary>
	/// 按键匹配
	/// </summary>
	public int SinglechipKey { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class EquipmentCheckConfig2ndTable : ExcelTable
    {
        public static EquipmentCheckConfig2ndTable Instance;
    
        public readonly List<EquipmentCheckConfig2nd> dataList;
        private Dictionary<int, EquipmentCheckConfig2nd> _dataDict = new Dictionary<int, EquipmentCheckConfig2nd>();

        public EquipmentCheckConfig2ndTable()
        {
            Instance = this;
        }

        public EquipmentCheckConfig2nd Get(int id)
        {
            _dataDict.TryGetValue(id, out EquipmentCheckConfig2nd value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(EquipmentCheckConfig2nd)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, EquipmentCheckConfig2nd> GetAll()
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
