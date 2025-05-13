using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.Pool;

namespace Wx.Runtime
{
    public class DefaultPoolHelper : PoolHelperBase
    {
        public override GameObject LoadEntitySync(string location)
        {
            return Resources.Load<GameObject>(location);
        }

        public override GameObject InstantiateHandleSync(GameObject handle, Transform parent)
        {
            return Instantiate<GameObject>(handle, parent, false);
        }

        public override async UniTask<GameObject> LoadEntityAsync(string location)
        {
            var operation = Resources.LoadAsync<GameObject>(location);
            await operation.ToUniTask();
            return operation.asset as GameObject;
        }

        public override async UniTask<GameObject> InstantiateHandleAsync(GameObject handle, Transform parent)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            return Instantiate<GameObject>(handle, parent, false);
        }


    }
}
