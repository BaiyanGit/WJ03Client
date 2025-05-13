using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.UI
{
    public abstract class UIFormHelperBase : MonoBehaviour
    {
        public abstract object InstantiateUIForm(string uiFormAssetName,UIGroup uiGroup);

        public abstract UniTask<object> InstantiateUIFormAsync(string uiFormAssetName, UIGroup uiGroup);
    }
}
