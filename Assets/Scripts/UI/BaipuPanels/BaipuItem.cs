using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaipuItem : MonoBehaviour
{
    public static bool isClick = false;//更新info要用到

    public Button btn;
    public Text leftTxt;
    public Text rightTxt;
    public Image imgChess;

    private BaipuchessBoard board;

    private static string alphabet = "ABCDEFGHJKLMNOPQRSTUVWXYZ";//棋盘要显示的信息 I是去掉的，显示归显示，棋谱还是有I的

    private Stone mystone;

    // Start is called before the first frame update
    void Start()
    {
        board= BaiPuMgr.Instance.GetCurrentBoard();
        btn.onClick.AddListener(() =>
        {
            int i = int.Parse(leftTxt.text.Trim('.'));
            board.SetBoard(i);
            if(mystone!=null)
                board.ShowTextObj(mystone);

            isClick = true;
            
            if(board.turn)
            {
                //修改下一次落子的颜色
                board.isWhite = !mystone.getColor();
            }
        });
    }

    public void InitInfo(Stone _stone)
    {
        mystone = _stone;
        leftTxt.text = mystone.getStep() + ".";
        rightTxt.text = alphabet[mystone.getX()] + (BaiPuMgr.Instance.GetCurrentBoard().size - mystone.getY()).ToString();
        switch(mystone.getColor())
        {
            case false:
                Texture2D texture1 = ResMgr.Instance.Load<Texture2D>("Chess/黑棋64");
                Sprite sprite1 = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height), new Vector2(1f, 0.5f));
                imgChess.sprite = sprite1;
                break;
            case true:
                Texture2D texture2 = ResMgr.Instance.Load<Texture2D>("Chess/白棋64");
                Sprite sprite2 = Sprite.Create(texture2, new Rect(0, 0, texture2.width, texture2.height), new Vector2(1f, 0.5f));
                imgChess.sprite = sprite2;
                break;
            default:
                imgChess.sprite = null;
                break;
        }
    }
}
