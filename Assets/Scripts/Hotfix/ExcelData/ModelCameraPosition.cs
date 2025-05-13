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
    public partial class ModelCameraPosition : ExcelObject, IExcelData
    {
        public int Id { get; set; }

        	/// <summary>
	/// 模型标签入口
	/// </summary>
	public int MLabelEntry { get; set; }

	/// <summary>
	/// 说明
	/// </summary>
	public string Desc { get; set; }

	/// <summary>
	/// 点击UI展示入口
	/// </summary>
	public string MType { get; set; }

	/// <summary>
	/// 相机初始视角位置
	/// </summary>
	public float[] CPosition { get; set; }

	/// <summary>
	/// 相机初始视角角度
	/// </summary>
	public float[] CRotation { get; set; }

	/// <summary>
	/// 相机与目标默认距离
	/// </summary>
	public float DefDistance { get; set; }

	/// <summary>
	/// 相机缩放最大与最小距离
	/// </summary>
	public float[] SWDistance { get; set; }

	/// <summary>
	/// 
	/// </summary>
	public float[] PanOffset { get; set; }


    }

    /// <summary>
    /// 操作
    /// </summary>
    public partial class ModelCameraPositionTable : ExcelTable
    {
        public static ModelCameraPositionTable Instance;
    
        public readonly List<ModelCameraPosition> dataList;
        private Dictionary<int, ModelCameraPosition> _dataDict = new Dictionary<int, ModelCameraPosition>();

        public ModelCameraPositionTable()
        {
            Instance = this;
        }

        public ModelCameraPosition Get(int id)
        {
            _dataDict.TryGetValue(id, out ModelCameraPosition value);
            if (value == null)
            {
                Debug.LogError($"配置找不到，配置表名: {nameof(ModelCameraPosition)}，配置id: {id}");
            }
            return value;
        }
    
        public bool Contain(int id)
        {
            return _dataDict.ContainsKey(id);
        }
    
        public Dictionary<int, ModelCameraPosition> GetAll()
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
