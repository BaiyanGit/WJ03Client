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
    public partial class StructureConfig : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 类型
	/// 0：发动机
	/// 1：底盘系统
	/// 2：电气系统
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
	/// 模型名称数组（用|隔开）
	/// </summary>
	public string[] ModelPaths { get; set; }

	/// <summary>
	/// 模型隐藏排除数组（用|隔开）
	/// </summary>
	public string[] IgnoreModelPaths { get; set; }

	/// <summary>
	/// 相机初始位置
	/// </summary>
	public float[] TargetPos { get; set; }

	/// <summary>
	/// 相机初始朝向
	/// </summary>
	public float[] TargetRot { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class StructureConfigTable : ExcelTable
    {
        public static StructureConfigTable Instance;
    
        public readonly List<StructureConfig> dataList;
        private Dictionary<int, StructureConfig> _dataDict = new Dictionary<int, StructureConfig>();

        public StructureConfigTable()
        {
            Instance = this;
        }

        public StructureConfig Get(int id)
        {
            _dataDict.TryGetValue(id, out StructureConfig value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(StructureConfig)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, StructureConfig> GetAll()
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
