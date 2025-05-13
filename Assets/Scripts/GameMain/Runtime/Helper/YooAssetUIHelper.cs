using Cysharp.Threading.Tasks;
using GameMain.Runtime.UI;
using Wx.Runtime.UI;
using YooAsset;
using Wx.Runtime;

namespace GameMain.Runtime
{
    public class YooAssetUIHelper : UIFormHelperBase
    {
        public override object InstantiateUIForm(string uiFormAssetName, UIGroup uiGroup)
        {
            var handle = YooAssets.LoadAssetSync(uiFormAssetName).InstantiateSync(uiGroup.Handle);
            return handle.GetOrAddComponent<UGuiForm>();
        }

        public override async UniTask<object> InstantiateUIFormAsync(string uiFormAssetName, UIGroup uiGroup)
        {
            var handle = YooAssets.LoadAssetAsync(uiFormAssetName).InstantiateAsync(uiGroup.Handle);
            await handle;
            return handle.Result.GetOrAddComponent<UGuiForm>();
        }
    }
}
