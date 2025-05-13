using Wx.Runtime.Machine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.Event;
using Hotfix.UI;

namespace Hotfix
{
    public class StateLoadLoginScene : IStateNode
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

            UniTask.Void(LoadLoginScene);
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

        private async UniTaskVoid LoadLoginScene()
        {
            await GameEntry.UI.OpenUIFormAsync<UILoading>();
            UIEventDefine.UILoadingShowPro.SendMessage(1);
            await GameEntry.Scene.LoadSceneAsync((int)EnumScene.LoginScene, (loading) => { UIEventDefine.UILoadingUpdatePro.SendMessage(loading * 100); });
            GameEntry.UI.CloseUIForm<UILoading>();
            _machine.ChangeState<StateSelectModel>();
        }
    }
}