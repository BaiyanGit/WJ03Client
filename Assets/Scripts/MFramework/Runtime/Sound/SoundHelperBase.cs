using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace Wx.Runtime.Sound
{
    public abstract class SoundHelperBase : MonoBehaviour
    {
        public abstract SoundSource InstantiateSource(string name,Transform parent);

        public abstract AudioClip LoadAudioClip(string soundAssetName);

        public abstract UniTask<AudioClip> LoadAudioClipAsync(string soundAssetName);
    }
}
