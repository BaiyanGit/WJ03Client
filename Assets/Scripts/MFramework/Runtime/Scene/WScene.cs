using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using YooAsset;

namespace Wx.Runtime.Scene
{
    public class WScene : WModule
    {
        public override int Priority => 3;

        protected override void Awake()
        {
            base.Awake();
            WLog.Log($"{nameof(WScene)} initialize !");
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {

        }

        /// <summary>
        /// 重新切换当前场景
        /// </summary>
        public void ReLoadActiveScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// 加载下一个场景
        /// </summary>
        /// <param name="isCyclical">是否场景循环</param>
        public void LoadNextScene(bool isCyclical = false)
        {
            var buildIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (buildIndex > SceneManager.sceneCountInBuildSettings - 1)
            {
                if (isCyclical)
                {
                    buildIndex = 0;
                }
                else
                {
                    WLog.Warning($"加载场景失败！场景索引越界");
                    return;
                }
            }
            SceneManager.LoadScene(buildIndex);
        }

        /// <summary>
        /// 加载上一个场景
        /// </summary>
        /// <param name="isCyclical">是否场景循环</param>
        public void LoadPreviousScene(bool isCyclical = false)
        {
            var buildIndex = SceneManager.GetActiveScene().buildIndex - 1;

            if (buildIndex < 0)
            {
                if (isCyclical)
                {
                    buildIndex = SceneManager.sceneCountInBuildSettings - 1;
                }
                else
                {
                    WLog.Warning($"加载场景失败！场景索引越界");
                    return;
                }
            }
            SceneManager.LoadScene(buildIndex);
        }

        /// <summary>
        /// 异步加载场景，根据名字来切换场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loading"></param>
        /// <param name="setActiveAfterCompleted"></param>
        /// <param name="mode"></param>
        private async UniTask LoadScene(string sceneName, UnityAction<float> loading = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //开始加载资源
            var handler = SceneManager.LoadSceneAsync(sceneName, mode);
            handler.allowSceneActivation = false; //资源加载最多到0.9

            //等待资源加载完毕
            while (handler.progress < 0.9f)
            {
                loading?.Invoke(handler.progress);
                await UniTask.Yield();
            }
            //当asyncOperation.allowSceneActivation 为false，则asyncOperation.Progress最多只能到达0.9
            //凑成1，可以方便外部进度条的显示
            loading?.Invoke(1);
            handler.allowSceneActivation = setActiveAfterCompleted;
            await UniTask.Delay(100);
        }

        /// <summary>
        /// 异步加载场景，根据索引来切换
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <param name="loading"></param>
        /// <param name="setActiveAfterCompleted"></param>
        /// <param name="mode"></param>
        /// <param name="millisecondsDelay"></param>
        public async UniTask LoadSceneAsync(int buildIndex, UnityAction<float> loading = null, bool setActiveAfterCompleted = true,
            LoadSceneMode mode = LoadSceneMode.Single, int millisecondsDelay = 100)
        {
            //开始加载资源
            var handler = SceneManager.LoadSceneAsync(buildIndex, mode);
            handler.allowSceneActivation = false; //资源加载最多到0.9

            //等待资源加载完毕
            //while (handler.progress < 0.9f)
            //{
            //    loading?.Invoke(handler.progress);
            //    await UniTask.Yield();
            //}

            while (!handler.isDone)
            {
                loading?.Invoke(handler.progress);
                await UniTask.Yield();
                await UniTask.NextFrame();

                if (handler.progress >= 0.9f)
                {
                    loading?.Invoke(1);
                    await UniTask.Delay(millisecondsDelay);
                    handler.allowSceneActivation = setActiveAfterCompleted;
                }
            }

            //当asyncOperation.allowSceneActivation 为false，则asyncOperation.Progress最多只能到达0.9
            //凑成1，可以方便外部进度条的显示
            //loading?.Invoke(1);
            //handler.allowSceneActivation = setActiveAfterCompleted;

            //await UniTask.Delay(millisecondsDelay);
        }

        public async UniTask YLoadSceneTask(string sceneName, UnityAction<float> loading = null, UnityAction<SceneHandle> completed = null, bool suspendLoad = false,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            //开始加载资源
            var handler = YooAssets.LoadSceneAsync(sceneName, mode, suspendLoad);

            //等待资源加载完毕
            while (handler.Progress < 0.9f)
            {
                loading?.Invoke(handler.Progress);
                await UniTask.Yield();
            }
            //凑成1，可以方便外部进度条的显示
            loading?.Invoke(1);
            await UniTask.Delay(100);
            completed?.Invoke(handler);
        }

        /// <summary>
        /// 异步加载场景，根据索引来切换
        /// </summary>
        /// <param name="buildIndex"></param>
        /// <param name="loading"></param>
        /// <param name="setActiveAfterCompleted"></param>
        /// <param name="mode"></param>
        /// <param name="millisecondsDelay"></param>
        public async UniTask UnLoadSceneAsync(int buildIndex, UnityAction Unloading = null, bool setActiveAfterCompleted = true, int millisecondsDelay = 100)
        {
            //开始加载资源
            AsyncOperation handler = SceneManager.UnloadSceneAsync(buildIndex);
            Debug.Log(handler);
            while (handler.isDone)
            {
                Unloading?.Invoke();
                await UniTask.Yield();
                await UniTask.NextFrame();
                Debug.Log(handler.progress);
                if (handler.progress >= 0.9f)
                {
                    Unloading?.Invoke();
                    await UniTask.Delay(millisecondsDelay);
                }
            }
        }
    }


}

