using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaipuBtnPanel : MonoBehaviour
{
    public Button btnTurn;
    public Button btnBlack;
    public Button btnWhite;
    public Button btnDelete;
    public Button btnImport;
    public Button btnExport;

    // Start is called before the first frame update
    void Start()
    {
        btnTurn.onClick.AddListener(() =>
        {
            BaipuchessBoard board = BaiPuMgr.Instance.GetCurrentBoard();
            board.isWhite = !board.isWhite;
            board.turn = true;
        });

        btnBlack.onClick.AddListener(() =>
        {
            BaipuchessBoard board = BaiPuMgr.Instance.GetCurrentBoard();
            board.isWhite = false;
            board.turn = false;
        });

        btnWhite.onClick.AddListener(() =>
        {
            BaipuchessBoard board = BaiPuMgr.Instance.GetCurrentBoard();
            board.isWhite = true;
            board.turn = false;
        });

        btnDelete.onClick.AddListener(() =>
        {
            BaipuchessBoard board = BaiPuMgr.Instance.GetCurrentBoard();
            board.ResetBoard();
        });

        btnImport.onClick.AddListener(() =>
        {
            FileOperationByWin32.OpenSgfFile();
        });

        btnExport.onClick.AddListener(() =>
        {
            string path = FileOperationByWin32.Get_SavingSgfFilePath();
            if(path!=null && path.Length!=0)
                BaiPuMgr.Instance.GetCurrentBoard().SaveChessManual(path);
        });
    }
}
