using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime;
using Wx.Runtime.Sound;
using YooAsset;

namespace GameMain.Runtime
{
    public class YooAssetSoundHelper : SoundHelperBase
    {
        public override SoundSource InstantiateSource(string name, Transform parent)
        {
            SoundSource soundSource = new GameObject($"[{name}]").GetOrAddComponent<SoundSource>();
            soundSource.transform.parent = parent;
            soundSource.transform.localPosition = Vector3.zero;
            soundSource.transform.localRotation = Quaternion.identity;
            soundSource.transform.localScale = Vector3.one;
            return soundSource;
        }

        public override AudioClip LoadAudioClip(string soundAssetName)
        {
            var handle = YooAssets.LoadAssetAsync<AudioClip>(soundAssetName);
            handle.WaitForAsyncComplete();
            return handle.AssetObject as AudioClip;
        }

        public override async UniTask<AudioClip> LoadAudioClipAsync(string soundAssetName)
        {
            var handle = YooAssets.LoadAssetAsync<AudioClip>(soundAssetName);
            await handle;
            return handle.AssetObject as AudioClip;
        }
    }
}
