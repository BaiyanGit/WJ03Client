using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 声音数据控制
/// </summary>
public class VoiceDataItem : MonoBehaviour
{
    Button currentButton;
    Image currentImage;
    private float fillDuration = 5f;

    // 保存Tween引用以便控制
    private Tween fillTween;

    public bool _isDefaultValue;

    public int _id;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(int id, Sprite imageSprite, bool defaultValue = false)
    {
        currentButton = transform.GetComponentInChildren<Button>();
        currentImage = transform.Find("Img_Value").GetComponent<Image>();

        currentImage.fillAmount = 0;
        _isDefaultValue = defaultValue;
        _id = id;

        currentImage.sprite = imageSprite;
        currentButton.onClick.AddListener(BtnClickAction);
    }

    void BtnClickAction()
    {
        // 如果已有动画在运行，先终止
        if (fillTween != null && fillTween.IsActive())
        {
            fillTween.Kill();
        }

        // 禁用按钮防止重复点击
        currentButton.interactable = false;
        currentImage.sprite = GetSprite(_isDefaultValue);

        // 使用DOTween动画控制FillAmount
        fillTween = currentImage.DOFillAmount(1f, fillDuration)
            .SetEase(Ease.Linear) // 线性填充（可替换为其他缓动类型）
            .OnComplete(() =>
            {
                // 动画完成后的回调
                Debug.Log("填充完成！");
                // 启用按钮
                currentButton.interactable = true;
                AudioWaveTexture._instance.SetDefaultTexture();
            });
    }

    private void OnDestroy()
    {
        // 确保销毁时释放Tween资源
        if (fillTween != null)
        {
            fillTween.Kill();
        }
    }

    private Sprite GetSprite(bool isDefaultValue)
    {
        Texture2D texture2D = new Texture2D(386, 120);

        Texture2D texture = AudioWaveTexture._instance.GetTexture(texture2D, isDefaultValue);
        Sprite currentSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
        return currentSprite;
    }
}