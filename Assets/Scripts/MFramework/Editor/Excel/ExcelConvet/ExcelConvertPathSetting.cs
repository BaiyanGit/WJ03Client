using System.IO;
using UnityEngine;

namespace Wx.Editor.Excel
{
    public enum ExcelConvertPathType : int
    {
        StreamingAssets = 0,
        Resources = 1,
        GameRes = 2,
        Both = 3
    }

    public static class ExcelConvertPathSetting
    {
  
        /// <summary>
        /// 模板路径
        /// </summary>
        public const string ExcelTemplateFilePath = "Assets/Scripts/MFramework/Editor/Excel/ExcelConvet/Template/ExcelDataClassTemplate.txt";

        /// <summary>
        /// 代码生成路径
        /// </summary>
        public const string GenerateCSFilePath = "Assets/Scripts/Hotfix/ExcelData/";

        /// <summary>
        /// asset数据在Resources下的生成路径
        /// </summary>
        public const string ASSET_OUTPUT_ResourcesPATH = "Assets/Resources/ExcelData/";

        /// <summary>
        /// asset数据生成在Streaming Assets下的路径
        /// </summary>
        public const string ASSET_OUTPUT_StreamingPATH = "Assets/StreamingAssets/ExcelData/";
        
        /// <summary>
        /// asset数据生成在GameRes下的路径
        /// </summary>
        public const string ASSET_OUTPUT_GameResPATH = "Assets/GameRes/Json/";

        /// <summary>
        /// 属性名行
        /// </summary>
        public const int EXCEL_ROW_INDEX_Prop = 0;

        /// <summary>
        /// 注释行
        /// </summary>
        public const int EXCEL_ROW_INDEX_Note = 1;

        /// <summary>
        /// 类型行
        /// </summary>
        public const int EXCEL_ROW_INDEX_Type = 2;

        /// <summary>
        /// 内容行
        /// </summary>
        public const int EXCEL_ROW_INDEX_Content_Start = 3;

        /// <summary>
        /// Excel表格路径
        /// </summary>
        /// <returns></returns>
        public static string GetExcelPath()
        {
            // path: ../../design/config/
            string excelPath = Directory.CreateDirectory(Application.dataPath).Parent.FullName + "\\Excel\\";

            return excelPath;
        }

        /// <summary>
        /// Excel代码完整生成路径
        /// </summary>
        /// <returns></returns>
        public static string GetExcelGenerateCSFilePath()
        {
            string hotfixPath = Application.dataPath + GenerateCSFilePath;
            hotfixPath = hotfixPath.Replace("/AssetsAssets", "/Assets");
            return hotfixPath;
        }

        /// <summary>
        /// asset数据完整生成路径
        /// </summary>
        /// <returns></returns>
        public static string GetExcelGenerateAssetFilePath(ExcelConvertPathType pathType)
        {
            string assetGeneratePath = pathType switch
            {
                ExcelConvertPathType.Resources => Application.dataPath + ASSET_OUTPUT_ResourcesPATH,
                ExcelConvertPathType.StreamingAssets => Application.dataPath + ASSET_OUTPUT_StreamingPATH,
                ExcelConvertPathType.GameRes => Application.dataPath + ASSET_OUTPUT_GameResPATH,
                _ => ""
            };

            assetGeneratePath = assetGeneratePath.Replace("/AssetsAssets", "/Assets");
            return assetGeneratePath;
        }
    }
}