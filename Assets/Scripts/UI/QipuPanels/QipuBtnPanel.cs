using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QipuBtnPanel : MonoBehaviour
{
    public Button btnBack1;
    public Button btnBack5;
    public Button btnFront1;
    public Button btnFront5;
    public Button btnAutoPlay;
    public Button btnAI;
    public Button btnTry;

    private Image imgAI;
    private Image imgTry;

    private Coroutine routine;

    // Start is called before the first frame update
    void Start()
    {
        QipuChessBoardMgr.Instance.GetChessBoard();
        imgTry = btnTry.GetComponentInChildren<Image>();
        imgAI = btnAI.GetComponentInChildren<Image>();

        btnBack1.onClick.AddListener(() =>
        {
            if (routine != null) StopCoroutine(routine);
            QipuChessBoardMgr.Instance.GetChessBoard().Play(-1);
            QipuChessBoardMgr.Instance.GetChessBoard().ClearWinrateTextObjs();
        });

        btnBack5.onClick.AddListener(() =>
        {
            if (routine != null) StopCoroutine(routine);
            QipuChessBoardMgr.Instance.GetChessBoard().Play(-5);
            QipuChessBoardMgr.Instance.GetChessBoard().ClearWinrateTextObjs();
        });

        btnFront1.onClick.AddListener(() =>
        {
            if (routine != null) StopCoroutine(routine);
            QipuChessBoardMgr.Instance.GetChessBoard().Play(1);
            QipuChessBoardMgr.Instance.GetChessBoard().ClearWinrateTextObjs();
        });

        btnFront5.onClick.AddListener(() =>
        {
            if(routine != null) StopCoroutine(routine);
            QipuChessBoardMgr.Instance.GetChessBoard().Play(5);
            QipuChessBoardMgr.Instance.GetChessBoard().ClearWinrateTextObjs();
        });

        btnAutoPlay.onClick.AddListener(() =>
        {
            QipuChessBoardMgr.Instance.GetChessBoard().ClearWinrateTextObjs();
            if (routine == null)
                routine = StartCoroutine(QipuChessBoardMgr.Instance.GetChessBoard().AutoPlay());
        });

        btnAI.onClick.AddListener(() =>
        {
            if (QipuChessBoardMgr.Instance.GetChessBoard().isTry == true) return;//try状态不让分析

            if (routine != null) StopCoroutine(routine);

            QipuChessBoardMgr.Instance.GetChessBoard().ClearWinrateTextObjs();
            QipuChessBoardMgr.Instance.GetChessBoard().CleanBoard();
            QipuChessBoardMgr.Instance.GetChessBoard().AIAnalyze();
        });

        btnTry.onClick.AddListener(() =>
        {
            if (routine != null) StopCoroutine(routine);
            if (QipuChessBoardMgr.Instance.GetChessBoard().isTry == true)
            {
                //修改btnAI的颜色和enabled为true
                imgAI.color = Color.white;
                btnAI.enabled = true;

                QipuChessBoardMgr.Instance.GetRecommendMovesPanel().Clear();

                QipuChessBoardMgr.Instance.GetChessBoard().ClearStepTextObjs();
                QipuChessBoardMgr.Instance.GetChessBoard().CleanBoard();
                QipuChessBoardMgr.Instance.GetChessBoard().GenerateStepTextObj();
                QipuChessBoardMgr.Instance.GetChessBoard().isTry = false;
                imgTry.color = Color.white;
            }
            else if (QipuChessBoardMgr.Instance.GetChessBoard().isTry == false)
            {
                //修改btnAI的颜色和enabled为false
                imgAI.color = new Color(0.3f, 0.3f, 0.3f);
                btnAI.enabled = false;

                QipuChessBoardMgr.Instance.GetChessBoard().isTry = true;
                QipuChessBoardMgr.Instance.GetChessBoard().ClearWinrateTextObjs();
                QipuChessBoardMgr.Instance.GetChessBoard().CleanBoard();
                QipuChessBoardMgr.Instance.GetChessBoard().InitPredictBoard();
                imgTry.color = new Color32(230, 195, 142, 255);
            }
        });
    }

    public void EnterTryCondition()
    {
        imgAI.color = new Color(0.3f, 0.3f, 0.3f);
        btnAI.enabled = false;
        imgTry.color = new Color32(230, 195, 142, 255);
    }
}
