using Wx.Runtime.Machine;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.UI;

namespace Hotfix
{
    public class StateTeachingDemo : IStateNode
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

            UniTask.Void(async () => { await GameEntry.UI.OpenUIFormAsync<UITeachingDemo>(); });
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