using System;

namespace Wx.Runtime.Timer
{
    public class Timer
    {
        private bool _isPositiveTiming;
        private float _duration;
        private Action _call;
        private Action<Timer> _timeEndCall;

        private float _currentTime;
        private bool _pause;

        public bool Pause
        {
            get { return _pause; }
            set { _pause = value; }
        }

        internal void Initialize(float duration, bool isPositiveTiming, Action call, Action<Timer> timeEndCall)
        {
            _duration = duration;
            _isPositiveTiming = isPositiveTiming;
            _call = call;
            _timeEndCall = timeEndCall;
            _pause = false;
            if (_isPositiveTiming)
            {
                _currentTime = 0;
            }
            else
            {
                _currentTime = _duration;
            }
        }

        internal void OnUpdate(float deltaTime)
        {
            if (_isPositiveTiming)
            {
                _currentTime += deltaTime;
                if (_duration < 0 || !(_currentTime >= _duration)) return;
                _call?.Invoke();
                _timeEndCall?.Invoke(this);
            }
            else
            {
                _currentTime -= deltaTime;
                if (!(_currentTime <= 0)) return;
                _call?.Invoke();
                _timeEndCall?.Invoke(this);
            }
        }
        
        public string GetTimeString()
        {
            var minutes = (int)(_currentTime / 60);
            var seconds = (int)(_currentTime % 60);
            return $"{minutes:00}:{seconds:00}";
        }

        public void ShutDown()
        {
            _call?.Invoke();
            _timeEndCall?.Invoke(this);
        }
        
        
        
        
    }
}