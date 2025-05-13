using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Wx.Editor
{
    public partial class GenerateFolder
    {
        private static readonly string[] resFolderName =
        {
            "Audio",
            "Materials",
            "Prefabs",
            "Shaders",
            "Textures"
        };

        private static readonly string[] unityFolderName =
        {
            "Scenes",
            "Editor",
            "Plugins",
            "StreamingAssets"
        };

        private static readonly string[] assetsFolderName =
        {
            "UIs",
            "Fonts",
            "Audios",
            "Videos",
            "Models",
            "Shaders",
            "Sprites",
            "Textures",
            "Materials",
            "Animators",
            "Animations",
        };
    }

    public partial class GenerateFolder : MonoBehaviour
    {
        [MenuItem("WTools/生成文件夹/Assets Folder")]
        private static void GenerateAssetsFolder()
        {
            var dataPath = Application.dataPath + "/Res/";
            if (Directory.Exists(dataPath) == false)
            {
                Directory.CreateDirectory(dataPath);
            }

            CreatFolder(dataPath, assetsFolderName);
        }

        [MenuItem("WTools/生成文件夹/Unity Folder")]
        private static void GenerateUnityFolder()
        {
            var dataPath = Application.dataPath + "/";
            CreatFolder(dataPath, unityFolderName);
        }

        [MenuItem("WTools/生成文件夹/Resources Folder")]
        private static void GenerateResFolder()
        {
            var dataPath = $"{Application.dataPath}/Resources/";
            if (Directory.Exists(dataPath) == false)
            {
                Directory.CreateDirectory(dataPath);
            }

            CreatFolder(dataPath, resFolderName);
        }

        private static void CreatFolder(string dataPath, IEnumerable<string> foldersName)
        {
            foreach (var folderName in foldersName)
            {
                var folder = dataPath + folderName;
                if (Directory.Exists(folder) == false)
                {
                    Directory.CreateDirectory(folder);
                    WLog.Log($"<color=yellow>新建文件夹：</color>{folder}");
                }
                else
                {
                    WLog.Log($"<color=red>文件夹存在：</color>{folder}");
                }
            }

            AssetDatabase.Refresh();
        }
    }
}