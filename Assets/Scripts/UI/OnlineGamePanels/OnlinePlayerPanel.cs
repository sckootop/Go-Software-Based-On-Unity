using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class OnlinePlayerPanel : MonoBehaviour
{
    //1为左 白棋 2为右 黑棋
    public Text txtPlayerName1;
    public Text txtPlayerName2;
    public Text txtRankScore1;
    public Text txtRankScore2;

    public Text txtEatenChesses1;//显示“提子数”的UI
    public Text txtEatenChesses2;

    public Text txtTime1;//展示的倒计时UI
    public Text txtTime2;
    public readonly float duration = 7200f;//暂时设定为固定值，单位为秒，默认值为7200s
    public readonly float looptime = 30f;//倒计时结束，读秒时间
    public readonly int loopturns = 5;//读秒轮数
    private float currentTime1;
    private float currentLoopTime1;
    private int currentLoopTurns1;
    private bool loopStart1 = false;
    private float currentTime2;
    private float currentLoopTime2;
    private int currentLoopTurns2;
    private bool loopStart2 = false;

    OnlinechessBoard chessboard;

    [HideInInspector]
    public AudioSource audioSource = null;

    private void Awake()
    {
        UIMgr.Instance.chatPanel = GameObject.Find("OnlinePlayerPanel").GetComponent<ChatPanel>();
        chessboard = OnlinegameChessBoardMgr.Instance.GetCurrentBoard();
    }

    void Start()
    {
        UserInfo localInfo = GameDataMgr.Instance.GetLocalUserInfo();
        UserInfo opponentInfo = GameDataMgr.Instance.GetOpponentUserInfo();
        if (GameDataMgr.Instance.isWhite == false)
        {
            txtPlayerName2.text = localInfo.userName;
            txtRankScore2.text = localInfo.rankScore.ToString();
            txtPlayerName1.text = opponentInfo.userName;
            txtRankScore1.text = opponentInfo.rankScore.ToString();
        }
        else
        {
            txtPlayerName1.text = localInfo.userName;
            txtRankScore1.text = localInfo.rankScore.ToString();
            txtPlayerName2.text = opponentInfo.userName;
            txtRankScore2.text = opponentInfo.rankScore.ToString();
        }

        currentTime1 = duration;
        currentLoopTime1 = looptime;
        currentLoopTurns1 = loopturns;
        currentTime2 = duration;
        currentLoopTime2 = looptime;
        currentLoopTurns2 = loopturns;

        ChangeTimeUI(txtTime1, currentTime1);
        ChangeTimeUI(txtTime2, currentTime2);
    }

    void Update()
    {
        //Update里改变时间，时间UI的变化会有点不一致，这是缺点
        if (chessboard.isgamefinish) return;

        bool turn = GameDataMgr.Instance.turn;
        if(turn)//白棋下棋
        {
            if(currentTime1 <=0)//思考时间用完了 提示开始读秒
            {
                if(loopStart1==false && SettingMgr.Instance.isDumiaoOn==true)
                {
                    MusicMgr.Instance.PlaySound("开始读秒", false);
                    loopStart1 = true;
                }
                
                //提示倒计时
                if(currentLoopTime1<= 5 && SettingMgr.Instance.isCountdownOn)
                {
                    audioSource = MusicMgr.Instance.PlaySound("5秒倒计时", false);
                }

                if(currentLoopTime1<=0)//用完一次读秒
                {
                    if(currentLoopTurns1 > 0)
                    {
                        currentLoopTurns1--;
                        currentLoopTime1 = looptime;
                    }
                    else //超时判负 向服务器发送比赛结果 更新用户数据 服务器会发消息更新客户端数据
                    {
                        GameTimeOutMsg msg = new GameTimeOutMsg();
                        msg.iswhitewin = !GameDataMgr.Instance.isWhite;
                        ClientAsyncNet.Instance.Send(msg);
                        MusicMgr.Instance.PlaySound("超时判负",false);
                    }
                }
                currentLoopTime1 -= Time.deltaTime;
                ChangeTimeUI(txtTime1, currentLoopTime1);
            }
            else
            {
                currentTime1 -= Time.deltaTime;
                ChangeTimeUI(txtTime1, currentTime1);
            }
        }
        else if(!turn)//黑棋下棋
        {
            if (currentTime2 <= 0)//思考时间用完了 提示开始读秒
            {
                if (loopStart2 == false && SettingMgr.Instance.isDumiaoOn == true)
                {
                    MusicMgr.Instance.PlaySound("开始读秒", false);
                    loopStart2 = true;
                }

                //提示倒计时
                if (currentLoopTime2 <= 5 && SettingMgr.Instance.isCountdownOn)
                {
                    audioSource = MusicMgr.Instance.PlaySound("5秒倒计时", false);
                }

                if (currentLoopTime2 <= 0)//用完一次读秒
                {
                    if (currentLoopTurns2 > 0)
                    {
                        currentLoopTurns2--;
                        currentLoopTime2 = looptime;
                    }
                    else //超时判负 向服务器发送比赛结果 更新用户数据 服务器会发消息更新客户端数据
                    {
                        GameTimeOutMsg msg = new GameTimeOutMsg();
                        msg.iswhitewin = !GameDataMgr.Instance.isWhite;
                        ClientAsyncNet.Instance.Send(msg);
                        MusicMgr.Instance.PlaySound("超时判负", false);
                    }
                }
                currentLoopTime2 -= Time.deltaTime;
                ChangeTimeUI(txtTime2, currentLoopTime2);
            }
            else
            {
                currentTime2 -= Time.deltaTime;
                ChangeTimeUI(txtTime2, currentTime2);
            }
        }

        txtEatenChesses1.text = GameDataMgr.Instance.eatenCount1.ToString();
        txtEatenChesses2.text = GameDataMgr.Instance.eatenCount2.ToString();
    }

    private void ChangeTimeUI(Text txtTime,float time)
    {
        int t1 = (int)time / 3600;
        if (t1 < 10)
        {
            txtTime.text = "0" + t1 + ":";
        }
        else
        {
            txtTime.text = t1 + ":";
        }

        int t2 = (int)time % 3600 / 60;
        if (t2 < 10)
        {
            txtTime.text += "0" + t2 + ":";
        }
        else
        {
            txtTime.text += t2 + ":";
        }

        int t3 = (int)time % 60;
        if (t3 < 10)
        {
            txtTime.text += "0" + t3;
        }
        else
        {
            txtTime.text += t3;
        }
    }

    void OnDestroy()
    {
        GameDataMgr.Instance.Reset();
        UIMgr.Instance.onlinePlayerPanel = null;
    }
}
