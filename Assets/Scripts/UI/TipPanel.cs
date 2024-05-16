using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

public class TipPanel : BasePanel, IPointerClickHandler
{
    public TextMeshProUGUI txt;
    protected override void Init()
    {
        //Invoke("AutoHide", 2f);
    }

    private void AutoHide()
    {
        UIMgr.Instance.HidePanel<TipPanel>();
    }

    public void InitText(string str)
    {
        txt.text = str;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AutoHide();        
    }
}
