using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Wx.Runtime
{
    //长按按钮
[AddComponentMenu("UI/LongClickButton")]
public class LongClickButton : Button
{
    [Serializable]
    public class LongClickEvent : UnityEvent
    {
    }

    [SerializeField] private LongClickEvent mOnLongClick = null;
    public LongClickEvent OnLongClick
    {
        get => mOnLongClick;
        set => mOnLongClick = value;
    }
    

    [Serializable]
    public class LongCancelEvent : UnityEvent
    {
    }

    //抬起
    [SerializeField] private LongCancelEvent mOnPointerUp = null;

    public LongCancelEvent OnPointUp
    {
        get => mOnPointerUp;
        set => mOnPointerUp = value;
    }

    // 长按需要的变量参数
    private bool _myIsStartPress = false;
    private float _myCurPointDownTime = 0f;
    [SerializeField] [Range(0, 5)] private float myLongPressTime = 0.6f;
    public BtnTypeEnum btnType;
    private bool _myLongPressTrigger = false;

    public bool longPressTrigger
    {
        get => _myLongPressTrigger;
        set => _myLongPressTrigger = value;
    }

    private void Update()
    {
        CheckIsLongPress();
    }

    /// <summary>
    /// 处理长按
    /// </summary>
    private void CheckIsLongPress()
    {
        if (!_myIsStartPress || _myLongPressTrigger) return;
        if (!(Time.time > _myCurPointDownTime + myLongPressTime)) return;
        _myLongPressTrigger = true;
        _myIsStartPress = false;
        mOnLongClick?.Invoke();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // 按下刷新当前时间
        base.OnPointerDown(eventData);
        _myCurPointDownTime = Time.time;
        _myIsStartPress = true;
        _myLongPressTrigger = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // 鼠标抬起，结束长按，调用抬起事件。
        base.OnPointerUp(eventData);
        _myIsStartPress = false;

        if (!_myLongPressTrigger) return;
        _myLongPressTrigger = false;
        mOnPointerUp.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标移出，结束长按、计时标志
        base.OnPointerExit(eventData);
        _myIsStartPress = false;
        _myLongPressTrigger = false;
    }
}
}