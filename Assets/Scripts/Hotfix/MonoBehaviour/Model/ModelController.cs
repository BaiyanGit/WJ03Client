using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMain.Runtime;
using UnityEngine;
using HighlightPlus;
using UnityEngine.EventSystems;
using Wx.Runtime;

namespace Hotfix
{
    public class ModelController : MonoBehaviour
    {
        private HighlightManager _highlightManager;
        private Transform _modelCamera;
        private Transform _detailPos;

        private Dictionary<int, ModelPointItem> _modelItemDic;
        private Dictionary<int, KeyValuePair<Vector3, Quaternion>> _originalPosAndRotDic;
        private List<MeshRenderer> _meshRenderers;
        private List<Tweener> _tweeners;

        private Transform _currentTarget;
        private Tweener _moveTweener;
        private Vector3 _originalModelPos;
        private Quaternion _originalModelRot;

        private bool _isMouseDown = false;
        private bool _isRightMouseDown = false;
        private Vector3 _startPos;
        private float _scrollValue;
        private Vector3 _modelToCameraDelta;

        private const float RotateSpeed = 15f;
        private const float MoveSpeed = 0.1f;
        private const float ShowAlpha = 1f;
        private const float HideAlpha = 0.1f;
        private const float ShowTime = 0.5f;
        private const float HideTime = 0.5f;


        private void Awake()
        {
            InitField();
            InitAllModelItem();
        }

        public void Control()
        {
            if (null == _currentTarget)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _startPos = Input.mousePosition;
                _isMouseDown = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isMouseDown = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                _startPos = Input.mousePosition;
                _isRightMouseDown = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                _isRightMouseDown = false;
            }

            _scrollValue = Input.GetAxis("Mouse ScrollWheel");
            if (_scrollValue == 0) return;
            _modelToCameraDelta = (_modelCamera.position - _currentTarget.position).normalized;
            _currentTarget.Translate(_modelToCameraDelta * (_scrollValue * MoveSpeed * 1000 * Time.deltaTime), Space.World);
        }

        public void ResetMouseControl()
        {
            _isMouseDown = false;
            _isRightMouseDown = false;
        }

        private void FixedUpdate()
        {
            if (_isMouseDown)
            {
                var currentPos = Input.mousePosition;
                var delta = currentPos - _startPos;
                var rotationAxis = Vector3.Cross(delta, Vector3.forward).normalized;
                var rotateAmount = delta.magnitude * RotateSpeed * Time.fixedDeltaTime;

                _currentTarget.Rotate(rotationAxis, rotateAmount, Space.World);
                _startPos = currentPos;
            }

            if (_isRightMouseDown)
            {
                var currentPos = Input.mousePosition;
                var delta = currentPos - _startPos;
                var moveAmount = delta * (MoveSpeed * Time.fixedDeltaTime);

                _currentTarget.position += moveAmount;
                _startPos = currentPos;
            }

        }

        private void InitAllModelItem()
        {
            foreach (var modelItem in this.transform.GetComponentsInChildren<ModelPointItem>())
            {
                var idS = modelItem.gameObject.name.Split('_')[0];
                var id = int.Parse(idS);
                _modelItemDic.TryAdd(id, modelItem);
                _originalPosAndRotDic.TryAdd(id, new KeyValuePair<Vector3, Quaternion>(modelItem.transform.localPosition, modelItem.transform.localRotation));
            }

            _meshRenderers = this.transform.GetComponentsInChildrenWithoutTx<MeshRenderer, ModelPointItem>();
        }

        private void InitField()
        {
            _currentTarget = this.transform;
            _originalModelPos = _currentTarget.localPosition;
            _originalModelPos = _currentTarget.localEulerAngles;
            _modelItemDic = new Dictionary<int, ModelPointItem>();
            _originalPosAndRotDic = new Dictionary<int, KeyValuePair<Vector3, Quaternion>>();
            _meshRenderers = new List<MeshRenderer>();
            _tweeners = new List<Tweener>();

            _highlightManager = ReferenceCollector.Instance.GetComponent<HighlightManager>("HighlightManager");
            _modelCamera = ReferenceCollector.Instance.GetComponent<Transform>("ModelCamera");
            _detailPos = ReferenceCollector.Instance.GetComponent<Transform>("DetailPos");
        }

