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
    public partial class MainStructureConfig : ExcelObject, IExcelData
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
	/// 模型相对路径
	/// </summary>
	public string ModelPath { get; set; }

	/// <summary>
	/// 相机初始位置
	/// </summary>
	public float[] TargetPos { get; set; }

	/// <summary>
	/// 相机初始朝向
	/// </summary>
	public float[] TargetRot { get; set; }

	/// <summary>
	/// 图标
	/// </summary>
	public string Image { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class MainStructureConfigTable : ExcelTable
    {
        public static MainStructureConfigTable Instance;
    
        public readonly List<MainStructureConfig> dataList;
        private Dictionary<int, MainStructureConfig> _dataDict = new Dictionary<int, MainStructureConfig>();

        public MainStructureConfigTable()
        {
            Instance = this;
        }

        public MainStructureConfig Get(int id)
        {
            _dataDict.TryGetValue(id, out MainStructureConfig value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(MainStructureConfig)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, MainStructureConfig> GetAll()
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
