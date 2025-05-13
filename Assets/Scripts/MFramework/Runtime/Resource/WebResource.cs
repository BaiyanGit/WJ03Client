using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace Wx.Runtime.Resource
{
    public class WebResource : IResourceBase
    {

        public async UniTask<string> LoadTextAsync(string location, CancellationTokenSource cancellationTokenSource, float timeout = 5f)
        {
            if (cancellationTokenSource == null) return null;

            var handle = UnityWebRequest.Get(location);
            handle.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (handle.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested || cancellationTokenSource.IsCancellationRequested)
                {
                    handle.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            var value = string.Empty;
            if (string.IsNullOrEmpty(handle.error))
            {
                value = handle.downloadHandler.text;
            }

            handle.Dispose();
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();
            return value;
        }

        public async UniTask<Texture> LoadTextureAsync(string location, CancellationTokenSource cancellationTokenSource,
            float timeout = 5f)
        {
            if (cancellationTokenSource == null) return null;

            var handle = UnityWebRequestTexture.GetTexture(location);
            handle.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (handle.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested || cancellationTokenSource.IsCancellationRequested)
                {
                    handle.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            Texture value = null;
            if (string.IsNullOrEmpty(handle.error))
            {
                value = DownloadHandlerTexture.GetContent(handle);
            }

            handle.Dispose();
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();
            return value;
        }

        public async UniTask<Sprite> LoadSpriteAsync(string location, CancellationTokenSource cancellationTokenSource,
            float timeout = 5f)
        {
            var texture = await LoadTextureAsync(location, cancellationTokenSource, timeout);
            return texture == null
                ? null
                : Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        public async UniTask<AudioClip> LoadAudioAsync(string location, CancellationTokenSource cancellationTokenSource,
            float timeout = 5f)
        {
            if (cancellationTokenSource == null) return null;

            var handle = UnityWebRequestMultimedia.GetAudioClip(location, AudioType.UNKNOWN);
            handle.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (handle.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested || cancellationTokenSource.IsCancellationRequested)
                {
                    handle.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            AudioClip value = null;
            if (string.IsNullOrEmpty(handle.error))
            {
                value = DownloadHandlerAudioClip.GetContent(handle);
            }

            handle.Dispose();
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();
            return value;
        }

        public async UniTask DownLoadFile(string location, string savePath, CancellationTokenSource cancellationTokenSource,
            UnityAction<float> progress = null, float timeout = 5f)
        {
            if (cancellationTokenSource == null) return;

            var handle = UnityWebRequest.Get(location);
            handle.downloadHandler = new DownloadHandlerFile(savePath);
            handle.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (handle.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested || cancellationTokenSource.IsCancellationRequested)
                {
                    handle.Abort();
                    break;
                }
                progress?.Invoke(handle.downloadProgress);
                await UniTask.Yield();
            }

            handle.Dispose();
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();
        }
    }
}
