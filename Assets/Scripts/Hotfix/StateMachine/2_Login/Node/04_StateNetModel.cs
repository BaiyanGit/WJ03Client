using System;
using Wx.Runtime.Machine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.Event;
using Hotfix.UI;

namespace Hotfix
{
    public class StateNetModel : IStateNode
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

            GameManager.Instance.GameMode = EnumGameMode.Net;
            UniTask.Void(Login);
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

        private async UniTaskVoid Login()
        {
            try
            {
                await GameEntry.UI.OpenUIFormAsync<UILoading>();
                
                UIEventDefine.UILoadingShowPro.SendMessage(0);
                
                //连接服务器
                await ConnectSever();
            }
            catch (OperationCanceledException) when(_cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Warning("UNITASK CANCEL");
            }
        }

        private async UniTask ConnectSever()
        {
            var success = false;
            while (!success)
            {
                var (connect,connectCall) = await GameEntry.Net.CreateClient(AppConst.UrlConst.severIP, AppConst.UrlConst.severPort);
                success = connect;
                if (success) continue;
                if (await ShowRetryDialog(connectCall))continue;
                GameEntry.UI.CloseUIForm<UILoading>();
                ProcessEventDefine.ChangeLoginMachineCall.SendMessage();
                return;
            }
            GameEntry.UI.CloseUIForm<UILoading>();
            //打开登录界面
            await GameEntry.UI.OpenUIFormAsync<UILogin>();
        }
        
        private static async UniTask<bool> ShowRetryDialog(string connectCall)
        {
            var tcs = new UniTaskCompletionSource<bool>();
            UIEventDefine.UIPopTipCall.SendMessage(
                () => tcs.TrySetResult(true),
                "Connect Filed",
                $"{connectCall},Do you want to try again?",
                () => tcs.TrySetResult(false));
            return await tcs.Task;
        }
    }
}