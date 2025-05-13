using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Encryption
{
    /// <summary>
    /// 加密管理器
    /// </summary>
    public class EncryptMgr : MonoBehaviour
    {
        /// <summary>
        /// 配置路劲
        /// </summary>
        private static string _configPath;

        [SerializeField, Header("是否加密")] public bool isEncrypt = false;
        [SerializeField, Header("是否关机")] public bool isShutdown = false;

        /// <summary>
        /// 解密完成回调
        /// </summary>
        public static Action onEncryptionSuccess;

        private void Start()
        {
#if !UNITY_EDITOR
            if (!isEncrypt)
            {
                onEncryptionSuccess?.Invoke();
                return;
            }
            _configPath = $"{Application.streamingAssetsPath}/Release/_info.txt";
            LoadData();
#else
            onEncryptionSuccess?.Invoke();
            //if (!isEncrypt)
            //{
            //    onEncryptionSuccess?.Invoke();
            //    return;
            //}
            //_configPath = $"{Application.streamingAssetsPath}/Release/_info.txt";
            //LoadData();
#endif

        }

        /// <summary>
        /// 加密数据
        /// </summary>
        private void LoadData()
        {
            if (!File.Exists(_configPath))
            {
                AppQuit();
            }
            else
            {
                string data = File.ReadAllText(_configPath);
                HandleData(data);
            }
        }

        /// <summary>
        /// 加载设备信息
        /// </summary>
        private CryptoData LoadDeviceData()
        {
            string deviceName = SystemInfo.deviceName;
            string deviceId = SystemInfo.deviceUniqueIdentifier;
            string operatingSystem = SystemInfo.operatingSystem;
            string graphicsDeviceName = SystemInfo.graphicsDeviceName;
            string graphicsDeviceType = SystemInfo.graphicsDeviceType.ToString();
            string graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
            string processorType = SystemInfo.processorType;
            return new CryptoData
            {
                deviceName = deviceName,
                deviceUniqueID = deviceId,
                operatingSystem = operatingSystem,
                graphicsDeviceName = graphicsDeviceName,
                graphicsDeviceType = graphicsDeviceType,
                graphicsDeviceVersion = graphicsDeviceVersion,
                processorType = processorType
            };
        }

        /// <summary>
        /// 重新加密数据
        /// </summary>
        /// <param name="data"></param>
        private void HandleData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                AppQuit();
                return;
            }
            string encryptedData = CryptoUtility.Decrypt(data);
            CryptoData configData = JsonUtility.FromJson<CryptoData>(encryptedData);
            CryptoData deviceData = LoadDeviceData();

            if (!Equals(configData, deviceData))
            {
                AppQuit();
                return;
            }

            if (CryptoUtility.IsExpired(configData.expirationDate))
            {
                AppQuit();
                return;
            }

            onEncryptionSuccess?.Invoke();
        }

        private void AppQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            return;
#endif
            if (!isShutdown) return;
            Process.Start("shutdown", "/s /t 0");
        }
    }
}