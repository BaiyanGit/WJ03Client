using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

public static class SensorsComReadManager
{
    public static string[] _coms;

    public static void Init()
    {
        _coms = ReadConfigCom();
        //Debug.Log(_coms.Length);
    }

    public static string[] ReadConfigCom()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath + "/Config/", "ComConfig.txt");

        if (File.Exists(filePath))
        {
            // 读取文件的所有行
            string[] lines = File.ReadAllLines(filePath);
            List<string> validLines = new List<string>();

            // 过滤掉以 # 开头的注释行
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim(); // 去除前后空白字符
                if (!trimmedLine.StartsWith("#") && !string.IsNullOrEmpty(trimmedLine))
                {
                    validLines.Add(trimmedLine);
                }
            }

            // 将有效行合并为一个字符串
            string fileContent = string.Join(Environment.NewLine, validLines);
            Debug.Log(fileContent);

            // 检查是否包含 "|"
            if (fileContent.Contains("|"))
            {
                // 使用 "|" 分割字符串
                string[] splitContent = fileContent.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                return splitContent;
            }
            else
            {
                // 如果没有 "|"，返回整个内容作为数组的唯一元素
                return null;
            }
        }
        else
        {
            Debug.LogError("文件未找到！");
            return null;
        }
    }

    public static List<ConfigItem> ReadConfigCom(string path)
    {
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            List<ConfigItem> configItems = new List<ConfigItem>();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // 跳过注释行和空行
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                // 按第一个 "|" 分割键值对（支持 Value 中包含 "|"）
                string[] parts = trimmedLine.Split('|');

                // 尝试将 Key 解析为 int
                if (!int.TryParse(parts[0].Trim(), out int key))
                {
                    Debug.LogWarning($"无效的 Key（无法解析为整数）: {trimmedLine}");
                    continue; // 跳过无效行
                }

                // 尝试将 Key 解析为 int
                if (!int.TryParse(parts[1].Trim(), out int value))
                {
                    Debug.LogWarning($"无效的 DefaultValued（无法解析为整数）: {trimmedLine}");
                    continue; // 跳过无效行
                }

                // 尝试将 Key 解析为 int
                if (!int.TryParse(parts[2].Trim(), out int defaultValue))
                {
                    Debug.LogWarning($"无效的 DefaultValued（无法解析为整数）: {trimmedLine}");
                    continue; // 跳过无效行
                }

                // 创建配置项
                ConfigItem item = new ConfigItem
                {
                    Key = key,
                    Value = value,
                    DefaultValueID = defaultValue
                };

                configItems.Add(item);
            }

            return configItems;
        }
        else
        {
            Debug.LogError("文件未找到！");
            return null;
        }
    }

    /// <summary>
    /// 临时
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string[] ReadConfigKey(string filePath)
    {
        if (File.Exists(filePath))
        {
            // 读取文件的所有行
            string[] lines = File.ReadAllLines(filePath);
            List<string> validLines = new List<string>();

            // 过滤掉以 # 开头的注释行
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim(); // 去除前后空白字符
                if (!trimmedLine.StartsWith("#") && !string.IsNullOrEmpty(trimmedLine))
                {
                    validLines.Add(trimmedLine);
                }
            }

            // 将有效行合并为一个字符串
            string fileContent = string.Join(Environment.NewLine, validLines);
            //Debug.Log(fileContent);

            // 检查是否包含 "|"
            if (fileContent.Contains("|"))
            {
                // 使用 "|" 分割字符串
                string[] splitContent = fileContent.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                return splitContent;
            }
            else
            {
                // 如果没有 "|"，返回整个内容作为数组的唯一元素
                return null;
            }
        }
        else
        {
            Debug.LogError("文件未找到！");
            return null;
        }
    }
}
