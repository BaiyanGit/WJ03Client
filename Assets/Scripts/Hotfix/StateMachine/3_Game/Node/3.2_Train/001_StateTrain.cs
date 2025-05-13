using Wx.Runtime.Machine;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.UI;

namespace Hotfix
{
    public class StateTrain : IStateNode
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
            //GameEntry.UI.OpenUIFormAsync<UITopicList>();
            UniTask.Void(async () =>
            {
                GameEntry.Singleton.CreateSingleton<SettlementManager>();
                //await GameEntry.UI.OpenUIFormAsync<UITopicList>();
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