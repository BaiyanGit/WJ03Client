using Wx.Runtime.Machine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.Event;
using Hotfix.UI;
using UnityEngine.SceneManagement;

namespace Hotfix
{
    public class StateLoadMainScene : IStateNode
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

            LoadMainScene().Forget();
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

        private async UniTask LoadMainScene()
        {
            await GameEntry.UI.OpenUIFormAsync<UILoading>();
            UIEventDefine.UILoadingShowPro.SendMessage(1);

            await GameEntry.Scene.LoadSceneAsync((int)EnumScene.SubScene, (loading) => { LoadingUpdate(loading, 0f, 1f); });
            await GameEntry.Scene.LoadSceneAsync((int)EnumScene.MainScene, null, true, LoadSceneMode.Additive);

            GameEntry.UI.CloseUIForm<UILoading>();
            _machine.ChangeState<StateSelectTopic>();
            //加载额外模型？
            ModelLoaderManager.Instance.LoadAllModels();
        }

        private void LoadingUpdate(float loading, float baseValue, float ratio)
        {
            UIEventDefine.UILoadingUpdatePro.SendMessage(loading * 100f * ratio + baseValue);
        }
    }
}