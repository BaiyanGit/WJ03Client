
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Hotfix.UI
{
    public class CheckTipItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;

        private Tweener _tweener;
        

        public void ShowTip(string tip)
        {
            if (_tweener != null)
            {
                _tweener.onComplete = null;
                _tweener.Kill();
            }
            transform.localScale = Vector3.zero;
            titleText.text = @tip;
            gameObject.SetActive(true);
            _tweener = transform.DOScale(1, 0.2f);
        }

        public void HideTip()
        {
            if (_tweener != null)
            {
                _tweener.onComplete = null;
                _tweener.Kill();
            }
            
            _tweener = transform.DOScale(0, 0.2f);
            _tweener.onComplete = () =>
            {
                gameObject.SetActive(false);
            };
        }
    }

}