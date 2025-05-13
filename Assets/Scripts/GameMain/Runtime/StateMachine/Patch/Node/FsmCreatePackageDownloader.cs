using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wx.Runtime.Machine;
using YooAsset;

namespace GameMain.Runtime
{
    public class FsmCreatePackageDownloader : IStateNode
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
            PatchEventDefine.PatchStatesChange.SendEventMessage("����������������");
            UniTask.Void(async () =>
            {
                await CreateDownloader();
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
        
        private async UniTask CreateDownloader()
        {
            try
            {
                await UniTask.Delay(500, cancellationToken: _cancellationTokenSource.Token);

                var package = YooAssets.GetPackage(AppConst.AssetConst.packageName);
                int downloadingMaxNum = 10;
                int failedTryAgain = 3;
                var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
                _machine.SetBlackboardValue("Downloader", downloader);

                if (downloader.TotalDownloadCount == 0)
                {
                    WLog.Log("Not found any download files !");
                    _machine.ChangeState<FsmUpdaterDone>();
                }
                else
                {
                    // �����¸����ļ��󣬹�������ϵͳ
                    // ע�⣺��������Ҫ������ǰ�����̿ռ䲻��
                    int totalDownloadCount = downloader.TotalDownloadCount;
                    long totalDownloadBytes = downloader.TotalDownloadBytes;
                    PatchEventDefine.FoundUpdateFiles.SendEventMessage(totalDownloadCount, totalDownloadBytes);
                }
            }
            catch (OperationCanceledException operationCanceledException) when(_cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Warning("UNITASK CANCEL");
            }
        }
    }
}