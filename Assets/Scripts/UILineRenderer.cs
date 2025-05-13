using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]//需要该组件才能生效
public class UILineRenderer : Graphic
{
    private List<Vector2> points = new List<Vector2>(); // 用于存储线条的点
    [SerializeField] private float lineWidth = 5f; // 线条宽度
    [SerializeField] private Color lineColor = Color.white; // 默认线条颜色

    // 每次需要重新绘制UI时调用
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); // 清空当前顶点数据

        // 如果没有足够的点，则不绘制任何东西
        if (points == null || points.Count < 2)
            return;

        // 遍历每个点，创建线段
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];

            // 计算垂直方向的法线，使线条有宽度
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * lineWidth / 2f;

            // 四个顶点（左下、左上、右上、右下）
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = lineColor; // 定义颜色

            // 左下
            vertex.position = new Vector3(start.x - perpendicular.x, start.y - perpendicular.y);
            vh.AddVert(vertex);

            // 左上
            vertex.position = new Vector3(start.x + perpendicular.x, start.y + perpendicular.y);
            vh.AddVert(vertex);

            // 右上
            vertex.position = new Vector3(end.x + perpendicular.x, end.y + perpendicular.y);
            vh.AddVert(vertex);

            // 右下
            vertex.position = new Vector3(end.x - perpendicular.x, end.y - perpendicular.y);
            vh.AddVert(vertex);

            // 添加两个三角形来组成矩形线条
            int index = vh.currentVertCount;
            vh.AddTriangle(index - 4, index - 3, index - 2);
            vh.AddTriangle(index - 4, index - 2, index - 1);
        }
    }

    /// <summary>
    /// 设置一个Ui元素
    /// 为什么要转换坐标?因为UI元素极可能不在同一个父物体下,存在错综复杂的父子关系
    /// 先获取UiElement世界坐标系转屏幕坐标系再转到此脚本所在的Ui坐标系
    /// </summary>
    /// <param name="uiElement"></param>
    public void AppendUIElement(RectTransform uiElement)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, // 当前 UILineRenderer 的 RectTransform
            RectTransformUtility.WorldToScreenPoint(null, uiElement.position), // UI 元素的世界坐标转换为屏幕坐标
            null,
            out localPoint // 输出的局部坐标
        );

        // 如果已经有两个点，则移除第二个点，以保持绘制最新线条
        if (points.Count == 2)
        {
            points.RemoveAt(1);
        }

        // 添加转换后的局部坐标到点列表中
        points.Add(localPoint);

        // 标记为需要重新绘制
        SetVerticesDirty();
    }

    /// <summary>
    /// 设置鼠标位置为第二个点,此时鼠标和第一个UiElement可以构成一条线
    /// </summary>
    /// <param name="point"></param>
    public void SetMouse()
    {
        if (points.Count == 2)
        {
            points.RemoveAt(1);
        }
        var mousePostion = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePostion, null, out Vector2 point);
        points.Add(point);
        SetVerticesDirty();
    }

    /// <summary>
    /// 设置线的颜色
    /// </summary>
    /// <param name="newColor"></param>
    public void SetLineColor(Color newColor)
    {
        lineColor = newColor;
        SetVerticesDirty();
    }

    /// <summary>
    /// 设置线的宽带
    /// </summary>
    /// <param name="width"></param>
    public void SetWidth(float width)
    {
        lineWidth = width;
        SetVerticesDirty();
    }

    /// <summary>
    /// 重置组件
    /// </summary>
    public void ResetSelf()
    {
        points.Clear();
        lineColor = Color.white;
        lineWidth = 5f;
        SetVerticesDirty();
    }
}