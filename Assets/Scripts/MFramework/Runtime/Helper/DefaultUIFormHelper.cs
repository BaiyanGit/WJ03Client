using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Wx.Runtime.UI;

namespace Wx.Runtime
{
    public class DefaultUIFormHelper : UIFormHelperBase
    {
        public override object InstantiateUIForm(string uiFormAssetName, UIGroup uiGroup)
        {
            
            return Instantiate(Resources.Load($"Prefab/UI/{uiFormAssetName}"), uiGroup.Handle).GetOrAddComponent<UIFormLogic>();
        }

        public override async UniTask<object> InstantiateUIFormAsync(string uiFormAssetName, UIGroup uiGroup)
        {
            var goCache = await Resources.LoadAsync($"Prefab/UI/{uiFormAssetName}");
            return Instantiate(goCache, uiGroup.Handle).GetOrAddComponent<UIFormLogic>();
        }
    }
}
