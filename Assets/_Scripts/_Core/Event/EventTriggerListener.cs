using UnityEngine;
using UnityEngine.EventSystems;
public class EventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
    private const float CLICK_INTERVAL_TIME = 0.1f; //const click interval time
    private const float CLICK_INTERVAL_POS = 2; //const click interval pos

    public delegate void PointerEventDelegate(PointerEventData eventData, GameObject go);
    public delegate void BaseEventDelegate(BaseEventData eventData, GameObject go);
    public delegate void AxisEventDelegate(AxisEventData eventData, GameObject go);


    public BaseEventDelegate onDeselect = null;
    public PointerEventDelegate onBeginDrag = null;
    public PointerEventDelegate onDrag = null;
    public PointerEventDelegate onEndDrag = null;
    public PointerEventDelegate onDrop = null;
    public AxisEventDelegate onMove = null;
    public PointerEventDelegate onClick = null;
    public PointerEventDelegate onDown = null;
    public PointerEventDelegate onEnter = null;
    public PointerEventDelegate onExit = null;
    public PointerEventDelegate onUp = null;
    public PointerEventDelegate onScroll = null;
    public BaseEventDelegate onSelect = null;
    public BaseEventDelegate onUpdateSelect = null;
    public BaseEventDelegate onSubmit = null;

    //public static EventTriggerListener Get(Transform go)
    //{
    //    return Get(go);
    //}

    //public static EventTriggerListener Get(Transform go)
    //{
    //    return Get(go.gameObject);
    //}

    public static EventTriggerListener Get(MonoBehaviour go)
    {
        return Get(go.gameObject);
    }

    public static EventTriggerListener Get(MonoBehaviour go, string args)
    {
        return Get(go.gameObject, args);
    }

    public static EventTriggerListener Get(GameObject go)
    {
        return Get(go, "");
    }

    public static EventTriggerListener Get(GameObject go, string args)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();

        return listener;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null) onDeselect(eventData, gameObject);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(eventData, gameObject);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(eventData, gameObject);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(eventData, gameObject);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null) onDrop(eventData, gameObject);
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (onMove != null) onMove(eventData, gameObject);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(eventData, gameObject);
        }
    }


    public override void OnPointerDown(PointerEventData eventData)
    {

        if (onDown != null) onDown(eventData, gameObject);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(eventData, gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(eventData, gameObject);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(eventData, gameObject);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null) onScroll(eventData, gameObject);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(eventData, gameObject);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(eventData, gameObject);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        if (onSubmit != null) onSubmit(eventData, gameObject);
    }
}