        public void ResetTargetTran()
        {
            _currentTarget = this.transform;
        }

        public void ResetModel()
        {
            this.transform.localPosition = _originalModelPos;
            this.transform.localRotation = _originalModelRot;
        }

        public void HighlightModel(int id)
        {
            foreach (var keyValuePair in _modelItemDic)
            {
                if (keyValuePair.Key == id)
                {
                    keyValuePair.Value.ShowModelPoint(ShowAlpha, ShowTime);
                    if (_moveTweener != null && _moveTweener.IsPlaying())
                    {
                        _moveTweener.Kill();
                    }
                    _highlightManager.SelectObject(keyValuePair.Value.transform);
                    _currentTarget = keyValuePair.Value.transform;
                    _moveTweener = keyValuePair.Value.transform.DOLocalMove(_detailPos.position, ShowTime);
                }
                else
                {
                    keyValuePair.Value.HideModelPoint(HideAlpha, HideTime);
                }
            }

            AnimateModelPoint(HideAlpha, HideTime);
        }

        public void UnHighlightModel(int id)
        {
            ResetTargetTran();
            if (_moveTweener != null && _moveTweener.IsPlaying())
            {
                _moveTweener.Kill();
            }
            foreach (var keyValuePair in _modelItemDic)
            {
                if (keyValuePair.Key == id)
                {
                    _highlightManager.UnselectObject(keyValuePair.Value.transform);
                    if (!_originalPosAndRotDic.TryGetValue(keyValuePair.Key, out var posAndRot))
                    {
                        throw new Exception("Can not find original position and rotation of model.");
                    }
                    keyValuePair.Value.transform.localPosition = posAndRot.Key;
                    keyValuePair.Value.transform.localRotation = posAndRot.Value;
                }
                else
                {
                    keyValuePair.Value.ShowModelPoint(ShowAlpha, ShowTime);
                }

            }
            AnimateModelPoint(ShowAlpha, ShowTime);
        }

        public void HighlightModels(List<int> ids)
        {
            foreach (var keyValuePair in _modelItemDic)
            {
                if (ids.Contains(keyValuePair.Key))
                {
                    keyValuePair.Value.ShowModelPoint(ShowAlpha, ShowTime);
                    _highlightManager.SelectObject(keyValuePair.Value.transform);
                }
                else
                {
                    keyValuePair.Value.HideModelPoint(HideAlpha, HideTime);
                }
            }
            AnimateModelPoint(HideAlpha, HideTime);
        }

        public void UnHighlightModels(List<int> ids)
        {
            foreach (var keyValuePair in _modelItemDic)
            {
                if (ids.Contains(keyValuePair.Key))
                {
                    _highlightManager.UnselectObject(keyValuePair.Value.transform);
                }
                else
                {
                    keyValuePair.Value.ShowModelPoint(ShowAlpha, ShowTime);
                }
            }
            AnimateModelPoint(ShowAlpha, ShowTime);
        }

        public void ShowValue(CheckItemData checkItemData)
        {
            foreach (var keyValuePair in _modelItemDic.Where(keyValuePair => keyValuePair.Key == checkItemData.id))
            {
                keyValuePair.Value.ShowModelPointValue(checkItemData);
            }
        }

        public void HideValue(int id)
        {
            foreach (var keyValuePair in _modelItemDic.Where(keyValuePair => keyValuePair.Key == id))
            {
                keyValuePair.Value.HideModelPointValue();
            }
        }

        public void HideAllValue()
        {
            foreach (var keyValuePair in _modelItemDic)
            {
                keyValuePair.Value.HideModelPointValue();
            }
        }

        public void ShowModelWithAnimation(int id, string animationName)
        {

        }

        public void HideModelWithAnimation(int id, string animationName)
        {

        }

        private void AnimateModelPoint(float alpha, float time)
        {
            foreach (var tweener in _tweeners.Where(tweener => tweener.IsPlaying()))
            {
                tweener.Kill();
            }
            _tweeners.Clear();

            foreach (var tweener in from meshRenderer in _meshRenderers from material in meshRenderer.materials select material.DOFade(alpha, time))
            {
                _tweeners.Add(tweener);
            }
        }
    }
}
