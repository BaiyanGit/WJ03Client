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
            // ��ȡ�ļ���������
            string[] lines = File.ReadAllLines(filePath);
            List<string> validLines = new List<string>();

            // ���˵��� # ��ͷ��ע����
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim(); // ȥ��ǰ��հ��ַ�
                if (!trimmedLine.StartsWith("#") && !string.IsNullOrEmpty(trimmedLine))
                {
                    validLines.Add(trimmedLine);
                }
            }

            // ����Ч�кϲ�Ϊһ���ַ���
            string fileContent = string.Join(Environment.NewLine, validLines);
            Debug.Log(fileContent);

            // ����Ƿ���� "|"
            if (fileContent.Contains("|"))
            {
                // ʹ�� "|" �ָ��ַ���
                string[] splitContent = fileContent.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                return splitContent;
            }
            else
            {
                // ���û�� "|"����������������Ϊ�����ΨһԪ��
                return null;
            }
        }
        else
        {
            Debug.LogError("�ļ�δ�ҵ���");
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

                // ����ע���кͿ���
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                // ����һ�� "|" �ָ��ֵ�ԣ�֧�� Value �а��� "|"��
                string[] parts = trimmedLine.Split('|');

                // ���Խ� Key ����Ϊ int
                if (!int.TryParse(parts[0].Trim(), out int key))
                {
                    Debug.LogWarning($"��Ч�� Key���޷�����Ϊ������: {trimmedLine}");
                    continue; // ������Ч��
                }

                // ���Խ� Key ����Ϊ int
                if (!int.TryParse(parts[1].Trim(), out int value))
                {
                    Debug.LogWarning($"��Ч�� DefaultValued���޷�����Ϊ������: {trimmedLine}");
                    continue; // ������Ч��
                }

                // ���Խ� Key ����Ϊ int
                if (!int.TryParse(parts[2].Trim(), out int defaultValue))
                {
                    Debug.LogWarning($"��Ч�� DefaultValued���޷�����Ϊ������: {trimmedLine}");
                    continue; // ������Ч��
                }

                // ����������
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
            Debug.LogError("�ļ�δ�ҵ���");
            return null;
        }
    }

    /// <summary>
    /// ��ʱ
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string[] ReadConfigKey(string filePath)
    {
        if (File.Exists(filePath))
        {
            // ��ȡ�ļ���������
            string[] lines = File.ReadAllLines(filePath);
            List<string> validLines = new List<string>();

            // ���˵��� # ��ͷ��ע����
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim(); // ȥ��ǰ��հ��ַ�
                if (!trimmedLine.StartsWith("#") && !string.IsNullOrEmpty(trimmedLine))
                {
                    validLines.Add(trimmedLine);
                }
            }

            // ����Ч�кϲ�Ϊһ���ַ���
            string fileContent = string.Join(Environment.NewLine, validLines);
            //Debug.Log(fileContent);

            // ����Ƿ���� "|"
            if (fileContent.Contains("|"))
            {
                // ʹ�� "|" �ָ��ַ���
                string[] splitContent = fileContent.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                return splitContent;
            }
            else
            {
                // ���û�� "|"����������������Ϊ�����ΨһԪ��
                return null;
            }
        }
        else
        {
            Debug.LogError("�ļ�δ�ҵ���");
            return null;
        }
    }
}
