using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wx.Runtime.Timer
{
    public class WTimer : WModule
    {
        public override int Priority => 11;

        private LinkedList<Timer> _playingTimers;
        private Queue<Timer> _cacheTimers;
        private LinkedListNode<Timer> _mCachedNode;

        protected override void Awake()
        {
            base.Awake();

            _playingTimers = new LinkedList<Timer>();
            _cacheTimers = new Queue<Timer>();
            _mCachedNode = null;

            WLog.Log($"{nameof(WTimer)} initialize !");
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (_playingTimers.Count <= 0) return;
            
            var current = _playingTimers.First;
            
            while (current != null)
            {
                if (current.Value.Pause)
                {
                    current = current.Next;
                    continue;
                }
                _mCachedNode = current.Next;
                current.Value.OnUpdate(deltaTime);
                current = _mCachedNode;
                _mCachedNode = null;
            }
        }

        public Timer CreateTimer(float duration, bool isPositiveTiming, Action call)
        {
            var timer = _cacheTimers.Count > 0 ? _cacheTimers.Dequeue() : new Timer();

            timer.Initialize(duration, isPositiveTiming, call, (t) =>
            {
                _playingTimers.Remove(t);
                _cacheTimers.Enqueue(t);
            });
            _playingTimers.AddFirst(timer);
            return timer;
        }
        
        
    }
}