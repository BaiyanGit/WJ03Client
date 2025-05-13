using UnityEditor;
using UnityEngine;

public class GenerateStateEditor : EditorWindow
{
    private string _stateName = "";
    private Object _templateObject = null;
    private Object _folderObject = null;


    [MenuItem("WTools/Generate State")]
    public static void ShowWindow()
    {
        GetWindow<GenerateStateEditor>("Generate State");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("State Name:");
        _stateName = EditorGUILayout.TextField(_stateName);

        EditorGUILayout.LabelField("Template:");
        _templateObject = EditorGUILayout.ObjectField(_templateObject, typeof(TextAsset), false);

        EditorGUILayout.LabelField("Folder:");
        _folderObject = EditorGUILayout.ObjectField(_folderObject, typeof(DefaultAsset), false);

        if (GUILayout.Button("Create Script"))
        {
            CreateScript();
        }
    }

    private void CreateScript()
    {
        if (string.IsNullOrEmpty(_stateName) || _templateObject == null || _folderObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Please enter state name, select a template and select a folder", "OK");
            return;
        }


        string folderPath = AssetDatabase.GetAssetPath(_folderObject);

        if (string.IsNullOrEmpty(folderPath))
        {
            EditorUtility.DisplayDialog("Error", "Please select a valid folder.", "OK");
            return;
        }

        string fullPath = folderPath + "/" + _stateName + ".cs";

        if (System.IO.File.Exists(fullPath))
        {
            if(!EditorUtility.DisplayDialog("Warning", "Already has file. Do you want to coverd", "OK", "CANCEL"))
            {
                return;
            }
            System.IO.File.Delete(fullPath);
        }

        var templatePath = AssetDatabase.GetAssetPath(_templateObject);

        if (string.IsNullOrEmpty(templatePath))
        {
            EditorUtility.DisplayDialog("Error", "Please select a valid template.", "OK");
            return;
        }

        var template = System.IO.File.ReadAllText(templatePath);

        var className = _stateName.Split('_').Length > 1 ? _stateName.Split('_')[1] : _stateName;
        System.IO.File.WriteAllText(fullPath, template.Replace("{0}", className));
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", "Script created successfully.", "OK");
    }
}