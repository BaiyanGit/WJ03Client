
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.Pool;
using YooAsset;

namespace GameMain.Runtime
{
    public class YooAssetPoolHelper : PoolHelperBase
    {
        public override async UniTask<GameObject> InstantiateHandleAsync(GameObject handle, Transform parent)
        {
            await UniTask.Yield();
            return Instantiate(handle, parent, false);
        }

        public override GameObject InstantiateHandleSync(GameObject handle, Transform parent)
        {
            return Instantiate(handle, parent, false);
        }

        public override async UniTask<GameObject> LoadEntityAsync(string location)
        {
            var handle = YooAssets.LoadAssetAsync(location);
            await handle;
            return handle.AssetObject as GameObject;
        }

        public override GameObject LoadEntitySync(string location)
        {
            return YooAssets.LoadAssetSync(location).AssetObject as GameObject;
        }
    }
}
