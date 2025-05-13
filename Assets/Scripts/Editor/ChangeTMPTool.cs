using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ChangeTMPTool : EditorWindow
{
    private TMP_FontAsset _font;
    private GameObject _currentRoot;
    
    
    [MenuItem("WTools/ChangeTMPTool")]
    public static void ShowWindow()
    {
        var window = GetWindow<ChangeTMPTool>();
        window.titleContent = new GUIContent("更改TMP字体");
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("字体:",GUILayout.Width(100));

        _font = (TMP_FontAsset)EditorGUILayout.ObjectField(_font, typeof(TMP_FontAsset), true, GUILayout.Width(200));
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("当前根物体:",GUILayout.Width(100));
        
        _currentRoot = (GameObject)EditorGUILayout.ObjectField(_currentRoot, typeof(GameObject), true,GUILayout.Width(200));
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);

        if (GUILayout.Button("更改字体"))
        {
            if (_font == null || _currentRoot == null)
            {
                EditorUtility.DisplayDialog("提示", "字体或根物体不能为空", "确定");
                GUILayout.EndVertical();
                return;
            }

            ChangeTMPFont();
            EditorUtility.DisplayDialog("提示", "字体更换成功", "确定");
        }
        
        GUILayout.EndVertical();
    }

    private void ChangeTMPFont()
    {
        var rootInstance = (GameObject)PrefabUtility.InstantiatePrefab(_currentRoot);
        
        var textMeshProUIs = rootInstance.transform.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var textMeshProUI in textMeshProUIs)
        {
            WLog.Log("ChangeTMPFont: " + textMeshProUI.name);
            textMeshProUI.font = _font;
        }
        
        PrefabUtility.SaveAsPrefabAsset(rootInstance, AssetDatabase.GetAssetPath(_currentRoot));

        DestroyImmediate(rootInstance);
    }

}
