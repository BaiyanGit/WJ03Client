using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GameMain.Runtime
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollViewEx : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        private enum SnapState
        {
            None,
            Inertia,
            Reverse,
        }

        private const float SnapReverseSpeed = 100f;

        [SerializeField] private bool snap;
        [SerializeField] private bool scale;
        [SerializeField, Range(0.5f, 2f)] private float maxScale = 1.2f;
        [SerializeField, Range(0.5f, 2f)] private float minScale = 1f;

        private ScrollRect _scrollRect;
        private RectTransform _content;
        private bool _isInit;
        private Vector2 _size;
        private Vector2 _childSize;
        private float _spacing;
        private float _snapDecelerate;
        private float _offsetX;
        private Vector2 _snapTargetPos;
        private SnapState _snapState;
        private List<RectTransform> _children;
        private int _index;

        public Action<int> onIndexChanged;

        private void Update()
        {
            if (!_isInit) return;
            switch (_snapState)
            {
                case SnapState.Inertia:
                    UpdateSnapInertia();
                    break;
                case SnapState.Reverse:
                    UpdateSnapReverse();
                    break;
                case SnapState.None:
                default:
                    break;
            }

            if (!_content.hasChanged || !scale) return;
            UpdateScale();
        }


        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StartSnap();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            BreakSnap();
        }

        public void Init()
        {
            _isInit = false;

            _scrollRect = GetComponent<ScrollRect>();
            _content = _scrollRect.content;

            if (!_scrollRect.horizontal) return;
            _children = new List<RectTransform>();
            _size = _scrollRect.viewport.rect.size;

            for (var i = 0; i < _content.childCount; i++)
            {
                _children.Add(_content.GetChild(i).GetComponent<RectTransform>());
            }

            if (_children.Count == 0) return;

            _childSize = _children[0].rect.size;
            var layout = _content.GetComponent<HorizontalLayoutGroup>();
            _spacing = layout.spacing;
            _offsetX = _spacing + _childSize.x;

            _snapDecelerate = _scrollRect.decelerationRate;

            if (scale)
            {
                UpdateScale();
            }

            _index = 0;
            onIndexChanged?.Invoke(_index);
            _isInit = true;
        }


        private void UpdateSnapInertia()
        {
            if (_scrollRect.velocity.x is <= -SnapReverseSpeed or >= SnapReverseSpeed) return;
            StartSnapReverse();
            return;
        }

        private void UpdateSnapReverse()
        {
            if (Mathf.Abs(_content.anchoredPosition.x - _snapTargetPos.x) < 1)
            {
                _content.anchoredPosition = _snapTargetPos;
                EndSnap();
                return;
            }

            _content.anchoredPosition = Vector2.Lerp(_content.anchoredPosition, _snapTargetPos, _snapDecelerate);
        }

        /// <summary>
        /// 0 -> 0
        /// 1 -> 0+gridSpace+childSize.x
        /// 2 -> 0+gridSpace*2+childSize.x*2
        /// </summary>
        private void StartSnapReverse()
        {
            _index = (int)(_content.anchoredPosition.x / _offsetX);

            if (_index > 0) _index = 0;
            if (Mathf.Abs(_index) >= _children.Count) _index = -(_children.Count-1);
            
            _snapTargetPos.x = _index * _offsetX;
            _snapTargetPos.y = _content.anchoredPosition.y;
            onIndexChanged?.Invoke(Mathf.Abs(_index));
            
            _snapState = SnapState.Reverse;
            _scrollRect.StopMovement();
        }

        private void EndSnap()
        {
            if (_snapState == SnapState.None) return;

            _scrollRect.StopMovement();
            _snapState = SnapState.None;
        }

        private void BreakSnap()
        {
            _snapState = SnapState.None;
        }

        private void StartSnap()
        {
            if (!snap) return;
            if (_children.Count == 0) return;
            _snapState = SnapState.Inertia;
        }

        private void UpdateScale()
        {
            var tempCenter = Mathf.Abs(_content.anchoredPosition.x) + _size.x / 2;
            foreach (var children in _children)
            {
                if (!children.gameObject.activeSelf)
                {
                    continue;
                }

                var tempOffset = Mathf.Abs(tempCenter - children.anchoredPosition.x);
                if (tempOffset > _size.x / 2 + _childSize.x)
                {
                    continue;
                }

                var tempScale = maxScale - (tempOffset / (_size.x));
                if (tempScale < minScale)
                {
                    tempScale = minScale;
                }

                children.localScale = new Vector3(tempScale, tempScale, 1);
            }
        }

        public void ResetScrollEx()
        {
            _snapState = SnapState.None;
            _index = 0;
            _content.anchoredPosition = Vector2.zero;
            onIndexChanged?.Invoke(_index);
        }
    }
}