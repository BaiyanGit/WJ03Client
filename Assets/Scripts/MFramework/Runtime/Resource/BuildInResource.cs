using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Wx.Runtime.Resource
{
    public class BuildInResource : IResourceBase
    {
        public T Load<T>(string location) where T : Object
        {
            return Resources.Load<T>(location);
        }

        public async UniTask<T> LoadAsync<T>(string location, CancellationToken cancellationToken) where T : Object
        {
            try
            {
                var value = await Resources.LoadAsync<T>(location).WithCancellation(cancellationToken);
                return value as T;
            }
            catch (OperationCanceledException operationCanceledException)
            {
                WLog.Log("UNITASK CANCEL");
                return null;
            }
        }
    }
}
