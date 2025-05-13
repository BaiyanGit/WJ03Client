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
    public partial class FaultCheckConfig1st : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 标题
	/// </summary>
	public string Title { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class FaultCheckConfig1stTable : ExcelTable
    {
        public static FaultCheckConfig1stTable Instance;
    
        public readonly List<FaultCheckConfig1st> dataList;
        private Dictionary<int, FaultCheckConfig1st> _dataDict = new Dictionary<int, FaultCheckConfig1st>();

        public FaultCheckConfig1stTable()
        {
            Instance = this;
        }

        public FaultCheckConfig1st Get(int id)
        {
            _dataDict.TryGetValue(id, out FaultCheckConfig1st value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(FaultCheckConfig1st)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, FaultCheckConfig1st> GetAll()
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
