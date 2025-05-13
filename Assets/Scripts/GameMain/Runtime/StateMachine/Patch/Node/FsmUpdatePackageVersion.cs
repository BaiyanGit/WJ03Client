using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.Machine;
using YooAsset;

namespace GameMain.Runtime
{
    /// <summary>
    /// 更新资源版本号
    /// </summary>
    public class FsmUpdatePackageVersion : IStateNode
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
            PatchEventDefine.PatchStatesChange.SendEventMessage("获取最新的资源版本 !");
            UniTask.Void(async () =>
            {
                await UpdatePackageVersion();
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

        private async UniTask UpdatePackageVersion()
        {
            try
            {
                await UniTask.Delay(500);
                
                var package = YooAssets.GetPackage(AppConst.AssetConst.packageName);
                var operation = package.UpdatePackageVersionAsync();
                await operation.ToUniTask(cancellationToken: _cancellationTokenSource.Token);

                if (operation.Status != EOperationStatus.Succeed)
                {
                    Debug.LogWarning(operation.Error);
                    PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
                }
                else
                {
                    AppConst.AssetConst.yooAssetSettings.Version = operation.PackageVersion;
                    _machine.ChangeState<FsmUpdatePackageManifest>();
                }
            }
            catch (OperationCanceledException operationCanceledException) when(_cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Warning("UNITASK CANCEL");
            }
        }
    }
}