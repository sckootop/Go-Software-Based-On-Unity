using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//复制到另一个客户端测试
public class OnlineGameEntrancePanel : BasePanel
{
    public Text txtSize;
    public Button btnSizeLeft;
    public Button btnSizeRight;
    string[] SizeSelections = new string[] { "9×9", "13×13", "19×19" };
    private int curindex = 0;
    private int size = 9;

    public Button btnStart;
    public Button btnReturn;
    protected override void Init()
    {
        txtSize.text = SizeSelections[curindex];
        OnlinegameChessBoardMgr.Instance.boardsize = 9;

        btnSizeLeft.onClick.AddListener(() =>
        {
            curindex = curindex - 1 >= 0 ? curindex - 1 : SizeSelections.Length - 1;
            txtSize.text = SizeSelections[curindex];
            if (curindex == 0) size = 9;
            else if (curindex == 1) size = 13;
            else if (curindex == 2) size = 19;
            OnlinegameChessBoardMgr.Instance.boardsize = size;
        });

        btnSizeRight.onClick.AddListener(() =>
        {
            curindex = curindex + 1 < SizeSelections.Length ? curindex + 1 : 0;
            txtSize.text = SizeSelections[curindex];
            if (curindex == 0) size = 9;
            else if (curindex == 1) size = 13;
            else if (curindex == 2) size = 19;
            OnlinegameChessBoardMgr.Instance.boardsize = size;
        });

        btnStart.onClick.AddListener(() =>
        {
            MatchMsg msg = new MatchMsg();
            msg.size = (short)size;
            ClientAsyncNet.Instance.Send(msg);//发送匹配消息
            UIMgr.Instance.ShowPanel<WaitPanel>();
            UIMgr.Instance.HidePanel<OnlineGameEntrancePanel>();
        });

        btnReturn.onClick.AddListener(() =>
        {
            UIMgr.Instance.ShowPanel<PlayPanel>();
            UIMgr.Instance.HidePanel<OnlineGameEntrancePanel>();
        });
    }
}
