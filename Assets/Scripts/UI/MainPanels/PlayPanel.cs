using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayPanel : BasePanel
{
    public Button btnLocal;
    public Button btnOnline;

    protected override void Init()
    {
        btnOnline.onClick.AddListener(() =>
        {
            UIMgr.Instance.ShowPanel<OnlineGameEntrancePanel>();
            UIMgr.Instance.HidePanel<PlayPanel>();
        });

        btnLocal.onClick.AddListener(() =>
        {
            UIMgr.Instance.ShowPanel<LocalGameEntrancePanel>();
            UIMgr.Instance.HidePanel<PlayPanel>();
        }); 
    }
}
