using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wx.Runtime.Machine;
using UnityEngine;
using YooAsset;

namespace GameMain.Runtime
{
    public class FsmUpdatePackageManifest : IStateNode
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
            PatchEventDefine.PatchStatesChange.SendEventMessage("更新资源清单！");
            UniTask.Void(async () =>
            {
                await UpdateManifest();
            });
        }

        void IStateNode.OnExit()
        {
            _cancellationTokenSource.Cancel();
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

        void IStateNode.OnDestroy()
        {
            _cancellationTokenSource.Cancel();
        }
        
        private async UniTask UpdateManifest()
        {
            try
            {
                await UniTask.Delay(500, cancellationToken: _cancellationTokenSource.Token);
                
                var package = YooAssets.GetPackage(AppConst.AssetConst.packageName);
                var operation = package.UpdatePackageManifestAsync(AppConst.AssetConst.yooAssetSettings.Version);
                await operation.ToUniTask(cancellationToken: _cancellationTokenSource.Token);

                if (operation.Status != EOperationStatus.Succeed)
                {
                    Debug.LogWarning(operation.Error);
                    PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
                    await UniTask.Yield(cancellationToken: _cancellationTokenSource.Token);
                }
                else
                {
                    _machine.ChangeState<FsmCreatePackageDownloader>();
                }
            }
            catch (OperationCanceledException operationCanceledException) when(_cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Warning("UNITASK CANCEL");
            }
        }
    }
}