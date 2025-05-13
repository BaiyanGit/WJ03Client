using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Wx.Editor
{
    /// <summary>
    /// 选择一个文件之后可以进行生成
    /// </summary>
    public abstract class CreateAssetEditor
    {
        [MenuItem("WTools/生成.Assets文件", false)]
        public static void OnCreateAsset()
        {
            foreach (var guid in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var pathSplit = path.Split('/');
                var className = pathSplit[^1].Replace(".cs", "");
                var classType = GetClassByClassName(className);
                var assetPath = path.Replace(".cs", ".asset");
                if (File.Exists(assetPath))
                {
                    WLog.Error("创建失败，资源已经存在");
                }
                else if (classType != null)
                {
                    var classInst = ScriptableObject.CreateInstance(classType);
                    AssetDatabase.CreateAsset(classInst, assetPath);
                }
                else
                {
                    WLog.Error("创建失败，脚本不可创建");
                }
            }
        }

        [MenuItem("WTools/生成.Assets文件", true)]
        private static bool IsValidateFun()
        {
            return (from guid in Selection.assetGUIDs
                select AssetDatabase.GUIDToAssetPath(guid)
                into path
                select path.Split('/')
                into pathSplit
                select pathSplit[^1]).Any(className => className.Contains(".cs"));
        }

        /// <summary>
        /// 通过类名获取对应的类
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private static System.Type GetClassByClassName(string className)
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return allAssemblies.SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type =>
                type.Name == className && type.IsSubclassOf(typeof(ScriptableObject)));
        }
    }
}