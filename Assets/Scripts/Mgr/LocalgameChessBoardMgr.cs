using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalgameChessBoardMgr :BaseManager<LocalgameChessBoardMgr>
{
    private Dictionary<int, LocalchessBoard> boards = new Dictionary<int, LocalchessBoard>{
        {9, null},
        {13, null},
        {19, null}
    };
    public int curboardSize = 0;

    GameObject boardPanel;

    public LocalchessBoard GetChessBoard(int size)
    {
        if (boardPanel == null)
        {
            boardPanel = GameObject.Find("ChessBoardPanel");
        }

        if (boards[size] == null)
        {
            GameObject obj = boardPanel.transform.Find("ChessBoard" + size).gameObject;
            boards[size] = obj.GetComponent<LocalchessBoard>();
        }
        return boards[size];
    }

    public LocalchessBoard GetCurrentBoard()
    {
        return GetChessBoard(curboardSize);
    }

    public void ShowChessBoard(int size)
    {
        if (curboardSize == size) return;
        if (boards[size] == null)
        {
            GetChessBoard(size).gameObject.SetActive(true);
        }
        curboardSize = size;
        switch (size)
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
        if (boards[size].gameObject.activeSelf == true)
        {
            boards[size].gameObject.SetActive(false);
        }
    }
}
