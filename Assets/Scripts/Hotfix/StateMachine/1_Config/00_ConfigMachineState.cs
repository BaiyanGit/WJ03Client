using Wx.Runtime.Machine;
using System.Threading;

namespace Hotfix
{
    public class ConfigMachineState : IStateNode
    {
        private WMachine _machine;
        private CancellationTokenSource _cancellationTokenSource = new();

        private WMachine _subMachine;
        
        void IStateNode.OnCreate(WMachine machine)
        {
            _machine = machine;
            _subMachine = new WMachine(this);
            _subMachine.AddNode<StateLoadConfig>();
        }

        void IStateNode.OnEnter(params object[] datas)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            _subMachine.Run<StateLoadConfig>();
        }

        void IStateNode.OnExit()
        {
            _cancellationTokenSource.Cancel();
        }

        void IStateNode.OnUpdate()
        {
            _subMachine.Update();
        }

        void IStateNode.OnFixedUpdate()
        {
            _subMachine.FixedUpdate();
        }

        void IStateNode.OnLateUpdate()
        {
            _subMachine.LateUpdate();
        }

        void IStateNode.OnDestroy()
        {
            _subMachine.Destroy();
            _cancellationTokenSource.Cancel();
        }
    }
}