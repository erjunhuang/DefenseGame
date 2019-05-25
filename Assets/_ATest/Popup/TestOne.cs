using UnityEngine;
using Core.Popup;
using UnityEngine.EventSystems;
using QGame.Core.Event;

public class TestOne : Panel
{
    protected override void OnAwake()
    {
        base.OnAwake();

        EventTriggerListener.Get(this.gameObject).onClick = OnClickBtn;
        //XEventBus.Instance.Register(EventId.AboveReturnClick, UpdateGold);
    }

    protected override void OnStart()
    {
        base.OnStart();
    }
    protected override void OnLoadData()
    {
        base.OnLoadData();
    }
     
    protected override void OnUpdate(float delatTime)
    {
        base.OnUpdate(delatTime);
    }

    protected override void OnClose()
    {
        base.OnClose();
        //XEventBus.Instance.UnRegister(EventId.AboveReturnClick, UpdateGold);
    }

    public void OnClickBtn(PointerEventData eventdata, GameObject go)
    {
        Close();
    }
    private void UpdateGold(XEventArgs xEventArgs)
    {
        int gold = (int)xEventArgs.GetData<int>();
        Debug.Log("TestOne UpdateGold:" + gold);
    }
}
