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
            //locationΪ��Դ���ڵ�������Դ��ַ
            var handle = YooAssets.LoadAllAssetsSync(location);
            var value = handle.AllAssetObjects;
            handle.Release();
            return value;
        }

        public async UniTask<Object[]> LoadAllAsync(string location,CancellationTokenSource cancellationTokenSource)
        {
            //locationΪ��Դ���ڵ�������Դ��ַ
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
        /// ж���������ü���Ϊ�����Դ����
        /// �������л�����֮�������Դ�ͷŷ�������д��ʱ�����ʱ��ȥ�ͷš�
        /// </summary>
        /// <param name="packageName"></param>
        public void UnloadUnusedAssets(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            package.UnloadUnusedAssets();
        }

        /// <summary>
        /// ����ж��ָ������Դ����
        /// ע�⣺�������Դ���ڱ�ʹ�ã��÷�������Ч��
        /// </summary>
        public void TryUnloadUnusedAsset(string packageName,string location)
        {
            var package = YooAssets.GetPackage(packageName);
            package.TryUnloadUnusedAsset(location);
        }

        /// <summary>
        /// ǿ��ж��������Դ�����÷������ں��ʵ�ʱ�����á�
        /// ע�⣺Package�����ٵ�ʱ��Ҳ���Զ����ø÷�����
        /// </summary>
        public void ForceUnloadAllAssets(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            package.ForceUnloadAllAssets();
        }
        
        
    }
}
