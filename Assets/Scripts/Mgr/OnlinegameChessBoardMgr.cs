using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinegameChessBoardMgr : BaseManager<OnlinegameChessBoardMgr>
{
    public int boardsize = 0;
    private OnlinechessBoard boardscript;
    private OnlinePlayerPanel playerpanel;
    public bool readytoLoad = false;
    public OnlinechessBoard GetCurrentBoard()
    {
        if(boardscript == null)
        {
            boardscript = GameObject.Find("ChessBoard").GetComponent<OnlinechessBoard>();
            Debug.Log("Õ¯¬Á∆Â≈Ãº”‘ÿÕÍ≥…");
        }
        return boardscript;
    }

    public OnlinePlayerPanel GetOnlinePlayerPanel()
    {
        if(playerpanel == null)
        {
            playerpanel=GameObject.Find("OnlinePlayerPanel").GetComponent<OnlinePlayerPanel>();
        }
        return playerpanel;
    }
}
