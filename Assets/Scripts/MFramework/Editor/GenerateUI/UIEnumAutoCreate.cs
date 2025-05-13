using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Wx.Editor.UI
{
    public class UIEnumAutoCreate
    {
        public static void Create(string uiName)
        {
            var tempPath = UIAutoCreatePathSetting.TemplateFilePath + UIAutoCreatePathSetting.EnumTemplateName;
            var targetFile = UIAutoCreatePathSetting.GenerateEnumFilePath;
            
            var tempText = File.ReadAllText(tempPath);
            string fileContent;
                
            if (!File.Exists(targetFile))
            {
                File.WriteAllText(targetFile, tempText);
                fileContent = tempText;
            }
            else
            {
                fileContent = File.ReadAllText(targetFile);
            }
            
            var lastEnumValue = ExtractLastEnumValue(fileContent);

            Debug.Log("最后一个枚举值为: " + lastEnumValue);

            // 在此处添加新的枚举值，例如：
            var newEnumValue = lastEnumValue + 1;

            var updatedContent = UpdateEnumFile(fileContent, uiName, newEnumValue);
            File.WriteAllText(targetFile, updatedContent);

            Debug.Log($"已将新枚举 {uiName} ({newEnumValue}) 添加到文件 {targetFile}");
            AssetDatabase.Refresh(); 
        }
        
        private static int ExtractLastEnumValue(string fileContent)
        {
            var pattern = @"(?<=namespace\s+Hotfix\s*\{\s*public\s+enum\s+EnumUIForm\s*\{\s*)([^\{\}])*?(?=\}\s*\}\s*$)";
            var enumContent = Regex.Match(fileContent, pattern).Value;
            if (string.IsNullOrEmpty(enumContent.Trim()))
            {
                return 1000;
            }
        
            var enumEntries = enumContent.Split(',');
            var lastEnumEntry = enumEntries.Length > 1 ? enumEntries[^2].Trim() : enumEntries.Last().Trim();

            var enumNameAndValue = lastEnumEntry.Split('=');
        
            if (int.TryParse(enumNameAndValue[1].Trim(), out var enumValue))
            {
                return enumValue;
            }

            Debug.LogError("无法从脚本文件中提取枚举值");
            return -1;
        }
    
        private static string UpdateEnumFile(string fileContent, string newEnumName, int newEnumValue)
        {
            var pattern = @"(?<=namespace\s+Hotfix\s*\{\s*public\s+enum\s+EnumUIForm\s*\{\s*)([^\{\}])*?(?=\}\s*\}\s*$)";
            var enumContent = Regex.Match(fileContent, pattern).Value;

            var enumEntries = enumContent.Split(',');

            var newEnumEntry = $"\n\t\t{newEnumName} = {newEnumValue},\n\t";
        
            enumEntries = enumEntries.Take(enumEntries.Length - 1).Concat(new[] { newEnumEntry }).ToArray();

            var updatedEnumContent = string.Join(",", enumEntries.Select(entry => "\t" + entry));

            var updatedContent = Regex.Replace(fileContent, pattern,updatedEnumContent);

            return updatedContent;
        }
    }
    

}