using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LocalPausePanel : BasePanel
{
    public Button btnContinue;
    public Button btnHuiQi;
    public Button btnStopStep;
    public Button btnRestart;
    public Button btnBackMain;

    private LocalchessBoard board;
    protected override void Init()
    {
        board = LocalgameChessBoardMgr.Instance.GetCurrentBoard();
        btnContinue.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<LocalPausePanel>();
            board.DelayCanPlay();
        });

        btnHuiQi.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<LocalPausePanel>();
            board.HuiQi();
        });

        btnStopStep.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<LocalPausePanel>();
            board.StopOneStep();
        });

        btnRestart.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<LocalPausePanel>();
            board.ResetBoard();
        });

        btnBackMain.onClick.AddListener(() =>
        {
            LocalgameChessBoardMgr.Instance.curboardSize = 0;
            UIMgr.Instance.HidePanel<LocalPausePanel>();
            UIMgr.Instance.HidePanel<GameResultPanel>();
            UIMgr.Instance.ShowPanel<MainPanel>();
            UIMgr.Instance.ShowPanel<PlayPanel>();
            SceneManager.LoadScene("UserScene");
        });
    }
}
