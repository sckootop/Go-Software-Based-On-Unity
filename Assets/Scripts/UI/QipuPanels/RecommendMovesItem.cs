using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecommendMovesItem : MonoBehaviour
{
    public Button btn;
    public Text leftTxt;
    public Text rightTxt;
    public Image imgChess;

    private MoveInfo moveInfo;

    private static string alphabet = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
    private static Dictionary<int, string> dic = new Dictionary<int, string>()
    {
        {1,"一"},{2,"二"},{3,"三"},{4,"四"},{5,"五"},{6,"六"},{7,"七"},{8,"八"},{9,"九"},{10,"十"},{11,"十一"},{12,"十二"}
    };

    private void Start()
    {
        btn.onClick.AddListener(() =>
        {
            QipuChessBoardMgr.Instance.GetChessBoard().ShowPredictMoves(moveInfo);
        });      
    }

    public void InitInfo(MoveInfo info, bool iswhite)
    {
        this.moveInfo = info;
        leftTxt.text = "第" + dic[moveInfo.order + 1] + "选点 " + moveInfo.winrate.ToString() + "%";
        rightTxt.text = alphabet[moveInfo.col] + (QipuChessBoardMgr.Instance.boardsize - moveInfo.row).ToString();
        switch(iswhite)
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
