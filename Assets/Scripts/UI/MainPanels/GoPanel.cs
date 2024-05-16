using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoPanel : BasePanel
{
    public Text txtTiku;
    public Text txtRecognize;
    public Text txtMyQipu;

    protected override void Init()
    {
        EventTrigger trigger1 = txtTiku.GetComponent<EventTrigger>();
        EventTrigger trigger2 = txtRecognize.GetComponent<EventTrigger>();
        EventTrigger trigger3 = txtMyQipu.GetComponent<EventTrigger>();

        EventTrigger.Entry en = new EventTrigger.Entry();
        en.eventID = EventTriggerType.PointerUp;
        en.callback.AddListener((BaseEventData data)=>
        {
            UIMgr.Instance.HidePanel<RecognizeImagePanel>();
            UIMgr.Instance.HidePanel<PersonalGameEntrancesPanel>();
            UIMgr.Instance.ShowPanel<TikuEntrancesPanel>();
            txtTiku.color = new Color32(230, 195, 142, 255);
            txtRecognize.color = Color.white;
            txtMyQipu.color = Color.white;
        });
        trigger1.triggers.Add(en);

        en = new EventTrigger.Entry();
        en.eventID = EventTriggerType.PointerUp;
        en.callback.AddListener((BaseEventData data) =>
        {
            UIMgr.Instance.HidePanel<TikuEntrancesPanel>();
            UIMgr.Instance.HidePanel<PersonalGameEntrancesPanel>();
            UIMgr.Instance.ShowPanel<RecognizeImagePanel>();
            txtTiku.color = Color.white;
            txtMyQipu.color = Color.white;
            txtRecognize.color = new Color32(230, 195, 142, 255);
        });
        trigger2.triggers.Add(en);

        en = new EventTrigger.Entry();
        en.eventID = EventTriggerType.PointerUp;
        en.callback.AddListener((BaseEventData data) =>
        {
            UIMgr.Instance.HidePanel<RecognizeImagePanel>();
            UIMgr.Instance.HidePanel<TikuEntrancesPanel>();
            UIMgr.Instance.ShowPanel<PersonalGameEntrancesPanel>();
            txtRecognize.color = Color.white;
            txtTiku.color = Color.white;
            txtMyQipu.color = new Color32(230, 195, 142, 255);
        });
        trigger3.triggers.Add(en);     
    }

    public void ShowPanel(PanelName name)
    {
        switch(name)
        {
            case PanelName.TikuEntrancesPanel:
                UIMgr.Instance.ShowPanel<TikuEntrancesPanel>();
                txtTiku.color = new Color32(230, 195, 142, 255);
                break;
            case PanelName.RecognizeImagePanel:
                UIMgr.Instance.ShowPanel<RecognizeImagePanel>();
                txtRecognize.color = new Color32(230, 195, 142, 255);
                break;
            case PanelName.PersonalGameEntrancesPanel:
                UIMgr.Instance.ShowPanel<PersonalGameEntrancesPanel>();
                txtMyQipu.color = new Color32(230, 195, 142, 255);
                break;
        }
    }
}

public enum PanelName
{
    TikuEntrancesPanel,
    RecognizeImagePanel,
    PersonalGameEntrancesPanel
}
