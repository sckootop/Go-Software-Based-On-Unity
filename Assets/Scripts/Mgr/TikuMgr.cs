using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TikuMgr : BaseManager<TikuMgr>
{
    //管理三个size的棋盘
    private Dictionary<int, TikuchessBoard> boards=new Dictionary<int, TikuchessBoard>{
        {9, null},
        {13, null},
        {19, null}
    };//三个棋盘一开始都为失活状态
    public int curboardSize = 0;

    //题库相关
    public Dictionary<string, List<GameTree>> tikuDic = new Dictionary<string, List<GameTree>>();
    public string curTiku = "吃子练习";

    //题库面板
    TikuInfoPanel tikuInfoPanel;

    //Panel
    GameObject boardPanel;

    public TikuchessBoard GetChessBoard(int size)
    {
        if(boardPanel==null)
        {
            boardPanel = GameObject.Find("ChessBoardPanel");
        }

        if (boards[size] == null)
        {
            GameObject obj = boardPanel.transform.Find("ChessBoard" + size).gameObject;
            boards[size] = obj.GetComponent<TikuchessBoard>();
        }
        return boards[size];
    }

    public TikuchessBoard GetCurrentBoard()
    {
        return GetChessBoard(curboardSize);
    }

    public TikuInfoPanel GetTikuInfoPanel()
    {
        if(tikuInfoPanel == null)
        {
            tikuInfoPanel = GameObject.Find("TikuInfoPanel").GetComponent<TikuInfoPanel>();
        }
        return tikuInfoPanel;
    }

    public void ShowChessBoard(int size)
    {
        if (curboardSize == size) return;//已经是显示状态了
        if (boards[size]==null)
        {
            GetChessBoard(size).gameObject.SetActive(true);
        }
        curboardSize = size;
        switch(size)
        {
            case 9:
                HideChessBoard(13);
                HideChessBoard(19);
                break;
            case 13:
                HideChessBoard(9);
                HideChessBoard(19);
                break;
            case 19:
                HideChessBoard(9);
                HideChessBoard(13);
                break;
        }
        boards[size].gameObject.SetActive(true);
    }

    private void HideChessBoard(int size)
    {
        if (boards[size] == null)
        {
            GetChessBoard(size);
        }
        if (boards[size].gameObject.activeSelf==true)
        {
            boards[size].gameObject.SetActive(false);
        } 
    }
}
