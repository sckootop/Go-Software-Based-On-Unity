using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PVEPausePanel : BasePanel
{
    public Button btnContinue;
    public Button btnConcede;
    public Button btnBackMain;

    private PVEchessBoard board;

    protected override void Init()
    {
        if (board == null)
        {
            board = GameObject.Find("ChessBoard").GetComponent<PVEchessBoard>();
        }

        btnContinue.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<PVEPausePanel>();
            board.DelayCanPlay();
        });

        btnConcede.onClick.AddListener(() =>
        {
            board.Concede();
            UIMgr.Instance.HidePanel<PVEPausePanel>();
        });

        btnBackMain.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<PVEPausePanel>();
            UIMgr.Instance.HidePanel<GameResultPanel>();
            UIMgr.Instance.ShowPanel<MainPanel>();
            UIMgr.Instance.ShowPanel<PlayPanel>();
            SceneManager.LoadScene("UserScene");
        });
    }
}
