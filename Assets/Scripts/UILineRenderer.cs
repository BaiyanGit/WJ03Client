using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]//��Ҫ�����������Ч
public class UILineRenderer : Graphic
{
    private List<Vector2> points = new List<Vector2>(); // ���ڴ洢�����ĵ�
    [SerializeField] private float lineWidth = 5f; // �������
    [SerializeField] private Color lineColor = Color.white; // Ĭ��������ɫ

    // ÿ����Ҫ���»���UIʱ����
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); // ��յ�ǰ��������

        // ���û���㹻�ĵ㣬�򲻻����κζ���
        if (points == null || points.Count < 2)
            return;

        // ����ÿ���㣬�����߶�
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];

            // ���㴹ֱ����ķ��ߣ�ʹ�����п��
            Vector2 direction = (end - start).normalized;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x) * lineWidth / 2f;

            // �ĸ����㣨���¡����ϡ����ϡ����£�
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = lineColor; // ������ɫ

            // ����
            vertex.position = new Vector3(start.x - perpendicular.x, start.y - perpendicular.y);
            vh.AddVert(vertex);

            // ����
            vertex.position = new Vector3(start.x + perpendicular.x, start.y + perpendicular.y);
            vh.AddVert(vertex);

            // ����
            vertex.position = new Vector3(end.x + perpendicular.x, end.y + perpendicular.y);
            vh.AddVert(vertex);

            // ����
            vertex.position = new Vector3(end.x - perpendicular.x, end.y - perpendicular.y);
            vh.AddVert(vertex);

            // �����������������ɾ�������
            int index = vh.currentVertCount;
            vh.AddTriangle(index - 4, index - 3, index - 2);
            vh.AddTriangle(index - 4, index - 2, index - 1);
        }
    }

    /// <summary>
    /// ����һ��UiԪ��
    /// ΪʲôҪת������?��ΪUIԪ�ؼ����ܲ���ͬһ����������,���ڴ��۸��ӵĸ��ӹ�ϵ
    /// �Ȼ�ȡUiElement��������ϵת��Ļ����ϵ��ת���˽ű����ڵ�Ui����ϵ
    /// </summary>
    /// <param name="uiElement"></param>
    public void AppendUIElement(RectTransform uiElement)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, // ��ǰ UILineRenderer �� RectTransform
            RectTransformUtility.WorldToScreenPoint(null, uiElement.position), // UI Ԫ�ص���������ת��Ϊ��Ļ����
            null,
            out localPoint // ����ľֲ�����
        );

        // ����Ѿ��������㣬���Ƴ��ڶ����㣬�Ա��ֻ�����������
        if (points.Count == 2)
        {
            points.RemoveAt(1);
        }

        // ���ת����ľֲ����굽���б���
        points.Add(localPoint);

        // ���Ϊ��Ҫ���»���
        SetVerticesDirty();
    }

    /// <summary>
    /// �������λ��Ϊ�ڶ�����,��ʱ���͵�һ��UiElement���Թ���һ����
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
    /// �����ߵ���ɫ
    /// </summary>
    /// <param name="newColor"></param>
    public void SetLineColor(Color newColor)
    {
        lineColor = newColor;
        SetVerticesDirty();
    }

    /// <summary>
    /// �����ߵĿ��
    /// </summary>
    /// <param name="width"></param>
    public void SetWidth(float width)
    {
        lineWidth = width;
        SetVerticesDirty();
    }

    /// <summary>
    /// �������
    /// </summary>
    public void ResetSelf()
    {
        points.Clear();
        lineColor = Color.white;
        lineWidth = 5f;
        SetVerticesDirty();
    }
}