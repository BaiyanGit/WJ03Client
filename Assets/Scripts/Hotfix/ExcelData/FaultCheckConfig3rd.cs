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
    public partial class FaultCheckConfig3rd : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 标题
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// 分类类型（etc...）
	/// </summary>
	public int Type { get; set; }

	/// <summary>
	/// 代理中心点
	/// </summary>
	public float[] CenterPos { get; set; }

	/// <summary>
	/// 目标模型路径
	/// </summary>
	public string[] TargetModel { get; set; }

	/// <summary>
	/// 相机注视点
	/// </summary>
	public float[] TargetPos { get; set; }

	/// <summary>
	/// 相机注视朝向
	/// </summary>
	public float[] TargetRot { get; set; }

	/// <summary>
	/// 故障展示类型（1-音频2-预制物etc）
	/// </summary>
	public int[] ShowType { get; set; }

	/// <summary>
	/// 故障展示参数
	/// </summary>
	public string[] ShowParam { get; set; }

	/// <summary>
	/// 描述
	/// </summary>
	public string Description { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class FaultCheckConfig3rdTable : ExcelTable
    {
        public static FaultCheckConfig3rdTable Instance;
    
        public readonly List<FaultCheckConfig3rd> dataList;
        private Dictionary<int, FaultCheckConfig3rd> _dataDict = new Dictionary<int, FaultCheckConfig3rd>();

        public FaultCheckConfig3rdTable()
        {
            Instance = this;
        }

        public FaultCheckConfig3rd Get(int id)
        {
            _dataDict.TryGetValue(id, out FaultCheckConfig3rd value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(FaultCheckConfig3rd)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, FaultCheckConfig3rd> GetAll()
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
