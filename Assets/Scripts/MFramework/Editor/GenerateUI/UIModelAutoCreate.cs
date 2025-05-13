using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Wx.Editor.UI
{
    public abstract class UIModelAutoCreate
    {
        public static void Create(string uiName,string desc, string templatePath, string targetPath)
        {
            var tempTxt = AssetDatabase.LoadAssetAtPath<TextAsset>(templatePath);
            var tempStr = tempTxt.text;
            
            var modelClassName = "UIModel" + uiName;

            tempStr = tempStr.Replace("{0}", desc);
            tempStr = tempStr.Replace("{1}", modelClassName);

            var filePath = targetPath + modelClassName + ".cs";

            if (File.Exists(filePath))
            {
                if (EditorUtility.DisplayDialog("警告", "检测到脚本，是否覆盖", "确定", "取消"))
                {
                    SaveFile(tempStr, filePath);
                }
            }
            else
            {
                SaveFile(tempStr, filePath);
            }
        }

        private static void SaveFile(string str, string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(str);
                }
            }

            WLog.Log("创建成功: " + filePath);
            AssetDatabase.Refresh();
        }
    }
}