using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Wx.Runtime.Machine;
using YooAsset;

namespace GameMain.Runtime
{
    /// <summary>
    /// 初始化资源包
    /// </summary>
    public class FsmInitializePackage : IStateNode
    {
        private WMachine _machine;
        private CancellationTokenSource _cancellationTokenSource = new();
        void IStateNode.OnCreate(WMachine machine)
        {
            _machine = machine;
        }

        void IStateNode.OnEnter(params object[] datas)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            PatchEventDefine.PatchStatesChange.SendEventMessage("初始化资源包！");
            UniTask.Void(async () =>
            {
                await InitPackage();
            });
        }

        void IStateNode.OnUpdate()
        {
        }

        void IStateNode.OnFixedUpdate()
        {
        }

        void IStateNode.OnLateUpdate()
        {
        }

        void IStateNode.OnExit()
        {
            _cancellationTokenSource.Cancel();
        }

        void IStateNode.OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }

        private async UniTask InitPackage()
        {
            try
            {
                var playMode = AppConst.AssetConst.ePlayMode;
                var packageName = AppConst.AssetConst.packageName;
                var buildPipeline = AppConst.AssetConst.buildPipeline;

                // 创建资源包裹类
                var package = YooAssets.TryGetPackage(packageName);
                if (package == null)
                    package = YooAssets.CreatePackage(packageName);
                
                // 编辑器下的模拟模式
                InitializationOperation initializationOperation = null;
                if (playMode == EPlayMode.EditorSimulateMode)
                {
                    var createParameters = new EditorSimulateModeParameters();
                    createParameters.SimulateManifestFilePath =
                        EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                // 单机运行模式
                if (playMode == EPlayMode.OfflinePlayMode)
                {
                    var createParameters = new OfflinePlayModeParameters();
                    createParameters.DecryptionServices = new FileStreamDecryption();
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                // 联机运行模式
                if (playMode == EPlayMode.HostPlayMode)
                {
                    string defaultHostServer = GetHostServerURL();
                    string fallbackHostServer = GetHostServerURL();
                    var createParameters = new HostPlayModeParameters();
                    createParameters.DecryptionServices = new FileStreamDecryption();
                    createParameters.BuildinQueryServices = new GameQueryServices();
                    createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                // WebGL运行模式
                if (playMode == EPlayMode.WebPlayMode)
                {
                    string defaultHostServer = GetHostServerURL();
                    string fallbackHostServer = GetHostServerURL();
                    var createParameters = new WebPlayModeParameters();
                    createParameters.DecryptionServices = new FileStreamDecryption();
                    createParameters.BuildinQueryServices = new GameQueryServices();
                    createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                await initializationOperation.ToUniTask(cancellationToken: _cancellationTokenSource.Token);

                // 如果初始化失败弹出提示界面
                if (initializationOperation.Status != EOperationStatus.Succeed)
                {
                    WLog.Warning($"{initializationOperation.Error}");
                    PatchEventDefine.InitializeFailed.SendEventMessage();
                }
                else
                {
                    var version = initializationOperation.PackageVersion;
                    WLog.Log($"Init resource package version : {version}");
                    YooAssets.SetDefaultPackage(package);
                    _machine.ChangeState<FsmUpdatePackageVersion>();
                }
            }
            catch (OperationCanceledException operationCanceledException) when(_cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Warning("UNITASK CANCEL");
            }
        }

        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        private string GetHostServerURL()
        {
            var hostServerIP = AppConst.AssetConst.assetsSever;
            var appVersion = AppConst.AssetConst.severVersion;

#if UNITY_EDITOR
            return EditorUserBuildSettings.activeBuildTarget switch
            {
                BuildTarget.Android => $"{hostServerIP}/CDN/Android/{appVersion}",
                BuildTarget.iOS => $"{hostServerIP}/CDN/IPhone/{appVersion}",
                BuildTarget.WebGL => $"{hostServerIP}/CDN/WebGL/{appVersion}",
                _ => $"{hostServerIP}/CDN/PC/{appVersion}"
            };
#else
            return Application.platform switch
            {
                RuntimePlatform.Android => $"{hostServerIP}/CDN/Android/{appVersion}",
                RuntimePlatform.IPhonePlayer => $"{hostServerIP}/CDN/IPhone/{appVersion}",
                RuntimePlatform.WebGLPlayer => $"{hostServerIP}/CDN/WebGL/{appVersion}",
                _ => $"{hostServerIP}/CDN/PC/{appVersion}"
            };
#endif
        }

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }

            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }

            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }

        /// <summary>
        /// 资源文件流加载解密类
        /// </summary>
        private class FileStreamDecryption : IDecryptionServices
        {
            /// <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream =
                    new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo,
                out Stream managedStream)
            {
                BundleStream bundleStream =
                    new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStreamAsync(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
            }

            private static uint GetManagedReadBufferSize()
            {
                return 1024;
            }
        }

        /// <summary>
        /// 资源文件偏移加载解密类
        /// </summary>
        private class FileOffsetDecryption : IDecryptionServices
        {
            /// <summary>
            /// 同步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
            }

            /// <summary>
            /// 异步方式获取解密的资源包对象
            /// 注意：加载流对象在资源包对象释放的时候会自动释放
            /// </summary>
            AssetBundleCreateRequest IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo,
                out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
            }

            private static ulong GetFileOffset()
            {
                return 32;
            }
        }
    }

    /// <summary>
    /// 资源文件解密流
    /// </summary>
    public class BundleStream : FileStream
    {
        public const byte KEY = 64;

        public BundleStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access,
            share)
        {
        }

        public BundleStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] ^= KEY;
            }

            return index;
        }
    }
}