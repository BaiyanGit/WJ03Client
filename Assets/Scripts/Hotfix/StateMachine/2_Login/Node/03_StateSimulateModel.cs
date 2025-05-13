using Wx.Runtime.Machine;
using UnityEngine;
using System.Threading;
using Hotfix.Event;

namespace Hotfix
{
    public class StateSimulateModel : IStateNode
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

            GameManager.Instance.GameMode = EnumGameMode.Simulate;
            ProcessEventDefine.ChangeGameMachineCall.SendMessage();
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
    }
}