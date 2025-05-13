using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.UI;
using Unity.VisualScripting;
using GameMain.Runtime.UI;

namespace GameMain.Runtime
{
    public class UGuiFormHelper : UIFormHelperBase
    {
        public override object InstantiateUIForm(string uiFormAssetName, UIGroup uiGroup)
        {
            return Instantiate(Resources.Load($"Prefab/UI/{uiFormAssetName}"), uiGroup.Handle).GetOrAddComponent<UGuiForm>();
        }

        public override async UniTask<object> InstantiateUIFormAsync(string uiFormAssetName, UIGroup uiGroup)
        {
            var goCache = await Resources.LoadAsync($"Prefab/UI/{uiFormAssetName}");
            return Instantiate(goCache, uiGroup.Handle).GetOrAddComponent<UGuiForm>();
        }
    }
}
