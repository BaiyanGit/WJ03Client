using System;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using Hotfix.Event;
using Hotfix.UI;

using Wx.Runtime.Http;
using Wx.Runtime.Singleton;

namespace Hotfix
{
    /// <summary>
    /// 硬件管理器
    /// </summary>
    public class HardwareManager : SingletonInstance<HardwareManager>, ISingleton
    {
        public bool connectHardware;

        public void OnCreate(object createParam)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLateUpdate()
        {
        }

        public void OnDestroy()
        {
        }

        /// <summary>
        /// 初始化硬件
        /// </summary>
        /// <returns></returns>
        public async UniTask<bool> InitHardware(CancellationToken cancellationToken)
        {
            var isConnectHardware = false;
            while (!isConnectHardware)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return false;
                }

                isConnectHardware = await InternalInitHardware(cancellationToken);
                if (isConnectHardware) continue;
                var shouldRetry = await ShowRetryDialog();
                if (shouldRetry) continue;
                GameManager.Instance.QuitApplication();
                return false;
            }

            return true;
        }

        public int GetHardwareValue(int hardwareSerialId)
        {
            return -1;
        }

        public bool GetHardwareState(int hardwareSerialId)
        {
            return false;
        }

        private async UniTask<bool> InternalInitHardware(CancellationToken cancellationToken)
        {
            await UniTask.Yield();
            connectHardware = true;
            return connectHardware;
        }

        private async UniTask<bool> ShowRetryDialog()
        {
            var tcs = new UniTaskCompletionSource<bool>();
            UIEventDefine.UIPopTipCall.SendMessage(
                () => tcs.TrySetResult(true),
                "Failed",
                "Connect Hardware failed, commit to try again?",
                () => tcs.TrySetResult(false));
            return await tcs.Task;
        }
    }
}