using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour
{
    public Button btn;
    public Text leftTxt;
    public Text midTxt;
    public Image img;
    public GameTree gameTree;

    private int index;

    private void Start()
    {
        btn.onClick.AddListener(() =>
        {
            TikuMgr.Instance.ShowChessBoard(gameTree.size);
            TikuMgr.Instance.GetCurrentBoard().SetQuestion(gameTree);
            TikuMgr.Instance.GetCurrentBoard().ResetBoard();
            TikuMgr.Instance.GetTikuInfoPanel().SetIndex(index);
            TikuMgr.Instance.GetTikuInfoPanel().HideTip();
        });
    }

    public void InitInfo(string ltxt, string mtxt, GameTree t, int _index)
    {
        leftTxt.text = ltxt;
        midTxt.text = mtxt;
        gameTree = t;
        index = _index;
    }

    public void SetComplete(bool open)
    {
        if(open)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        }
        else
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        }
    }
}
