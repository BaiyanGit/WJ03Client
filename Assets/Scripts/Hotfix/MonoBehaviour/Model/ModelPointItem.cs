
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Hotfix.UI;
using UnityEngine;

namespace Hotfix
{
    public class ModelPointItem : MonoBehaviour
    {
        [SerializeField] private MonitorValueItem monitorValueItem;

        private List<MeshRenderer> _meshRenderers;
        private List<Tweener> _tweeners;

        private bool _isShowed;
        private bool _isHided;

        private void Awake()
        {
            _tweeners = new List<Tweener>();
            _meshRenderers = this.transform.GetComponentsInChildren<MeshRenderer>().ToList();
        }

        public void ShowModelPoint(float alpha, float showTime)
        {
            if (_isShowed) return;
            AnimateModelPoint(alpha, showTime);
            _isHided = false;
            _isShowed = true;
        }

        public void HideModelPoint(float alpha, float hideTime)
        {
            if (_isHided) return;
            AnimateModelPoint(alpha, hideTime);
            _isHided = true;
            _isShowed = false;
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

        public void ShowModelPointValue(CheckItemData checkItemData)
        {
            monitorValueItem.InitData(checkItemData);
            monitorValueItem.gameObject.SetActive(true);
        }

        public void HideModelPointValue()
        {
            monitorValueItem.gameObject.SetActive(false);
        }
    }
}
