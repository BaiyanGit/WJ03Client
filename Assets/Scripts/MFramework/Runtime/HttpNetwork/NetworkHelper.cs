using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Wx.Runtime.Http
{
    public enum NetworkAuthorizeType
    {
        Token,
        Authorization,
    }
    /// <summary>
    /// Http网络通信，UniTask形式
    /// </summary>
    public static class NetworkHelper
    {
        private const NetworkAuthorizeType AuthorizeType = NetworkAuthorizeType.Token;

        #region 异步Get请求

        /// <summary>
        /// 异步获取Json数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> GetJsonAsync<T>(string url, float timeout, string token = null)
            where T : ResponseDataBase, new()
        {
            var result = new T();
            using var unityWebRequest = UnityWebRequest.Get(url);
            if (!string.IsNullOrEmpty(token))
            {
                unityWebRequest.SetRequestHeader(AuthorizeType.ToString(), token); //设定请求头
            }

            _ = unityWebRequest.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (unityWebRequest.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested)
                {
                    unityWebRequest.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            if (string.IsNullOrEmpty(unityWebRequest.error) && unityWebRequest.downloadHandler.text != null)
            {
                WLog.Warning($"Web JSON：{url} \n" + unityWebRequest.downloadHandler.text);
                result = StringifyHelper.JsonDeSerialize<T>(unityWebRequest.downloadHandler.text);
            }
            else
            {
                WLog.Error(url + "\n" + unityWebRequest.error);
                result.code = -1;
                result.msg = unityWebRequest.error;
            }

            // 取消任务
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();

            return result;
        }


        /// <summary>
        /// 异步获取贴图文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async UniTask<Texture> GetTextureAsync(string url, float timeout)
        {
            Texture result = null;
            using var unityWebRequest = UnityWebRequestTexture.GetTexture(url);
            _ = unityWebRequest.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (unityWebRequest.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested)
                {
                    unityWebRequest.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            if (string.IsNullOrEmpty(unityWebRequest.error) && unityWebRequest.downloadHandler != null)
            {
                WLog.Warning($"Web Texture：”{url}“ Downloaded");
                result = DownloadHandlerTexture.GetContent(unityWebRequest);
            }
            else
            {
                WLog.Error("Get Texture Error ：" + unityWebRequest.error + "\n" + url);
            }

            // 取消任务
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();

            return result;
        }

        /// <summary>
        /// 异步获取精灵文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async UniTask<Sprite> GetSpriteAsync(string url, float timeout)
        {
            var texture = await GetTextureAsync(url, timeout);
            return texture == null
                ? null
                : Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        /// <summary>
        /// 异步获取音频文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async UniTask<AudioClip> GetAudioClipAsync(string url, float timeout)
        {
            AudioClip result = null;
            using var unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
            _ = unityWebRequest.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (unityWebRequest.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested)
                {
                    unityWebRequest.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            if (string.IsNullOrEmpty(unityWebRequest.error) && unityWebRequest.downloadHandler != null)
            {
                WLog.Warning($"Web AudioClip：”{url}“ Downloaded");
                result = DownloadHandlerAudioClip.GetContent(unityWebRequest);
            }
            else
            {
                WLog.Error("Get AudioClip Error ：" + unityWebRequest.error + "\n" + url);
            }

            // 取消任务
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();

            return result;
        }

        #endregion

        #region 异步Post请求

        /// <summary>
        /// 异步发送Json请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tk"></param>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> PostJsonAsync<TK, T>(string url, TK tk, float timeout, string token = null)
            where TK : RequestDataBase where T : ResponseDataBase, new()
        {
            byte[] bodyRaw = null;
            if (tk != null)
            {
                var json = StringifyHelper.JsonSerialize(tk);
                bodyRaw = Encoding.UTF8.GetBytes(json);
            }

            var result = new T();
            using var unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            if (!string.IsNullOrEmpty(token))
            {
                unityWebRequest.SetRequestHeader(AuthorizeType.ToString(), token); //设定请求头
            }

            unityWebRequest.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            _ = unityWebRequest.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (unityWebRequest.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested)
                {
                    unityWebRequest.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            if (string.IsNullOrEmpty(unityWebRequest.error) && unityWebRequest.downloadHandler.text != null)
            {
                WLog.Warning($"Web JSON：{url} \n" + unityWebRequest.downloadHandler.text);
                
                result = StringifyHelper.JsonDeSerialize<T>(unityWebRequest.downloadHandler.text);
            }
            else
            {
                WLog.Error(url + "\n" + unityWebRequest.error);
                result.code = -1;
                result.msg = unityWebRequest.error;
            }

            // 取消任务
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();

            return result;
        }

        /// <summary>
        /// 异步发送表单请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="wwwForm"></param>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> PostFormAsync<T>(string url, WWWForm wwwForm, float timeout,
            string token = null)
            where T : ResponseDataBase, new()
        {
            var result = new T();
            using var unityWebRequest = UnityWebRequest.Post(url, wwwForm);

            if (!string.IsNullOrEmpty(token))
            {
                unityWebRequest.SetRequestHeader(AuthorizeType.ToString(), token); //设定请求头
            }

            _ = unityWebRequest.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (unityWebRequest.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested)
                {
                    unityWebRequest.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            if (string.IsNullOrEmpty(unityWebRequest.error) && unityWebRequest.downloadHandler.text != null)
            {
                WLog.Warning($"Web JSON：{url} \n" + unityWebRequest.downloadHandler.text);
                result = StringifyHelper.JsonDeSerialize<T>(unityWebRequest.downloadHandler.text);
            }
            else
            {
                WLog.Error(url + "\n" + unityWebRequest.error);
                result.code = -1;
                result.msg = unityWebRequest.error;
            }

            // 取消任务
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();

            return result;
        }

        #endregion

        /// <summary>
        /// 删除异步操作
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> DeleteAsync<T>(string url, float timeout,
            string token = null)
            where T : ResponseDataBase, new()
        {
            var result = new T();
            using var unityWebRequest = UnityWebRequest.Delete(url);

            var dh = new DownloadHandlerBuffer();
            unityWebRequest.downloadHandler = dh;

            if (!string.IsNullOrEmpty(token))
            {
                unityWebRequest.SetRequestHeader(AuthorizeType.ToString(), token); //设定请求头
            }

            _ = unityWebRequest.SendWebRequest();

            var timeoutTokenSource = new CancellationTokenSource();
            timeoutTokenSource.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

            while (true)
            {
                if (unityWebRequest.isDone)
                {
                    break;
                }

                if (timeoutTokenSource.IsCancellationRequested)
                {
                    unityWebRequest.Abort();
                    break;
                }

                await UniTask.Yield();
            }

            if (string.IsNullOrEmpty(unityWebRequest.error) && unityWebRequest.downloadHandler.text != null)
            {
                WLog.Warning($"Web JSON：{url} \n" + unityWebRequest.downloadHandler.text);
                result = StringifyHelper.JsonDeSerialize<T>(unityWebRequest.downloadHandler.text);
            }
            else
            {
                WLog.Error(url + "\n" + unityWebRequest.error);
                result.code = -1;
                result.msg = unityWebRequest.error;
            }

            // 取消任务
            timeoutTokenSource.Cancel();
            timeoutTokenSource.Dispose();

            return result;
        }
    }
}