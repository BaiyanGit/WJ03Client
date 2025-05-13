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
    public partial class FaultCheckConfig2nd : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 标题
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// 展示类型（1-发动机部分2-底盘部分3-转向系部分4-制动系部分5-电气设备部分）
	/// </summary>
	public int Type { get; set; }

	/// <summary>
	/// 描述
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// 任务类型（是否任务发布）
	/// </summary>
	public int TrainType { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class FaultCheckConfig2ndTable : ExcelTable
    {
        public static FaultCheckConfig2ndTable Instance;
    
        public readonly List<FaultCheckConfig2nd> dataList;
        private Dictionary<int, FaultCheckConfig2nd> _dataDict = new Dictionary<int, FaultCheckConfig2nd>();

        public FaultCheckConfig2ndTable()
        {
            Instance = this;
        }

        public FaultCheckConfig2nd Get(int id)
        {
            _dataDict.TryGetValue(id, out FaultCheckConfig2nd value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(FaultCheckConfig2nd)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, FaultCheckConfig2nd> GetAll()
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
