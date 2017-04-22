using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum E_TouchType:byte
{
    OnClick,
    OnDoubleClick,
    OnDown,
    OnUp,
    OnEnter,
    OnExit,
    OnSelect,
    OnUpdateSelect,
    OnDeSelect,
    OnDragBegin,
    OnDrag,
    OnDragEnd,
    OnDrop,
    OnScroll,
    OnMove,
}

public delegate void OnTouchEvent(GameObject listener,object args,params object [] _params);

public class TouchEvent
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public E_TouchType e_TouchType;
    /// <summary>
    /// 要添加的事件方法
    /// </summary>
    private event OnTouchEvent onTouchEvent = null;
    /// <summary>
    /// 需要的其他可变参数
    /// </summary>
    private object[] eventParams;

    public TouchEvent(OnTouchEvent touchEvent, params object[] _params)
    {
        AddEvent(touchEvent, _params);
    }

    public TouchEvent()
    {

    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="_event">要添加的事件</param>
    /// <param name="_params">需要的其他可变参数</param>
    public void AddEvent(OnTouchEvent _onTouchEvent, params object[] _params)
    {
        RemoveEvent();
        onTouchEvent += _onTouchEvent;
        eventParams = _params;
    }

    /// <summary>
    /// 通知监听者触发事件
    /// </summary>
    /// <param name="_listener">要通知的物体</param>
    /// <param name="_args"></param>
    public void CallTouchEvent(GameObject _listener,object _args)
    {
        if (null != onTouchEvent)
        {
            onTouchEvent(_listener, _args, eventParams);
        }
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    public void RemoveEvent()
    {
        if (null != onTouchEvent)
        {
            onTouchEvent -= onTouchEvent;
            onTouchEvent = null;
        }
    }
}

public class EventListener : MonoBehaviour,
    #region ----- interface 接口 -----
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler,

    ISelectHandler,
    IUpdateSelectedHandler,
    IDeselectHandler,

    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IScrollHandler,
    IMoveHandler
    #endregion
{
    private Dictionary<E_TouchType, TouchEvent> dicTouchEvnets = new Dictionary<E_TouchType, TouchEvent>(); 
    
    /// <summary>
    /// 获取或者添加监听者身上的EventListener组件
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static EventListener Get(GameObject go)
    {
        return go.GetOrAddComponent<EventListener>();
    }

    /// <summary>
    /// 获取对象身上的Transform组件
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static EventListener Get(Transform trans)
    {
        return trans.GetOrAddComponent<EventListener>();
    }

    /// <summary>
    /// 获取对象身上的Button组件
    /// </summary>
    /// <param name="btn"></param>
    /// <returns></returns>
    public static EventListener Get(Button btn)
    {
        return btn.gameObject.GetOrAddComponent<EventListener>();
    }
    void OnDestroy()
    {
        RemoveAllEvent();
    }

    /// <summary>
    /// 移除全部事件
    /// </summary>
    private void RemoveAllEvent()
    {
        foreach (var item in dicTouchEvnets)
        {
            item.Value.RemoveEvent();
        }
        dicTouchEvnets.Clear();
    }
    /// <summary>
    /// 从字典中获取事件
    /// </summary>
    /// <param name="_type">要获取的类型</param>
    /// <returns></returns>
    public TouchEvent GetEvent(E_TouchType _type)
    {
        TouchEvent _event;
        if (dicTouchEvnets.TryGetValue(_type, out _event))
        {
            return _event;
        }
        return null;
    }

    /// <summary>
    /// 监听者添加事件(使用之前先Get到监听者)
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_onTouchEvent"></param>
    /// <param name="_params"></param>
    public void AddEventListener(E_TouchType touchType, OnTouchEvent onTouchEvent, params object[] _params)
    {
        TouchEvent touchEvent= GetEvent(touchType);
        if (touchEvent == null)
        {
            touchEvent = new TouchEvent();
            dicTouchEvnets.Add(touchType, touchEvent);
        }
        dicTouchEvnets[touchType].e_TouchType = touchType;
        dicTouchEvnets[touchType].AddEvent(onTouchEvent, _params);
    }
    #region ----- OnPointerClick 点击事件(单击,双击) -----
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            TouchEvent touchEvent = GetEvent(E_TouchType.OnClick);
            if (touchEvent != null)
            {
                touchEvent.CallTouchEvent(this.gameObject,eventData);
            }
        }
        else if (eventData.clickCount == 2)
        {
            TouchEvent touchEvent = GetEvent(E_TouchType.OnDoubleClick);
            if (touchEvent != null)
            {
                touchEvent.CallTouchEvent(this.gameObject, eventData);
            }
        }
    }
    #endregion

    #region ----- OnPointerDown 点击事件(按下) -----
    public void OnPointerDown(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnDown);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnPointerUp 点击事件(抬起) -----
    public void OnPointerUp(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnUp);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnPointerEnter 点击事件(进入) -----
    public void OnPointerEnter(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnEnter);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnPointerExit 点击事件(移出) -----
    public void OnPointerExit(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnExit);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnSelect 选中事件(物体没选中) -----
    public void OnSelect(BaseEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnSelect);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnUpdateSelected 选中事件(物体没选中每帧调用) -----
    public void OnUpdateSelected(BaseEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnUpdateSelect);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnDeselect 选中事件(物体取消选中) -----
    public void OnDeselect(BaseEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnDeSelect);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnBeginDrag 拖拽事件(开始拖拽) -----
    public void OnBeginDrag(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnDragBegin);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnDrag 拖拽事件(拖拽进行中) -----
    public void OnDrag(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnDrag);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnEndDrag 拖拽事件(拖拽结束) -----
    public void OnEndDrag(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnDragEnd);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnDrop 拖拽事件(拖拽结束后的位置((鼠标)触摸位置))如果有物体，则那个物体调用 -----
    public void OnDrop(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnDrop);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnScroll 滑轮事件(滚动) -----
    public void OnScroll(PointerEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnScroll);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion

    #region ----- OnMove 移动事件(物体移动(与Input.GetAxis()中的Horizontal和Vertical想对应)，前提条件是选中) -----

    public void OnMove(AxisEventData eventData)
    {
        TouchEvent touchEvent = GetEvent(E_TouchType.OnMove);
        if (touchEvent != null)
        {
            touchEvent.CallTouchEvent(this.gameObject, eventData);
        }
    }
    #endregion
}
