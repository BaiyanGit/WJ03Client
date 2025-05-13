using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Wx.Editor.UI
{
    public class UIScriptAutoCreateEditorWindow : EditorWindow
    {
        private string _newUIName;
        private string _descUI;

        private GameObject _uiRootGo;
        private UIViewAutoCreateConfig _viewConfig;

        [MenuItem("WTools/UI自动生成器 #&%U", false, 999)]
        private static void ShowEditor()
        {
            var window = GetWindow<UIScriptAutoCreateEditorWindow>();
            window.titleContent.text = "UI自动生成器";
            
        }

        private void OnEnable()
        {
            _viewConfig = AssetDatabase.LoadAssetAtPath<UIViewAutoCreateConfig>(UIAutoCreatePathSetting.UIViewAutoCreateConfigPath);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("UI 生成工具", TitleStyle());
            GUILayout.EndHorizontal();

            //============================自动生成预制体设置==============================
            GUILayout.Space(20);
            EditorGUILayout.LabelField("1、预制体自动生成设置", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("新UI名字:", GUILayout.Width(70));
            _newUIName = EditorGUILayout.TextField(_newUIName, GUILayout.Width(400));
            if (GUILayout.Button("Create", GUILayout.Width(70)))
            {
                CreateUIPrefab();
            }

            GUILayout.EndHorizontal();

            //=======================================================================

            //============================自动生成代码设置==============================
            GUILayout.Space(20);
            EditorGUILayout.LabelField("2、自动生成代码设置", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("说明：");
            _descUI = GUILayout.TextField(_descUI, GUILayout.Width(600));

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("View根节点:", GUILayout.Width(70));
            _uiRootGo = (GameObject)EditorGUILayout.ObjectField(_uiRootGo, typeof(GameObject), true,
                GUILayout.Width(400));
            if (GUILayout.Button("View代码", GUILayout.Width(70)))
            {
                CreateUIView();
            }

            if (GUILayout.Button("MVC代码", GUILayout.Width(70)))
            {
                CreateMvc();
            }
            
            if(GUILayout.Button("更新枚举代码",GUILayout.Width(140)))
            {
                UpdateUIEnum();
            }

            GUILayout.EndHorizontal();
            
            // 显示 _viewConfig 信息
            GUILayout.Space(20);
            EditorGUILayout.LabelField("配置信息", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (_viewConfig != null)
            {
                for (var i = 0; i < _viewConfig.uiInfoList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("属性名:", GUILayout.Width(70));
                    _viewConfig.uiInfoList[i].propName = EditorGUILayout.TextField(_viewConfig.uiInfoList[i].propName, GUILayout.Width(200));
                    GUILayout.Space(50);
                    EditorGUILayout.LabelField("组件名:", GUILayout.Width(70));
                    _viewConfig.uiInfoList[i].comName = EditorGUILayout.TextField(_viewConfig.uiInfoList[i].comName, GUILayout.Width(200));
                    if (GUILayout.Button("删除", GUILayout.Width(70)))
                    {
                        _viewConfig.uiInfoList.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    GUILayout.EndHorizontal();
                }

                if (GUILayout.Button("添加新项", GUILayout.Width(100)))
                {
                    _viewConfig.uiInfoList.Add(new UIViewAutoCreateInfo());
                }
            }
            else
            {
                EditorGUILayout.LabelField("配置未加载", EditorStyles.boldLabel);
            }


            GUILayout.EndVertical();
        }

        private void CreateUIPrefab()
        {
            if (string.IsNullOrEmpty(_newUIName)) throw new System.Exception("请输入UI名字");

            const string strPrefab = ".prefab";
            const string strMeta = ".meta";

            var newPrefabEditorPath = UIAutoCreatePathSetting.PrefabCreatePath + _newUIName + strPrefab;
            var newPrefabResourcesPath = Application.dataPath + UIAutoCreatePathSetting.PrefabCreatePath;
            newPrefabResourcesPath = newPrefabResourcesPath.Replace("/AssetsAssets", "/Assets");
            WLog.Log(newPrefabResourcesPath);
            //copy prefab
            //origin
            var originPrefabFullPath = Application.dataPath + UIAutoCreatePathSetting.PrefabTemplatePath + strPrefab;
            originPrefabFullPath = originPrefabFullPath.Replace("/AssetsAssets", "/Assets");
            WLog.Log(originPrefabFullPath);
            //target
            var newPrefabFullPath = newPrefabResourcesPath + _newUIName + strPrefab;

            //copy meta
            var originMetaFullPath = originPrefabFullPath + strMeta;
            var newMetaFullPath = newPrefabFullPath + strMeta;

            var result = false;
            if (File.Exists(newPrefabFullPath))
            {
                if (EditorUtility.DisplayDialog("警告", "检测到UI预制体，是否覆盖", "确定", "取消"))
                {
                    File.Copy(originPrefabFullPath, newPrefabFullPath, true);
                    File.Copy(originMetaFullPath, newMetaFullPath, true);
                    result = true;
                }
            }
            else
            {
                CheckTargetPath(newPrefabResourcesPath);
                File.Copy(originPrefabFullPath, newPrefabFullPath);
                File.Copy(originMetaFullPath, newMetaFullPath);
                result = true;
            }

            if (result)
            {
                WLog.Log("UI创建成功: " + newPrefabFullPath);
                AssetDatabase.Refresh();

                _uiRootGo = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabEditorPath);
            }
        }

        private void CreateMvc()
        {
            CreateUIView();
            CreateUIModel();
            CreateUIControl();
        }

        private void CreateUIView()
        {
            if (_uiRootGo == null) throw new System.Exception("请拖入需要生成的UI预制体");

            var uiName = GetUIName();
            var tempPath = UIAutoCreatePathSetting.TemplateFilePath + UIAutoCreatePathSetting.ViewTemplateName;
            var targetPath = GetTargetGeneratePath(uiName);
            CheckTargetPath(targetPath);
            new UIViewAutoCreate().Create(uiName, _descUI, _uiRootGo, tempPath, targetPath);
        }

        private void CreateUIControl()
        {
            if (_uiRootGo == null) throw new System.Exception("请拖入需要生成的UI预制体");

            var uiName = GetUIName();
            var tempPath = UIAutoCreatePathSetting.TemplateFilePath + UIAutoCreatePathSetting.ControlTemplateName;
            var targetPath = GetTargetGeneratePath(uiName);
            CheckTargetPath(targetPath);
            new UIControlAutoCreate().Create(uiName, _descUI, tempPath, targetPath);
        }

        private void CreateUIModel()
        {
            if (_uiRootGo == null) throw new System.Exception("请拖入需要生成的UI预制体");

            var uiName = GetUIName();
            var tempPath = UIAutoCreatePathSetting.TemplateFilePath + UIAutoCreatePathSetting.ModelTemplateName;
            var targetPath = GetTargetGeneratePath(uiName);
            CheckTargetPath(targetPath);
            UIModelAutoCreate.Create(uiName, _descUI, tempPath, targetPath);
        }

        private void UpdateUIEnum()
        {
            if (_uiRootGo == null) throw new System.Exception("请拖入需要生成的UI预制体");
            var uiName = _uiRootGo.name;
            UIEnumAutoCreate.Create(uiName);
        }

        private void ShowConfig()
        {
            
        }

        private string GetTargetGeneratePath(string uiName)
        {
            return UIAutoCreatePathSetting.GenerateCsFilePath + "UI" + uiName + "/";
        }

        private void CheckTargetPath(string targetPath)
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
        }

        private string GetUIName()
        {
            var uiName = _uiRootGo.name.Replace("UI", "");
            return uiName;
        }

        /// <summary>
        /// 标题文字的样式
        /// </summary>
        /// <returns></returns>
        private static GUIStyle TitleStyle()
        {
            var labelStyle = new GUIStyle
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            return labelStyle;
        }
    }
}