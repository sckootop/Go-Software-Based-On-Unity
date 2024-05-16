using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlinePausePanel : BasePanel
{
    public Button btnContinue;
    public Button btnConcede;
    public Button btnStopStep;
    public Button btnBackMain;

    private OnlinechessBoard board;
    protected override void Init()
    {
        board = OnlinegameChessBoardMgr.Instance.GetCurrentBoard();

        if(GameDataMgr.Instance.turn == GameDataMgr.Instance.isWhite)//我方行棋回合
        {
            //显示停一手
            btnStopStep.transform.parent.Find("蒙版").gameObject.SetActive(false);
        }

        if(board.isgamefinish == true)
        {
            //显示返回面板
            btnBackMain.transform.parent.Find("蒙版").gameObject.SetActive(false);
        }

        btnContinue.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<OnlinePausePanel>();
            board.DelayCanPlay();
        });

        btnConcede.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<OnlinePausePanel>();
            board.Concede();
        });

        btnStopStep.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<OnlinePausePanel>();
            board.StopOneStep();
        });

        btnBackMain.onClick.AddListener(() =>
        {         
            UIMgr.Instance.HideAll();
            UIMgr.Instance.ShowPanel<MainPanel>();
            UIMgr.Instance.ShowPanel<PlayPanel>();
            SceneManager.LoadScene("UserScene");
        });
    }
}
