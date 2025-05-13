using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// �������ݿ���
/// </summary>
public class VoiceDataItem : MonoBehaviour
{
    Button currentButton;
    Image currentImage;
    private float fillDuration = 5f;

    // ����Tween�����Ա����
    private Tween fillTween;

    public bool _isDefaultValue;

    public int _id;

    /// <summary>
    /// ��ʼ��
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
        // ������ж��������У�����ֹ
        if (fillTween != null && fillTween.IsActive())
        {
            fillTween.Kill();
        }

        // ���ð�ť��ֹ�ظ����
        currentButton.interactable = false;
        currentImage.sprite = GetSprite(_isDefaultValue);

        // ʹ��DOTween��������FillAmount
        fillTween = currentImage.DOFillAmount(1f, fillDuration)
            .SetEase(Ease.Linear) // ������䣨���滻Ϊ�����������ͣ�
            .OnComplete(() =>
            {
                // ������ɺ�Ļص�
                Debug.Log("�����ɣ�");
                // ���ð�ť
                currentButton.interactable = true;
                AudioWaveTexture._instance.SetDefaultTexture();
            });
    }

    private void OnDestroy()
    {
        // ȷ������ʱ�ͷ�Tween��Դ
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