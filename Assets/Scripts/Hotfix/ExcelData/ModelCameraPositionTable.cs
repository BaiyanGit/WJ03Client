using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;
using Wx.Runtime.Excel;

namespace Hotfix.ExcelData
{
    using UnityEngine;

    public partial class ModelCameraPositionTable
    {
        public static readonly string ConfigJsonPath = $"{Application.streamingAssetsPath}/ExcelData/ModelCameraPosition.json";

        public bool isChanged = false;
        
        /// <summary>
        /// 根据标签和类型获取摄像机位置数据
        /// </summary>
        /// <param name="mLabelEntry">标签类型0:结构认知,1:原理展示</param>
        /// <param name="mType">元素内容(点击UI按钮的txt内容)</param>
        /// <returns></returns>
        public ModelCameraPosition GetCameraPositionByLabelAndType(int mLabelEntry, string mType)
        {
            var datas = dataList.FindAll(t => t.MLabelEntry == mLabelEntry);
            var data = datas.Find(t => t.MType == mType);
            if (data == null)
            {
                Debug.LogError($"没有找到对应的摄像机位置数据,msg: label={mLabelEntry} type={mType}");
                return null;
            }

            return data;
        }

        public void SaveData()
        {
            if (isChanged == false) return;
            var wrapper = new ExcelDataListWrapper<ModelCameraPosition> { dataList = dataList };

           // string jsonData = Regex.Unescape(JsonMapper.ToJson(dataList));
            File.WriteAllText(ConfigJsonPath, JsonMapper.ToJson(wrapper), Encoding.UTF8);
            isChanged = false;
        }
    }
}