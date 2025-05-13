using System;

using UnityEngine;
using UnityEngine.EventSystems;

public class DragHelper : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public Action BeginDrag { get; set; }
    public Action EndDrag { get; set; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDrag?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag?.Invoke();
    }

}
