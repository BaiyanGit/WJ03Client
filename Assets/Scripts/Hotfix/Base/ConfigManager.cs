using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitJson;
using UnityEngine;
using Wx.Runtime.Singleton;

namespace Hotfix
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

        public async UniTask InitGlobalConfig()
        {
            string path;

#if !UNITY_WEBGL
            path = Application.streamingAssetsPath + "/Config/peizhi.txt";
#endif

            if (File.Exists(@path))
            {
                var peizhi = File.ReadAllText(@path);
                string[] info = peizhi.Split('|');
                
                AppConst.Constant.BigScreenLayoutIndexMax = Convert.ToInt32(info[0]);
            }
        }

        public async UniTask InitNetConfig(CancellationTokenSource cancellationTokenSource)
        {
#if !UNITY_WEBGL
            AppConst.UrlConst.selfIP = GetLocalIPAddress();
#endif
            var severConfig = await GetSeverAddress(cancellationTokenSource);
            AppConst.UrlConst.severIP = severConfig.severIp;
            AppConst.UrlConst.severPort = severConfig.severPort;
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("没有找到IPv4地址");
        }

        public async UniTask<SeverConfigData> GetSeverAddress(CancellationTokenSource cancellationTokenSource, float timeout = 10f)
        {
            string severConfigText;

#if UNITY_STANDALONE_LINUX
            severConfigText = GameEntry.Resource.IOFileResource.ReadAllText(AppConst.ConfigPathConst.SeverConfigPath);
#else
            severConfigText = await GameEntry.Resource.WebResource.LoadTextAsync(AppConst.ConfigPathConst.SeverConfigPath, cancellationTokenSource, timeout);
#endif
            if (string.IsNullOrEmpty(severConfigText))
            {
                throw new Exception($"Can not get severConfig : {AppConst.ConfigPathConst.SeverConfigPath}");
            }

            var severConfig = JsonMapper.ToObject<SeverConfigData>(severConfigText);
            return severConfig;
        }
    }
}