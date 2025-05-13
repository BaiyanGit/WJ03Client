using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.Pool
{
    public abstract class PoolHelperBase : MonoBehaviour
    {
        public abstract GameObject LoadEntitySync(string location);

        public abstract UniTask<GameObject> LoadEntityAsync(string location);

        public abstract GameObject InstantiateHandleSync(GameObject handle, Transform parent);

        public abstract UniTask<GameObject> InstantiateHandleAsync(GameObject handle, Transform parent);
    }
}
