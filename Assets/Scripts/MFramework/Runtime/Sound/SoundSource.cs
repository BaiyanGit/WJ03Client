
using DG.Tweening;

using System;

using UnityEngine;

namespace Wx.Runtime.Sound
{
    public class SoundSource : MonoBehaviour
    {

        private AudioSource _audioSource;
        private int _serialId;
        private string _name;
        private bool _isLoop;

        private SoundGroupInfo _soundGroup;

        private Action _playOverCall;
        private Action<SoundSource> _stopCall;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int SerialId
        {
            get => _serialId;
        }

        public bool Pause
        {
            get => !_audioSource.isPlaying;
        }

        private void Awake()
        {
            _audioSource = gameObject.GetOrAddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        public void InitSoundSource(SoundGroupInfo _soundGroupInfo, Action<SoundSource> stopCall)
        {
            _soundGroup = _soundGroupInfo;
            _stopCall = stopCall ?? throw new Exception("sound stopCall is invalid");

            switch (_soundGroup)
            {
                case SoundGroupInfo.Voice:
                    _isLoop = false;
                    //TODO...
                    break;
                case SoundGroupInfo.Effect:
                    _isLoop = false;
                    //TODO...
                    break;
                case SoundGroupInfo.LoopSound:
                    _isLoop = true;
                    //TODO...
                    break;
                case SoundGroupInfo.SceneSound:
                    _isLoop = false;
                    //TODO...
                    break;
            }

            _audioSource.loop = _isLoop;
        }

        public void PlaySound(int serialId, AudioClip audioClip, Action playOverCall)
        {
            _serialId = serialId;
            _name = audioClip.name;
            gameObject.name = $"[{_name}]";
            _audioSource.clip = audioClip;
            _playOverCall = playOverCall;
            _audioSource.Play();

            _audioSource.volume = 0f;
            _audioSource.DOFade(1f, 0.5f);
        }

        public void PauseSound()
        {
            //_audioSource.Pause();

            _audioSource.DOFade(0f, 1f).OnComplete(()=> { _audioSource.Pause(); });
        }

        public void RecoverSound()
        {
            _audioSource.Play();

            _audioSource.volume = 0f;
            _audioSource.DOFade(1f, 1f);
        }

        public void StopSound(bool isCallBack)
        {
            _audioSource.Stop();
            _serialId = -1;
            _name = "WaitingSource";
            gameObject.name = $"[{_name}]";
            _audioSource.clip = null;
            _stopCall?.Invoke(this);
            if (isCallBack)
            {
                _playOverCall?.Invoke();
            }
            _playOverCall = null;
        }

        public void OnUpdate()
        {
            if (_audioSource.clip != null && _audioSource.time >= _audioSource.clip.length)
            {
                StopSound(true);
            }
        }
    }
}
