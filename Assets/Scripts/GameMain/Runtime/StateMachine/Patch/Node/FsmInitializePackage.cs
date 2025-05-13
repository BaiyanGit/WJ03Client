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
    /// ��ʼ����Դ��
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
            PatchEventDefine.PatchStatesChange.SendEventMessage("��ʼ����Դ����");
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

                // ������Դ������
                var package = YooAssets.TryGetPackage(packageName);
                if (package == null)
                    package = YooAssets.CreatePackage(packageName);
                
                // �༭���µ�ģ��ģʽ
                InitializationOperation initializationOperation = null;
                if (playMode == EPlayMode.EditorSimulateMode)
                {
                    var createParameters = new EditorSimulateModeParameters();
                    createParameters.SimulateManifestFilePath =
                        EditorSimulateModeHelper.SimulateBuild(buildPipeline, packageName);
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                // ��������ģʽ
                if (playMode == EPlayMode.OfflinePlayMode)
                {
                    var createParameters = new OfflinePlayModeParameters();
                    createParameters.DecryptionServices = new FileStreamDecryption();
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                // ��������ģʽ
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

                // WebGL����ģʽ
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

                // �����ʼ��ʧ�ܵ�����ʾ����
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
        /// ��ȡ��Դ��������ַ
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
        /// Զ����Դ��ַ��ѯ������
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
        /// ��Դ�ļ������ؽ�����
        /// </summary>
        private class FileStreamDecryption : IDecryptionServices
        {
            /// <summary>
            /// ͬ����ʽ��ȡ���ܵ���Դ������
            /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                BundleStream bundleStream =
                    new BundleStream(fileInfo.FileLoadPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                managedStream = bundleStream;
                return AssetBundle.LoadFromStream(bundleStream, fileInfo.ConentCRC, GetManagedReadBufferSize());
            }

            /// <summary>
            /// �첽��ʽ��ȡ���ܵ���Դ������
            /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
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
        /// ��Դ�ļ�ƫ�Ƽ��ؽ�����
        /// </summary>
        private class FileOffsetDecryption : IDecryptionServices
        {
            /// <summary>
            /// ͬ����ʽ��ȡ���ܵ���Դ������
            /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
            /// </summary>
            AssetBundle IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo, out Stream managedStream)
            {
                managedStream = null;
                return AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.ConentCRC, GetFileOffset());
            }

            /// <summary>
            /// �첽��ʽ��ȡ���ܵ���Դ������
            /// ע�⣺��������������Դ�������ͷŵ�ʱ����Զ��ͷ�
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
    /// ��Դ�ļ�������
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