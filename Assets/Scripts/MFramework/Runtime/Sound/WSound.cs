using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

namespace Wx.Runtime.Sound
{
    public class WSound : WModule
    {
        private Dictionary<string, SoundGroup> _soundGroups;
        private Dictionary<string, AudioClip> _audioClips;
        private Dictionary<string, List<int>> _playingAudio;

        private int _serialId;

        [SerializeField]
        private string mSoundHelperTypeName = "Wx.Runtime.DefaultSoundHelper";

        [SerializeField]
        private SoundHelperBase mCustomSoundHelper = null;

        [SerializeField]
        private SoundHelperBase soundHelper = null;

        public override int Priority => 7;

        protected override void Awake()
        {
            base.Awake();
            WLog.Log($"{nameof(WSound)} Initialize");

            _soundGroups = new Dictionary<string, SoundGroup>();
            _audioClips = new Dictionary<string, AudioClip>();
            _playingAudio = new Dictionary<string, List<int>>();
            _serialId = 0;

            soundHelper = Helper.CreateHelper(mSoundHelperTypeName, mCustomSoundHelper);
            if (soundHelper == null)
            {
                throw new Exception("sound helper is invalid");
            }

            soundHelper.name = "Sound Helper";
            var thisTransform = soundHelper.transform;
            thisTransform.SetParent(gameObject.transform);
            thisTransform.localScale = Vector3.one;
            thisTransform.localPosition = Vector3.zero;
            thisTransform.localRotation = Quaternion.identity;


            foreach (var value in Enum.GetValues(typeof(SoundGroupInfo)))
            {
                AddGroup((SoundGroupInfo)value);
            }
        }

        public void AddGroup(SoundGroupInfo soundGroupInfo)
        {
            if (!_soundGroups.TryGetValue(soundGroupInfo.ToString(), out var soundGroup))
            {
                soundGroup = new GameObject($"Sound Group - {soundGroupInfo}").AddComponent<SoundGroup>();
                soundGroup.transform.parent = this.transform;
                soundGroup.transform.localPosition = Vector3.zero;
                soundGroup.transform.localRotation = Quaternion.identity;
                soundGroup.transform.localScale = Vector3.one;
                _soundGroups.Add(soundGroupInfo.ToString(), soundGroup);
            }
            soundGroup.InitSoundGroup(soundGroupInfo.ToString(), soundGroupInfo, soundHelper);
        }

        public SoundGroup GetGroup(SoundGroupInfo soundGroupInfo)
        {
            SoundGroup soundGroup = null;
            foreach (var group in _soundGroups.Values)
            {
                if (group.SoundGroupInfo == soundGroupInfo)
                {
                    soundGroup = group;
                    break;
                }
            }
            if (soundGroup == null)
            {
                throw new Exception($"soundGroup is invalid. SoundGroupInfo is {soundGroupInfo}");
            }

            return soundGroup;
        }

        private SoundGroup GetSoundSource(int serialId)
        {
            SoundGroup soundGroup = null;
            foreach (var group in _soundGroups.Values)
            {
                if (group.HasSoundSource(serialId))
                {
                    soundGroup = group;
                    break;
                }
            }

            if (soundGroup == null)
            {
                throw new Exception($"soundGroup is invalid. serialId is {serialId}");
            }

            return soundGroup;
        }

        private void AddPlayingAudioNameList(string soundAssetName, int serialId)
        {
            if (_playingAudio.TryGetValue(soundAssetName, out var list))
            {
                list.Add(serialId);
            }
            else
            {
                _playingAudio.Add(soundAssetName, new List<int>() { serialId });
            }
        }

        public int PlaySound(string soundAssetName, SoundGroupInfo soundGroupInfo, Action playOverCall = null)
        {
            if (string.IsNullOrEmpty(soundAssetName))
            {
                throw new Exception("soundAssetName is invalid");
            }
            var soundGroup = GetGroup(soundGroupInfo);

            int serialId = ++_serialId;
            var audioClip = soundHelper.LoadAudioClip(soundAssetName);

            if (audioClip != null)
            {
                soundGroup.PlaySound(serialId, audioClip, playOverCall);
                AddPlayingAudioNameList(soundAssetName, serialId);
                return serialId;
            }

            throw new Exception($"audioClip is invalid. soundAssetName is :{soundAssetName}");
        }

        public async UniTask<int> PlaySoundAsync(string soundAssetName, SoundGroupInfo soundGroupInfo, Action playOverCall = null)
        {
            if (string.IsNullOrEmpty(soundAssetName))
            {
                throw new Exception("soundAssetName is invalid");
            }
            var soundGroup = GetGroup(soundGroupInfo);

            int serialId = ++_serialId;
            var audioClip = await soundHelper.LoadAudioClipAsync(soundAssetName);

            if (audioClip != null)
            {
                soundGroup.PlaySound(serialId, audioClip, playOverCall);
                AddPlayingAudioNameList(soundAssetName, serialId);
                return serialId;
            }

            throw new Exception($"audioClip is invalid. soundAssetName is :{soundAssetName}");
        }

        public void PauseSound(string soundAssetName)
        {
            if (!_playingAudio.TryGetValue(soundAssetName, out var list)) return;
            foreach (var serialId in list)
            {
                PauseSound(serialId);
            }
        }

        public void PauseSound(int serialId)
        {
            GetSoundSource(serialId).PauseSound(serialId);
        }

        public void RecoverSound(string soundAssetName)
        {
            if (!_playingAudio.TryGetValue(soundAssetName, out var list)) return;
            foreach (var serialId in list)
            {
                RecoverSound(serialId);
            }
        }

        public void RecoverSound(int serialId)
        {
            GetSoundSource(serialId).RecoverSound(serialId);
        }

        public void StopSound(string soundAssetName)
        {
            if (!_playingAudio.TryGetValue(soundAssetName, out var list)) return;
            for (var i = 0; i < list.Count; i++)
            {
                StopSound(list[i]);
                list.Remove(i);
            }
            if (list.Count != 0)
            {
                _playingAudio.Remove(soundAssetName);
            }
        }

        public void StopSound(int serialId, bool isCallBack = true)
        {
            GetSoundSource(serialId).StopSound(serialId, isCallBack);
        }

        public void PauseGroup(SoundGroupInfo soundGroupInfo)
        {
            GetGroup(soundGroupInfo).PauseAll();
        }

        public void RecoverGroup(SoundGroupInfo soundGroupInfo)
        {
            GetGroup(soundGroupInfo).RecoverAll();
        }

        public void StopGroup(SoundGroupInfo soundGroupInfo, bool isCallBack = true)
        {
            GetGroup(soundGroupInfo).StopAll(isCallBack);
        }

        public void PauseAll()
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                soundGroup.PauseAll();
            }
        }

        public void RecoverAll()
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                soundGroup.RecoverAll();
            }
        }

        public void StopAll(bool isCallBack = true)
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                soundGroup.StopAll(isCallBack);
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            foreach (var soundGroup in _soundGroups.Values)
            {
                soundGroup.OnUpdate();
            }
        }

        public bool IsPaused(string soundAssetName)
        {
            if (!_playingAudio.TryGetValue(soundAssetName, out var list))
            {
                return false;
            }

            for (var i = 0; i < list.Count; i++)
            {
                return GetSoundSource(list[i]).IsPaused(list[i]);
            }

            return false;
        }
    }
}
