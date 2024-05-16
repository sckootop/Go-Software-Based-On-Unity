using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameResultPanel : BasePanel
{
    public Button btnshowpanel;
    public Text txtResult;
    public Text txtReason;
    public UnityAction action;

    protected override void Init()
    {
        btnshowpanel.onClick.AddListener(() =>
        {
            action.Invoke();
        });
    }

    public void ChangeTxtInfo(string result, string reason)
    {
        txtResult.text = result;
        txtReason.text = reason;
    }

    private void OnDestroy()
    {
        action = null;//±ÜÃâÄÚ´æĞ¹Â¶
    }
}
