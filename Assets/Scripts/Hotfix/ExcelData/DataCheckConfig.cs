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
    public partial class DataCheckConfig : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 标题(故障)
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// 类型
	/// </summary>
	public int Type { get; set; }

	/// <summary>
	/// 描述
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 对应的UI名称
	/// </summary>
	public string UIModelName { get; set; }

	/// <summary>
	/// 对应错误项
	/// </summary>
	public int DefaultID { get; set; }

	/// <summary>
	/// 故障点对应的动画
	/// </summary>
	public string Anim { get; set; }

	/// <summary>
	/// 对应的传感器（EquipmentCheckConfig2nd）
	/// </summary>
	public int[] TargetSensors { get; set; }

	/// <summary>
	/// 模型路径
	/// </summary>
	public string[] ModelPaths { get; set; }

	/// <summary>
	/// 提示点偏移轴向
	/// </summary>
	public string TipPosOffsetAxis { get; set; }

	/// <summary>
	/// 提示点偏移量
	/// </summary>
	public float TipPosOffsetValue { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class DataCheckConfigTable : ExcelTable
    {
        public static DataCheckConfigTable Instance;
    
        public readonly List<DataCheckConfig> dataList;
        private Dictionary<int, DataCheckConfig> _dataDict = new Dictionary<int, DataCheckConfig>();

        public DataCheckConfigTable()
        {
            Instance = this;
        }

        public DataCheckConfig Get(int id)
        {
            _dataDict.TryGetValue(id, out DataCheckConfig value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(DataCheckConfig)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, DataCheckConfig> GetAll()
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
