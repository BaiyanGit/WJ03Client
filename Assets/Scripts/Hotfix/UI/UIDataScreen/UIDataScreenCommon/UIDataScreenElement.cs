using UnityEngine;

/// <summary>
/// 数据展示的元素
/// </summary>
public class UIDataScreenElement : MonoBehaviour
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title;
    /// <summary>
    /// 组件类型
    /// </summary>
    public UIDataScreenElementComponentType EType;
    /// <summary>
    /// 数值比率
    /// </summary>
    public readonly float Ratio;
    /// <summary>
    /// 背景图
    /// </summary>
    public Sprite BackgroundSprite;
    /// <summary>
    /// 前景图
    /// </summary>
    public Sprite ForgroundSprite;

    /// <summary>
    /// 改变刷新数值
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