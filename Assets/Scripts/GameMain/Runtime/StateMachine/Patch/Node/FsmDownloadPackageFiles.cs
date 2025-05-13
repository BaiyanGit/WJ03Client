using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wx.Runtime.Machine;
using UnityEngine;
using YooAsset;

namespace GameMain.Runtime
{
    public class FsmDownloadPackageFiles : IStateNode
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
            PatchEventDefine.PatchStatesChange.SendEventMessage("开始下载补丁文件！");
            UniTask.Void(async() =>
            {
                await BeginDownload();
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
        
        private async UniTask BeginDownload()
        {
            try
            {
                var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue("Downloader");
                downloader.OnDownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
                downloader.OnDownloadProgressCallback = PatchEventDefine.DownloadProgressUpdate.SendEventMessage;
                downloader.BeginDownload();
                await downloader.ToUniTask(cancellationToken: _cancellationTokenSource.Token);

                // 检测下载结果
                if (downloader.Status != EOperationStatus.Succeed)
                    await UniTask.Yield();

                _machine.ChangeState<FsmDownloadPackageOver>();
            }
            catch (OperationCanceledException operationCanceledException) when(_cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Warning("UNITASK CANCEL");
            }
        }
    }
}