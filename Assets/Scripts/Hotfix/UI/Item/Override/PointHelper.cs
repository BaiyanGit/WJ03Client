using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class PointHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Action PointerEnter { get; set; }
    public Action PointerExit { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit?.Invoke();
    }
}
