using UnityEngine;

/// <summary>
/// ����չʾ��Ԫ��
/// </summary>
public class UIDataScreenElement : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    public string Title;
    /// <summary>
    /// �������
    /// </summary>
    public UIDataScreenElementComponentType EType;
    /// <summary>
    /// ��ֵ����
    /// </summary>
    public readonly float Ratio;
    /// <summary>
    /// ����ͼ
    /// </summary>
    public Sprite BackgroundSprite;
    /// <summary>
    /// ǰ��ͼ
    /// </summary>
    public Sprite ForgroundSprite;

    /// <summary>
    /// �ı�ˢ����ֵ
    /// </summary>
    /// <param name="value"></param>
    public virtual void ChangeValue(float value)
    {

    }
}

public enum UIDataScreenElementComponentType
{
    None,
}