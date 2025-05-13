using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wx.Runtime.Machine;
using UnityEngine;
using YooAsset;

namespace GameMain.Runtime
{
    public class FsmClearPackageCache : IStateNode
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
            PatchEventDefine.PatchStatesChange.SendEventMessage("清理未使用的缓存文件！");
            UniTask.Void(async () =>
            {
                await ClearPackageCache();
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

        private async UniTask ClearPackageCache()
        {
            try
            {
                var packageName = AppConst.AssetConst.packageName;
                var package = YooAssets.GetPackage(packageName);
                await package.ClearUnusedCacheFilesAsync().WithCancellation(_cancellationTokenSource.Token);
                _machine.ChangeState<FsmUpdaterDone>();
            }
            catch (OperationCanceledException operationCanceledException) when(_cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Warning("UNITASK CANCEL");
            }
        }
        
    }
}