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
    public partial class ModuleConfig : ExcelObject, IExcelData
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
	/// 界面类名
	/// </summary>
	public string ClassName { get; set; }

	/// <summary>
	/// Pad端界面适配参数
	/// </summary>
	public string AdapterParam { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class ModuleConfigTable : ExcelTable
    {
        public static ModuleConfigTable Instance;
    
        public readonly List<ModuleConfig> dataList;
        private Dictionary<int, ModuleConfig> _dataDict = new Dictionary<int, ModuleConfig>();

        public ModuleConfigTable()
        {
            Instance = this;
        }

        public ModuleConfig Get(int id)
        {
            _dataDict.TryGetValue(id, out ModuleConfig value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(ModuleConfig)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, ModuleConfig> GetAll()
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
