using System.Threading;
using Cysharp.Threading.Tasks;
using Wx.Runtime.Machine;


namespace GameMain.Runtime
{
    public class FsmUpdaterDone : IStateNode
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
            UniTask.Void(async () =>
            {
                await GameManager.Instance.YooStartGame(_cancellationTokenSource);
                AppEntry.Singleton.DestroySingleton<ConfigManager>();
                AppEntry.Singleton.DestroySingleton<GameManager>();
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
        
        
    }
}