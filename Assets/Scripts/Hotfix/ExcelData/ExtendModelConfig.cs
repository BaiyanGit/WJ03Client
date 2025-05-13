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
    public partial class ExtendModelConfig : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 名称
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// 相对路径
	/// </summary>
	public string RelativePath { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class ExtendModelConfigTable : ExcelTable
    {
        public static ExtendModelConfigTable Instance;
    
        public readonly List<ExtendModelConfig> dataList;
        private Dictionary<int, ExtendModelConfig> _dataDict = new Dictionary<int, ExtendModelConfig>();

        public ExtendModelConfigTable()
        {
            Instance = this;
        }

        public ExtendModelConfig Get(int id)
        {
            _dataDict.TryGetValue(id, out ExtendModelConfig value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(ExtendModelConfig)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, ExtendModelConfig> GetAll()
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
