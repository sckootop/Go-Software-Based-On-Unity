using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaipuPausePanel : BasePanel
{
    public Button btnContinue;
    public Button btnRestart;
    public Button btnBackMain;

    private BaipuchessBoard boardscript;

    protected override void Init()
    {
        BaipuchessBoard boardscript = GameObject.Find("ChessBoard").GetComponent<BaipuchessBoard>();

        btnContinue.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<BaipuPausePanel>();
            boardscript.DelayCanPlay();
        });

        btnRestart.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<BaipuPausePanel>();
            boardscript.ResetBoard();
            boardscript.DelayCanPlay();
        });

        btnBackMain.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<BaipuPausePanel>();
            UIMgr.Instance.ShowPanel<MainPanel>();
            UIMgr.Instance.ShowPanel<PlayPanel>();
            SceneManager.LoadScene("UserScene");
        });
    }
}
