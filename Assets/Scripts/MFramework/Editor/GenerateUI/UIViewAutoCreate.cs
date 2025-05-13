using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Wx.Editor.UI
{
    public class UIViewAutoCreate
    {
        private class ViewPropAndCom
        {
            public string strProp;
            public string strGetCom;
        }

        private UIViewAutoCreateConfig _config;

        private GameObject _uiRootGo;

        private readonly Dictionary<string, ViewPropAndCom> _allPropsDic = new();

        public void Create(string uiName, string desc, GameObject uiRootGo, string templatePath, string targetPath)
        {
            _uiRootGo = uiRootGo;
            _allPropsDic.Clear();

            _config = AssetDatabase.LoadAssetAtPath<UIViewAutoCreateConfig>(UIAutoCreatePathSetting
                .UIViewAutoCreateConfigPath);

            var tempStrFile = AssetDatabase.LoadAssetAtPath<TextAsset>(templatePath).text;

            FindGoChild(uiRootGo.transform, true);
            if (_allPropsDic.Count <= 0)
            {
                WLog.Log("<color=#ff0000>组件数量为0，请确认组件命名是否正确！</color>");
            }

            var strTotalProps = new StringBuilder();
            var strTotalGetComs = new StringBuilder();
            foreach (var prop in _allPropsDic)
            {
                strTotalProps.Append(prop.Value.strProp);
                strTotalGetComs.Append(prop.Value.strGetCom);
            }

            var viewClassName = "UIView" + uiName;

            tempStrFile = tempStrFile.Replace("{0}", desc);
            tempStrFile = tempStrFile.Replace("{1}", viewClassName);
            tempStrFile = tempStrFile.Replace("{2}", strTotalProps.ToString());
            tempStrFile = tempStrFile.Replace("{3}", strTotalGetComs.ToString());
            string filePath = targetPath + viewClassName + ".cs";

            if (File.Exists(filePath))
            {
                if (EditorUtility.DisplayDialog("警告", "检测到脚本，是否覆盖", "确定", "取消"))
                {
                    SaveFile(tempStrFile, filePath);
                }
            }
            else
            {
                SaveFile(tempStrFile, filePath);
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


        private void FindGoChild(Transform ts, bool isRoot)
        {
            if (!isRoot)
            {
                CheckUIView(ts);
                if (ts.name.StartsWith("UICommon")) return;
            }

            for (int i = 0; i < ts.childCount; i++)
            {
                FindGoChild(ts.GetChild(i), false);
            }
        }


        private UIViewAutoCreateInfo FindComponentByConfig(string transName)
        {
            if (transName.StartsWith("UICommon"))
            {
                var info = new UIViewAutoCreateInfo()
                {
                    comName = transName,
                };
                return info;
            }

            for (int i = 0; i < _config.uiInfoList.Count; i++)
            {
                var info = _config.uiInfoList[i];

                if (transName.StartsWith(info.propName)) return info;
            }

            return null;
        }

        private int _childCount;

        private void CheckUIView(Transform child)
        {
            var info = FindComponentByConfig(child.name);

            if (info == null) return;

            //get final name
            string finalPropName;
            if (child.name.StartsWith("UICommon"))
            {
                finalPropName = child.name.Replace("UI", "ui");
            }
            else
            {
                //兼容多个下划线
                var firstIndex = child.name.IndexOf('_');
                try
                {
                    finalPropName = child.name[..firstIndex].ToLower();
                    finalPropName += child.name[(firstIndex + 1)..];
                }
                catch (Exception e)
                {
                    throw new System.Exception("组件命名错误！ " + child.name + "\n" + e.Message);
                }

            }

            string spaceAt = "";
            string spaceCo = "";
            string newLine = "";
            if (_childCount > 0)
            {
                spaceAt = "\t\t";
                spaceCo = "\t\t\t";
                newLine = "\n";
            }

            var strTempProp = $"{spaceAt}public {info.comName} {finalPropName};\n";

            var path = GetPath(child);
            var strTempCom =
                $"{newLine}{spaceCo}{finalPropName} = handle.transform.Find(\"{path}\").GetComponent<{info.comName}>();";

            if (_allPropsDic.TryGetValue(finalPropName, out var view))
            {
                throw new System.Exception("组件重名！ " + path);
            }

            var viewPropAndCom = new ViewPropAndCom()
            {
                strProp = strTempProp,
                strGetCom = strTempCom,
            };
            _allPropsDic.Add(finalPropName, viewPropAndCom);
            _childCount++;
        }

        private string GetPath(Transform transform)
        {
            var path = transform.name;
            while (transform.parent != _uiRootGo.transform)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }
    }
}