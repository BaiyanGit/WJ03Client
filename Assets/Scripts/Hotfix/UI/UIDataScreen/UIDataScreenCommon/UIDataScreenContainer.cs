using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����չʾ����
/// </summary>
public class UIDataScreenContainer : MonoBehaviour
{
    /// <summary>
    /// ��������������
    /// </summary>
    public int TargetIndex;
    /// <summary>
    /// ����
    /// </summary>
    public TMP_Text Title;
    /// <summary>
    /// ��������
    /// </summary>
    public ScrollRect ScrollRect;
    /// <summary>
    /// ê��С
    /// </summary>
    public Vector2 AnchorsMin;
    /// <summary>
    /// ê���
    /// </summary>
    public Vector2 AnchorsMax;
    /// <summary>
    /// ƫ��С
    /// </summary>
    public Vector2 OffsetMin;
    /// <summary>
    /// ƫ�ƴ�
    /// </summary>
    public Vector3 OffsetMax;
    /// <summary>
    /// ����ͼ
    /// </summary>
    public Sprite Background;
    /// <summary>
    /// ˢ�����ʣ��룩
    /// </summary>
    public float RefreshRatio;
    /// <summary>
    /// Ԫ���б�
    /// </summary>
    public List<UIDataScreenElement> ElementList;

    /// <summary>
    /// ������ʱ����
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