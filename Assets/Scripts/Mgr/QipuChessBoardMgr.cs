using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QipuChessBoardMgr : BaseManager<QipuChessBoardMgr>
{
    QipuBtnPanel qipubtnPanel;
    RecommendMovesPanel recommendMovesPanel;

    private QipuchessBoard boardscript;
    public int boardsize = 0;
    public int index = -1;

    //个人棋局缓存
    public List<ChessManual> qijuList = new List<ChessManual>();
    //棋局路径缓存
    public List<string> filepaths = new List<string>();

    public QipuchessBoard GetChessBoard()
    {
        if (boardscript == null)
        {
            Transform transform = GameObject.Find("ChessBoardPanel").transform;
            GameObject obj = transform.Find("ChessBoard" + boardsize).gameObject;
            obj.SetActive(true);
            boardscript = obj.GetComponent<QipuchessBoard>();
        }
        return boardscript;
    }

    public QipuBtnPanel GetQipuBtnPanel()
    {
        if(qipubtnPanel == null)
        {
            qipubtnPanel = GameObject.Find("QipuBtnPanel").GetComponent<QipuBtnPanel>();
        }
        return qipubtnPanel;
    }

    public RecommendMovesPanel GetRecommendMovesPanel()
    {
        if(recommendMovesPanel == null)
        {
            recommendMovesPanel = GameObject.Find("RecommendMovesPanel").GetComponent<RecommendMovesPanel>();
        }
        return recommendMovesPanel;
    }

    public ChessManual GetChessManual()
    {
        return qijuList[index];
    }

    public string GetFilePath()
    {
        return filepaths[index];
    }

    public void Destory()
    {
        boardsize = 0;
        boardscript = null;
        index = -1;
    }
}
