using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QipuPausePanel : BasePanel
{
    public Button btnContinue;
    public Button btnBackMain;
    public Button btnRestart;

    protected override void Init()
    {
        btnContinue.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<QipuPausePanel>();
            QipuChessBoardMgr.Instance.GetChessBoard().canplay = true;
        });

        btnBackMain.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<QipuPausePanel>();
            UIMgr.Instance.ShowPanel<MainPanel>();
            GoPanel panel = UIMgr.Instance.ShowPanel<GoPanel>();
            panel.ShowPanel(PanelName.PersonalGameEntrancesPanel);
            SceneManager.LoadScene("UserScene");
        });

        btnRestart.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<QipuPausePanel>();
            QipuChessBoardMgr.Instance.GetChessBoard().CleanBoard();
            QipuChessBoardMgr.Instance.GetChessBoard().ResetBoard();
            QipuChessBoardMgr.Instance.GetChessBoard().canplay = true;
        });
    }
}
