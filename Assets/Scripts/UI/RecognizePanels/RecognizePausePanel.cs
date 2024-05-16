using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RecognizePausePanel : BasePanel
{
    public Button btnContinue;
    public Button btnBackMain;

    private RecognizechessBoard board;
    protected override void Init()
    {
        board = RecognizeImageMgr.Instance.GetCurrentBoard();

        btnContinue.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel<RecognizePausePanel>();
            board.DelayCanPlay();
        });

        btnBackMain.onClick.AddListener(() =>
        {
            RecognizeImageMgr.Instance.curboardsize = 0;
            UIMgr.Instance.HidePanel<RecognizePausePanel>();
            UIMgr.Instance.ShowPanel<MainPanel>();
            UIMgr.Instance.ShowPanel<PlayPanel>();
            SceneManager.LoadScene("UserScene");
        });
    }
}
