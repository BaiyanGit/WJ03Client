using Wx.Runtime.Machine;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Hotfix.UI;

namespace Hotfix
{
    public class StateSelectModel : IStateNode
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

            //拉起背景音乐
            UniTask.Void(async () => { await GameEntry.Sound.PlaySoundAsync(AppConst.AssetPathConst.LoginBackSound, Wx.Runtime.Sound.SoundGroupInfo.LoopSound); });

            //UniTask.Void(async () => { await GameEntry.UI.OpenUIFormAsync<UIEntry>(); });
            //直接弹登陆界面
            UniTask.Void(async () => { await GameEntry.UI.OpenUIFormAsync<UILogin>(); });
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