using Wx.Runtime.Machine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.Event;
using Hotfix.UI;
using UnityEngine;

namespace Hotfix
{
    public class StateLoadConfig : IStateNode
    {
        private WMachine _machine;
        private CancellationTokenSource _cancellationTokenSource = new();

        void IStateNode.OnCreate(WMachine machine)
        {
            _machine = machine;
        }

        void IStateNode.OnEnter(params object[] data)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            UniTask.Void(Entry);
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

        private async UniTaskVoid Entry()
        {
            await GameEntry.UI.OpenUIFormAsync<UILoading>();
            UIEventDefine.UILoadingShowPro.SendMessage(0);
            await ConfigManager.Instance.InitGlobalConfig();
            await ConfigManager.Instance.InitNetConfig(_cancellationTokenSource);
            //开启服务器
            ServNet.Instance.StartServer(AppConst.UrlConst.selfIP, AppConst.UrlConst.selfPort);
            await GameEntry.Excel.LoadStreamingAsync();
            var connectHardwareResult = await HardwareManager.Instance.InitHardware(_cancellationTokenSource.Token);
            if (!connectHardwareResult) return;
            GameEntry.UI.CloseUIForm<UILoading>();
            ProcessEventDefine.ChangeLoginMachineCall.SendMessage();
        }
    }
}