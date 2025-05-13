using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wx.Runtime;
using Wx.Runtime.Sound;
using Wx.Runtime.UI;

namespace GameMain.Runtime.UI
{
    public class UGuiForm : UIFormLogic
    {
        private const int DepthFactor = 100;
        private const float FadeTime = 0.33f;

        private static TMP_FontAsset _mainFont = null;
        private Canvas _cachedCanvas = null;
        private CanvasGroup _canvasGroup = null;
        private readonly List<Canvas> _cachedCanvasContainer = new();
        private readonly List<TextMeshProUGUI> _cachedTextMeshProUGUIContainer = new();

        private IUIForm _uiForm = null;

        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return _cachedCanvas.sortingOrder;
            }
        }

        public override void SetMainFont(Object font)
        {
            var mainFont = font as TMP_FontAsset;
            
            if (mainFont == null)
            {
                WLog.Error("Main font is invalid.");
                return;
            }

            _mainFont = mainFont;

            //TODO...切换字体
            foreach (var container in _cachedTextMeshProUGUIContainer)
            {
                container.font = _mainFont;
                container.UpdateFontAsset();
            }
        }

        public override void PlayUIEffect(string soundAssetName)
        {
            AppEntry.Sound.PlaySoundAsync(soundAssetName, SoundGroupInfo.Effect).Forget();
        }

        public override void StopUIEffect(string soundAssetName)
        {
            AppEntry.Sound.StopSound(soundAssetName);
        }

        public void Close(bool ignoreFade = true,bool recycle = false)
        {
            StopAllCoroutines();
            if (ignoreFade)
            {
                //TODO...关闭UIForm
                AppEntry.UI.CloseUIForm(_uiForm, recycle);
            }
            else
            {
                if (!gameObject.activeSelf) return;
                StartCoroutine(CloseCo(FadeTime, recycle));
            }
        }

        private IEnumerator CloseCo(float duration, bool recycle = false)
        {
            yield return _canvasGroup.FadeToAlpha(0f, duration);
            //TODO...关闭UIForm
            AppEntry.UI.CloseUIForm(_uiForm, recycle);
            
        }

        public override void OnInit(IUIForm uiForm)
        {
            _uiForm = uiForm;

            _cachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            _cachedCanvas.overrideSorting = true;
            OriginalDepth = _cachedCanvas.sortingOrder;

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();

            GetComponentsInChildren(true, _cachedTextMeshProUGUIContainer);
        }


        public override void OnOpen()
        {
            _canvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(_canvasGroup.FadeToAlpha(1f, FadeTime));
        }

        public override void OnResume()
        {
            _canvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(_canvasGroup.FadeToAlpha(1f, FadeTime));
        }

        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            var oldDepth = Depth;
            var deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            GetComponentsInChildren(true, _cachedCanvasContainer);
            foreach (var t in _cachedCanvasContainer)
            {
                t.sortingOrder += deltaDepth;
            }
            _cachedCanvasContainer.Clear();
        }

        public override void OnRecycle()
        {
        }

        public override void OnClose()
        {
        }

        public override void OnPause()
        {

        }

        public override void OnCover()
        {
        }

        public override void OnReveal()
        {
        }

        public override void OnRefocus()
        {
        }

        public override void OnUpdate()
        {
        }

        public override void OnFixedUpdate()
        {
        }
    }
}
