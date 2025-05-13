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
    public partial class PrincipleConfig : ExcelObject, IExcelData
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
	/// 视频路径
	/// </summary>
	public string VideoPath { get; set; }

	/// <summary>
	/// 模型路径（包含模型名称，需要与视频路径数量进行统一）
	/// </summary>
	public string ModelPath { get; set; }

	/// <summary>
	/// 模型动画（一样）
	/// </summary>
	public string ModelAnim { get; set; }

	/// <summary>
	/// 相机位置
	/// </summary>
	public float[] CameraSetPos { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class PrincipleConfigTable : ExcelTable
    {
        public static PrincipleConfigTable Instance;
    
        public readonly List<PrincipleConfig> dataList;
        private Dictionary<int, PrincipleConfig> _dataDict = new Dictionary<int, PrincipleConfig>();

        public PrincipleConfigTable()
        {
            Instance = this;
        }

        public PrincipleConfig Get(int id)
        {
            _dataDict.TryGetValue(id, out PrincipleConfig value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(PrincipleConfig)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, PrincipleConfig> GetAll()
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
