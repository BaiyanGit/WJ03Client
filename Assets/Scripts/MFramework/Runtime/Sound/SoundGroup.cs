using System;
using System.Collections.Generic;

using UnityEngine;

namespace Wx.Runtime.Sound
{
    public class SoundGroup : MonoBehaviour
    {
        private string _name;
        private SoundGroupInfo _soundGroupInfo;
        private SoundHelperBase _soundHelper;

        private Queue<SoundSource> _sourceCaches;

        private LinkedList<SoundSource> _playingSources;
        private List<SoundSource> _pausedSources;

        private LinkedListNode<SoundSource> _mCachedNode;

        public string Name
        {
            get => _name;
        }

        public SoundGroupInfo SoundGroupInfo
        {
            get => _soundGroupInfo;
        }


        public void InitSoundGroup(string name, SoundGroupInfo soundGroupInfo, SoundHelperBase soundHelper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Sound Group name is invalid");
            }
            if (soundHelper == null)
            {
                throw new Exception("Sound Helper is invalid");
            }

            _name = name;
            _soundGroupInfo = soundGroupInfo;
            _soundHelper = soundHelper;
            _sourceCaches = new Queue<SoundSource>();
            _playingSources = new LinkedList<SoundSource>();
            _pausedSources = new List<SoundSource>();
        }

        public bool HasSoundSource(int serialId)
        {
            foreach (var soundSource in _playingSources)
            {
                if (soundSource.SerialId == serialId)
                {
                    return true;
                }
            }

            foreach (var soundSource in _pausedSources)
            {
                if (soundSource.SerialId == serialId)
                {
                    return true;
                }
            }

            return false;
        }

        public SoundSource GetPlayingSource(int serialId)
        {
            foreach (var soundSource in _playingSources)
            {
                if (soundSource.SerialId == serialId)
                {
                    return soundSource;
                }
            }
            return null;
        }

        public SoundSource GetPausedSource(int serialId)
        {
            foreach (var soundSource in _pausedSources)
            {
                if (soundSource.SerialId == serialId)
                {
                    return soundSource;
                }
            }
            return null;
        }

        public void PlaySound(int serialId, AudioClip audioClip, Action playOverCallBack)
        {
            if (audioClip == null)
            {
                throw new Exception("AudioClip is invalid");
            }
            SoundSource soundSource;
            if (_sourceCaches.Count > 0)
            {
                soundSource = _sourceCaches.Dequeue();
            }
            else
            {
                soundSource = _soundHelper.InstantiateSource(audioClip.name, this.transform);
                soundSource.InitSoundSource(_soundGroupInfo, StopCallBack);
            }
            soundSource.PlaySound(serialId, audioClip, playOverCallBack);
            _playingSources.AddFirst(soundSource);

        }

        public void PauseSound(int serialId)
        {
            var soundSource = GetPlayingSource(serialId);
            if (soundSource != null)
            {
                if (!soundSource.Pause)
                {
                    soundSource.PauseSound();
                    _playingSources.Remove(soundSource);
                    _pausedSources.Add(soundSource);
                }
            }
            else
            {
                throw new Exception($"soundSource is invalid.serialId :{serialId}");
            }
        }

        public void RecoverSound(int serialId)
        {
            var soundSource = GetPausedSource(serialId);
            if (soundSource != null)
            {
                if (soundSource.Pause)
                {
                    soundSource.RecoverSound();
                    _pausedSources.Remove(soundSource);
                    _playingSources.AddFirst(soundSource);
                }
            }
            else
            {
                throw new Exception($"soundSource is invalid.serialId :{serialId}");
            }
        }

        public void StopSound(int serialId, bool isCallBack)
        {
            var soundSource = GetPlayingSource(serialId);
            if (soundSource != null)
            {
                soundSource.StopSound(isCallBack);
                _playingSources.Remove(soundSource);
            }
            else
            {
                soundSource = GetPausedSource(serialId);
                if (soundSource != null)
                {
                    soundSource.StopSound(isCallBack);
                    _pausedSources.Remove(soundSource);
                }
                else
                {
                    throw new Exception($"soundSource is invalid.serialId :{serialId}");
                }
            }
        }

        public void PauseAll()
        {
            foreach (var soundSource in _playingSources)
            {
                if (!soundSource.Pause)
                {
                    soundSource.PauseSound();
                    _playingSources.Remove(soundSource);
                    _pausedSources.Add(soundSource);
                }
            }
        }

        public void RecoverAll()
        {
            foreach (var soundSource in _pausedSources)
            {
                if (soundSource.Pause)
                {
                    soundSource.RecoverSound();
                    _pausedSources.Remove(soundSource);
                    _playingSources.AddFirst(soundSource);
                }
            }
        }
        public void StopAll(bool isCallBack)
        {
            foreach (var soundSource in _playingSources)
            {
                soundSource.StopSound(isCallBack);
                _playingSources.Remove(soundSource);
            }
            foreach (var soundSource in _pausedSources)
            {
                soundSource.StopSound(isCallBack);
                _pausedSources.Remove(soundSource);
            }
        }

        private void StopCallBack(SoundSource soundSource)
        {
            if (_playingSources.Contains(soundSource))
            {
                _playingSources.Remove(soundSource);
            }
            if (_pausedSources.Contains(soundSource))
            {
                _pausedSources.Remove(soundSource);
            }
            _sourceCaches.Enqueue(soundSource);
        }


        public void OnUpdate()
        {
            LinkedListNode<SoundSource> current = _playingSources.First;
            while (current != null)
            {
                _mCachedNode = current.Next;
                current.Value.OnUpdate();
                current = _mCachedNode;
                _mCachedNode = null;
            }

        }

        public bool IsPaused(int serialId)
        {
            var soundSource = GetPausedSource(serialId);
            if (soundSource != null)
            {
                return soundSource.Pause;
            }

            return false;
        }

    }
}
