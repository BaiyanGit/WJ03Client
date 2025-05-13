using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Wx.Editor.UI
{
    public class UIControlAutoCreate
    {
        public void Create(string uiName,string desc, string templatePath, string targetPath)
        {
            var tempTxt = AssetDatabase.LoadAssetAtPath<TextAsset>(templatePath);
            var tempStr = tempTxt.text;

            var className = "UI" + uiName;
            var modelClassName = "UIModel" + uiName;
            var viewClassName = "UIView" + uiName;

            tempStr = tempStr.Replace("{0}", desc);
            tempStr = tempStr.Replace("{1}", className);
            tempStr = tempStr.Replace("{2}", viewClassName);
            tempStr = tempStr.Replace("{3}", modelClassName);
            tempStr = tempStr.Replace("{4}", viewClassName);
            tempStr = tempStr.Replace("{5}", modelClassName);

            var filePath = targetPath + className + ".cs";

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

        public void SaveFile(string str, string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(str);
                }
            }
            WLog.Log("创建成功: " + filePath);
            AssetDatabase.Refresh();
        }
    }
}