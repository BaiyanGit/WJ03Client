using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 数据展示容器
/// </summary>
public class UIDataScreenContainer : MonoBehaviour
{
    /// <summary>
    /// 依附的容器索引
    /// </summary>
    public int TargetIndex;
    /// <summary>
    /// 标题
    /// </summary>
    public TMP_Text Title;
    /// <summary>
    /// 滑动区域
    /// </summary>
    public ScrollRect ScrollRect;
    /// <summary>
    /// 锚点小
    /// </summary>
    public Vector2 AnchorsMin;
    /// <summary>
    /// 锚点大
    /// </summary>
    public Vector2 AnchorsMax;
    /// <summary>
    /// 偏移小
    /// </summary>
    public Vector2 OffsetMin;
    /// <summary>
    /// 偏移大
    /// </summary>
    public Vector3 OffsetMax;
    /// <summary>
    /// 背景图
    /// </summary>
    public Sprite Background;
    /// <summary>
    /// 刷新速率（秒）
    /// </summary>
    public float RefreshRatio;
    /// <summary>
    /// 元素列表
    /// </summary>
    public List<UIDataScreenElement> ElementList;

    /// <summary>
    /// 辅助计时工具
    /// </summary>
    private float timeHelper;

    private void Start()
    {
        ElementList = new();

        for (int i = 0; i < ElementList.Count; i++)
        {
            UIDataScreenElement component;
            switch (ElementList[i].EType)
            {
                default:
                    var temp = Resources.Load("") as GameObject;
                    temp = Instantiate(null) as GameObject;
                    temp.transform.SetParent(transform, false);
                    temp.transform.localScale = Vector2.one;
                    component = temp.AddComponent<UIDataScreenElement>();
                    break;
            }

            ElementList.Add(component);
        }
    }

    private void LateUpdate()
    {
        if (timeHelper >= RefreshRatio)
        {
            timeHelper = 0f;
        }

        for (int i = 0; i < ElementList.Count; i++)
        {
            ElementList[i].ChangeValue(0.5f);
        }

        timeHelper += Time.deltaTime;
    }
}