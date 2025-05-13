using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using YooAsset;
using Object = UnityEngine.Object;

namespace Wx.Runtime.Resource
{
    public class YooResource : IResourceBase
    {
        public T Load<T>(string location) where T : Object
        {
            var handle = YooAssets.LoadAssetSync<T>(location);
            var value =  handle.AssetObject as T;
            handle.Release();
            return value;
        }

        public async UniTask<T> LoadAsync<T>(string location, CancellationTokenSource cancellationTokenSource)
            where T : Object
        {
            try
            {
                var handle = YooAssets.LoadAssetAsync<T>(location);
                await handle.WithCancellation(cancellationTokenSource.Token);
                var value = handle.AssetObject as T;
                handle.Release();
                return value;
            }
            catch (OperationCanceledException operationCanceledException) when(cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Log("UNITASK CANCEL");
                return null;
            }
        }

        public TS LoadSub<T, TS>(string location, string subLocation) where T : Object where TS : Object
        {
            var handle = YooAssets.LoadSubAssetsSync<T>(location);
            var value = handle.GetSubAssetObject<TS>(subLocation);
            handle.Release();
            return value;
        }

        public async UniTask<TS> LoadSubAsync<T, TS>(string location, string subLocation, CancellationTokenSource cancellationTokenSource)
            where T : Object where TS : Object
        {
            try
            {
                var handle = YooAssets.LoadSubAssetsAsync<T>(subLocation);
                await handle.WithCancellation(cancellationTokenSource.Token);
                var value = handle.GetSubAssetObject<TS>(subLocation);
                handle.Release();
                return value;
            }
            catch (OperationCanceledException operationCanceledException) when(cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Log("UNITASK CANCEL");
                return null;
            }
        }

        public Object[] LoadAll(string location)
        {
            //location为资源包内的任意资源地址
            var handle = YooAssets.LoadAllAssetsSync(location);
            var value = handle.AllAssetObjects;
            handle.Release();
            return value;
        }

        public async UniTask<Object[]> LoadAllAsync(string location,CancellationTokenSource cancellationTokenSource)
        {
            //location为资源包内的任意资源地址
            try
            {
                var handle = YooAssets.LoadAllAssetsAsync(location);
                await handle.WithCancellation(cancellationTokenSource.Token);
                var value = handle.AllAssetObjects;
                handle.Release();
                return value;
            }
            catch (OperationCanceledException operationCanceledException) when(cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Log("UNITASK CANCEL");
                return null;
            }
        }

        public (byte[], string, string) LoadRaw(string location)
        {
            var handle = YooAssets.LoadRawFileSync(location);
            var data = handle.GetRawFileData();
            var text = handle.GetRawFileText();
            var path = handle.GetRawFilePath();
            handle.Release();
            return (data, text, path);
        }

        public async UniTask<(byte[], string, string)> LoadRawAsync(string location,
            CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var handle = YooAssets.LoadRawFileAsync(location);
                await handle.WithCancellation(cancellationTokenSource.Token);
                var data = handle.GetRawFileData();
                var text = handle.GetRawFileText();
                var path = handle.GetRawFilePath();
                handle.Release();
                return (data, text, path);
            }
            catch (OperationCanceledException operationCanceledException) when(cancellationTokenSource.IsCancellationRequested)
            {
                WLog.Log("UNITASK CANCEL");
                return (null, null, null);
            }
        }
        
        /// <summary>
        /// 卸载所有引用计数为零的资源包。
        /// 可以在切换场景之后调用资源释放方法或者写定时器间隔时间去释放。
        /// </summary>
        /// <param name="packageName"></param>
        public void UnloadUnusedAssets(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            package.UnloadUnusedAssets();
        }

        /// <summary>
        /// 尝试卸载指定的资源对象
        /// 注意：如果该资源还在被使用，该方法会无效。
        /// </summary>
        public void TryUnloadUnusedAsset(string packageName,string location)
        {
            var package = YooAssets.GetPackage(packageName);
            package.TryUnloadUnusedAsset(location);
        }

        /// <summary>
        /// 强制卸载所有资源包，该方法请在合适的时机调用。
        /// 注意：Package在销毁的时候也会自动调用该方法。
        /// </summary>
        public void ForceUnloadAllAssets(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            package.ForceUnloadAllAssets();
        }
        
        
    }
}
