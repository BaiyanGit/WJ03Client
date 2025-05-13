using ExcelDataReader;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Wx.Editor.Excel
{
    public class ExcelExportToClass
    {
        public void Generate(string filePath)
        {
            // try
            {
                using var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WLog.Log(file);
                using var excelData = ExcelReaderFactory.CreateOpenXmlReader(file);
                var dataSet = excelData.AsDataSet();
                var sheet = dataSet.Tables[0];

                var tempName = Path.GetFileNameWithoutExtension(filePath);
                var dataTableClassName = tempName + "Table";

                var data = new ExcelMiddleData();
                data.Init(sheet, filePath);

                var sbProps = new StringBuilder();
                for (int i = 0; i < data.realColumns.Count; i++)
                {
                    var type = data.types[i];
                    var prop = data.props[i];
                    var noteArray = data.notes[i];
                            
                    if (prop == "Id") continue;

                    sbProps.Append("\t/// <summary>\n");
                    for (int j = 0; j < noteArray.Length; j++)
                    {
                        var note = noteArray[j];
                        sbProps.Append("\t/// " + note + "\n");
                    }
                    sbProps.Append("\t/// </summary>\n");
                    sbProps.Append(string.Format("\tpublic {0} {1} {2}\n", type, prop, "{ get; set; }"));
                    sbProps.AppendLine();
                }

                var tempStrFile = AssetDatabase.LoadAssetAtPath<TextAsset>(ExcelConvertPathSetting.ExcelTemplateFilePath).text;
                tempStrFile = tempStrFile.Replace("{0}", tempName);
                tempStrFile = tempStrFile.Replace("{1}", dataTableClassName);
                tempStrFile = tempStrFile.Replace("{2}", sbProps.ToString());

                var csDir = ExcelConvertPathSetting.GetExcelGenerateCSFilePath();
                if (!Directory.Exists(csDir))
                {
                    Directory.CreateDirectory(csDir);
                }
                var targetFilePath = csDir + tempName + ".cs";

                SaveFile(tempStrFile, targetFilePath);
            }
            // catch (Exception e)
            // {
            //     Debug.LogError(e.ToString());
            // }
        }

        private void SaveFile(string str, string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(str);
                }
            }
            WLog.Log("class create: " + filePath);
            AssetDatabase.Refresh();
        }
    }
}