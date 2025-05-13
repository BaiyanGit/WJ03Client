using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Wx.Runtime.Singleton;

namespace GameMain.Runtime
{
    public class GameManager : SingletonInstance<GameManager>, ISingleton
    {
        private const string HotfixDll = "Hotfix";
        private readonly CancellationTokenSource _cancellationTokenSource = new();

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
            _cancellationTokenSource.Cancel();
        }

        public void BuildInStartGame()
        {
            var hotfixAssembly =
                System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == HotfixDll);
            StartGame(hotfixAssembly);
        }

        public async UniTask YooStartGame(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                Assembly hotfixAss = null;
#if UNITY_EDITOR
                hotfixAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == HotfixDll);
#else
                var hotfix =
                    await AppEntry.Resource.YooResource.LoadAsync<TextAsset>("Hotfix.dll", cancellationTokenSource);
                hotfixAss = Assembly.Load(hotfix.bytes);
#endif

                StartGame(hotfixAss);
            }
            catch (OperationCanceledException operationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Log("UNITASK CANCEL" + operationCanceledException);
            }
        }

        public void StartGame(Assembly assembly)
        {
            if (assembly == null)
            {
                WLog.Error($"CAN NOT LOAD HOTFIX DLL");
                return;
            }

            var hotfixEntry = assembly.GetType("Hotfix.GameEntry");
            hotfixEntry.GetMethod("Start")?.Invoke(null, null);
        }
    }
}