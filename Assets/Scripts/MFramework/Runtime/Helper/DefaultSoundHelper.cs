using Cysharp.Threading.Tasks;
using UnityEngine;
using Wx.Runtime.Sound;

namespace Wx.Runtime
{
    public class DefaultSoundHelper : SoundHelperBase
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
            return Resources.Load<AudioClip>($"Audio/{soundAssetName}");
        }

        public override async UniTask<AudioClip> LoadAudioClipAsync(string soundAssetName)
        {
            var handle = Resources.LoadAsync<AudioClip>($"Audio/{soundAssetName}");
            await handle;
            return handle.asset as AudioClip;
        }

    }
}
