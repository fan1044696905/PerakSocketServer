using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEventTriggerListener : MonoBehaviour,
    IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler,

    ISelectHandler,
    IUpdateSelectedHandler,
    IDeselectHandler,

    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public delegate void OnEventTouchHandler();

    public event OnEventTouchHandler onClick;
    public event OnEventTouchHandler onDoubleClick;
    public event OnEventTouchHandler onDown;
    public event OnEventTouchHandler onUp;
    public event OnEventTouchHandler onSelect;
    public event OnEventTouchHandler onUpdateSelect;
    public event OnEventTouchHandler onDeSelect;
    public event OnEventTouchHandler onBeginDrag;
    public event OnEventTouchHandler onDrag;
    public event OnEventTouchHandler onEndDrag;

    public static UIEventTriggerListener Get(GameObject go)
    {
        return go.GetOrAddComponent<UIEventTriggerListener>();
    }
    public static UIEventTriggerListener Get(Transform trans)
    {
        return trans.GetOrAddComponent<UIEventTriggerListener>();
    }
    public static UIEventTriggerListener Get(Button btn)
    {
        return btn.transform.GetOrAddComponent<UIEventTriggerListener>();
    }

    void OnDestroy()
    {
        RemoveAllHander();
    }

    private void RemoveAllHander()
    {
        RemoveHander(onClick);
        RemoveHander(onDoubleClick);
        RemoveHander(onDown);
        RemoveHander(onUp);
        RemoveHander(onSelect);
        RemoveHander(onUpdateSelect);
        RemoveHander(onDeSelect);
        RemoveHander(onBeginDrag);
        RemoveHander(onDrag);
        RemoveHander(onEndDrag);
    }

    private void RemoveHander(OnEventTouchHandler touchHandler)
    {
        if (null!=touchHandler)
        {
            touchHandler -= touchHandler;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            if (onClick != null)
            {
                OnButtonClickPlaySound();
                onClick();
            }
            
        }
        else if(eventData.clickCount ==2)
        {
            if (onDoubleClick != null)
            {
                onDoubleClick();
            }
            
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null)
        {
            onDown();
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null)
        {
            onUp();
        }
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null)
        {
            onSelect();
        }
        
    }

    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null)
        {
            onUpdateSelect();
        }
        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (onDeSelect != null)
        {
            onDeSelect();
        }
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onDown != null)
        {
            onBeginDrag();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null)
        {
            onDrag();
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null)
        {
            onEndDrag();
        }
        
    }

    /// <summary>
    /// 点击按钮默认播放普通按钮点击音效，如果无需播放或者更换音效则重写该方法，重写覆盖基类
    /// </summary>
    /// <param name="e_PlayMusic"></param>
    public virtual void OnButtonClickPlaySound(E_PlayMusic e_PlayMusic = E_PlayMusic.普通按钮点击)
    {
        SoundManager.Instance.PlaySound(e_PlayMusic);
    }
}
