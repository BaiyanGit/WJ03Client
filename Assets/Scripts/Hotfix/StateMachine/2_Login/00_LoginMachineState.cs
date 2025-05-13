using Wx.Runtime.Machine;
using System.Threading;
using Hotfix.Event;
using Wx.Runtime.Event;

namespace Hotfix
{
    public class LoginMachineState : IStateNode
    {
        private WMachine _machine;
        private CancellationTokenSource _cancellationTokenSource = new();
        
        private readonly EventGroup _eventGroup = new();
        private WMachine _subMachine;
        
        void IStateNode.OnCreate(WMachine machine)
        {
            _machine = machine;
            
            _subMachine = new WMachine(this);
            _subMachine.AddNode<StateLoadLoginScene>();
            _subMachine.AddNode<StateSelectModel>();
            _subMachine.AddNode<StateSimulateModel>();
            _subMachine.AddNode<StateNetModel>();
            _subMachine.AddNode<StateLoadMainScene>();
        }

        void IStateNode.OnEnter(params object[] datas)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _eventGroup.AddListener<UIEventDefine.UIEntrySelectModel>(OnSelectModelHandle);
            
            _subMachine.Run<StateLoadLoginScene>();
        }

        void IStateNode.OnExit()
        {
            _cancellationTokenSource.Cancel();
            _eventGroup.RemoveAllListener();
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
        
        private void OnSelectModelHandle(IEventMessage msg)
        {
            var message = (UIEventDefine.UIEntrySelectModel)msg;
            switch (message.modelIndex)
            {
                case 0:
                    _subMachine.ChangeState<StateSimulateModel>();
                    break;
                case 1:
                    _subMachine.ChangeState<StateNetModel>();
                    break;
                default: break;
            }
        }
    }
}