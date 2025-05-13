using System.Net.Sockets;
using System.Net;
using System;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using Wx.Runtime.Singleton;
using Wx.Runtime.Http;
using YooAsset;

namespace GameMain.Runtime
{
    public class ConfigManager : SingletonInstance<ConfigManager>, ISingleton
    {
        public void OnCreate(object createParam)
        {
        }
        
        public void OnUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
        }

        public void OnDestroy()
        {
            
        }

        public void InitClientConfig()
        {
            var settings = Resources.Load<YooAssetSettings>("YooAssetSettings");
            if (settings == null)
            {
                throw new Exception($"Can not Found YooAssetsSettings");
            }

            AppConst.AssetConst.yooAssetSettings = settings;
        }

        public async UniTask<int> InitYooAssetConfig()
        {
            var url = string.Empty;
            
#if UNITY_EDITOR
            url = EditorUserBuildSettings.activeBuildTarget switch
            {
                BuildTarget.Android => $"{AppConst.AssetConst.assetsSever}/CDN/Android/version.txt",
                BuildTarget.iOS => $"{AppConst.AssetConst.assetsSever}/CDN/IPhone/version.txt",
                BuildTarget.WebGL => $"{AppConst.AssetConst.assetsSever}/CDN/WebGL/version.txt",
                _ => $"{AppConst.AssetConst.assetsSever}/CDN/PC/version.txt"
            };
#else
            url =  Application.platform switch
            {
                RuntimePlatform.Android => $"{AppConst.AssetConst.assetsSever}/CDN/Android/version.txt",
                RuntimePlatform.IPhonePlayer => $"{AppConst.AssetConst.assetsSever}/CDN/IPhone/version.txt",
                RuntimePlatform.WebGLPlayer => $"{AppConst.AssetConst.assetsSever}/CDN/WebGL/version.txt",
                _ => $"{AppConst.AssetConst.assetsSever}/CDN/PC/version.txt"
            };
#endif
            var packageVersionResponse = await NetworkHelper.GetJsonAsync<PackageVersionResponse>(url, 30);
            if (packageVersionResponse.code == -1)
            {
                return -1;
            }
            else
            {
                var version = packageVersionResponse.version;
                AppConst.AssetConst.severVersion = version;
                return CompareVersion();
            }
        }
        
        private int CompareVersion()
        {
            var clientVersionSplit = AppConst.AssetConst.yooAssetSettings.Version.Split('.');
            var severVersionSplit = AppConst.AssetConst.severVersion.Split('.');
            for (var i = 0; i < 3; i++)
            {
                if (clientVersionSplit[i] != severVersionSplit[i])
                {
                    //大版本更新 重新下载客户端
                    return i == 0 ? 1 : 2;
                }
            }
            return 3;
        }
    }
}
