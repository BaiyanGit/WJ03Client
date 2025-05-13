using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TitleExtend : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform m_upTitle;

    public bool _isChoose;

    public void SetChoose(bool ischoose)
    {
        _isChoose = ischoose;
    }
    /// <summary>
    /// ≥ı ºªØ
    /// </summary>
    public void Init(Transform trans)
    {
        m_upTitle = trans as RectTransform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_upTitle.DOAnchorPosY(-70f, .5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isChoose)return;
        _isChoose = true;
        Invoke("WaitTime", 2f);
    }

    void WaitTime()
    {
        m_upTitle.DOAnchorPosY(0f, .5f);
        _isChoose = false;
    }
}
