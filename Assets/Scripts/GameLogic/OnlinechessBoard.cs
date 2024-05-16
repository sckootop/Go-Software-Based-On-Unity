using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

public class OnlinechessBoard : MonoBehaviour
{
    private Transform LeftTop;
    private Transform RightBottom;

    Vector3 LTPos;
    Vector3 RBPos;

    public int size;

    int[,] board;//1为白 2为黑
    Renderer[,] chesses;
    bool[,] transparent;
    
    float halfGridWidth = 1; 
    float halfGridHeight = 1; 
    int rowi = 0, coli = 0;
    bool initialColor;
    bool canFall = false;
    bool[,] visited;
    List<Tuple<int, int>> eatenChesses = new List<Tuple<int, int>>();
    List<Tuple<int, int>> tempCheeses = new List<Tuple<int, int>>();
    Tuple<int, int> jie = new Tuple<int, int>(-1, -1);
    bool prevJie = false;

    //棋局记录
    public ChessManual chessmanual = new ChessManual();
    int step = 0;

    bool canplay = true;
    bool myselfStopStep = false;
    [HideInInspector]
    public bool opponentStopStep = false;
    [HideInInspector]
    public bool isgamefinish = false;

    TextStep textStep = null;

    void Awake()
    {
        initialColor = GameDataMgr.Instance.isWhite;//本地玩家执什么棋

        board = new int[size, size];
        chesses = new Renderer[size, size];
        transparent = new bool[size, size];
        visited = new bool[size, size];

        chessmanual.setTime(System.DateTime.Now.ToString("f"));
        if (size != 19) chessmanual.setTiemu(6.5f);
        else chessmanual.setTiemu(7.5f);
        UserInfo info = GameDataMgr.Instance.GetLocalUserInfo();
        if(initialColor==false)
        {
            chessmanual.setBlackName(info.userName);
            chessmanual.setBlackRank(info.GetLevel());
            info = GameDataMgr.Instance.GetOpponentUserInfo();
            chessmanual.setWhiteName(info.userName);
            chessmanual.setWhiteRank(info.GetLevel());
        }
        else
        {
            chessmanual.setWhiteName(info.userName);
            chessmanual.setWhiteRank(info.GetLevel());
            info = GameDataMgr.Instance.GetOpponentUserInfo();
            chessmanual.setBlackName(info.userName);
            chessmanual.setBlackRank(info.GetLevel());
        }
        chessmanual.setRule("chinese");
        chessmanual.setSize(size);
    }

    void Start()
    {
        Transform cb = this.transform;
        LeftTop = cb.Find("LeftTop");
        RightBottom = cb.Find("RightBottom");

        Transform row;
        for (int i = 0; i < size; ++i)
        {
            row = cb.Find($"row{i + 1}");
            for (int j = 0; j < size; ++j)
            {
                chesses[i, j] = row.Find($"Chess{i + 1}_{j + 1}").gameObject.GetComponent<Renderer>();
                chesses[i, j].material.color = new Color(chesses[i, j].material.color.r, chesses[i, j].material.color.g, chesses[i, j].material.color.b, 0);
            }
        }

        StartCoroutine(WaitGameResultThenShowPanel());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIMgr.Instance.ShowPanel<OnlinePausePanel>();
            canplay = false;
        }

        if (isgamefinish) return;

        if (GameDataMgr.Instance.turn != initialColor)
        {
            //对方行棋
            int[] t = GameDataMgr.Instance.GetChessPos();
            if (t[0]!=-1 && t[1]!=-1)
            {
                FallChess(t[1], t[0], GameDataMgr.Instance.turn ? 1 : 2);
                opponentStopStep = false;
                chessmanual.AddStone(new Stone(GameDataMgr.Instance.turn, t[0], t[1], ++step));
                ShowTextObj(step, !GameDataMgr.Instance.turn, chesses[t[1], t[0]].transform.position - new Vector3(0, 0, 0.2f));
                GameDataMgr.Instance.turn = !GameDataMgr.Instance.turn;
            }
        }
        else
        {
            if (!canplay) return;
            
            //我方行棋
            LTPos = Camera.main.WorldToScreenPoint(LeftTop.transform.position);
            RBPos = Camera.main.WorldToScreenPoint(RightBottom.transform.position);

            halfGridWidth = (RBPos.x - LTPos.x) / (size * 2 - 2);
            halfGridHeight = (LTPos.y - RBPos.y) / (size * 2 - 2);

            int colt = (int)((Input.mousePosition.x - LTPos.x) / halfGridWidth);
            int rowt = (int)((Input.mousePosition.y - RBPos.y) / halfGridHeight);
            coli = (colt + 1) / 2;
            rowi = (rowt + 1) / 2;
            if (colt >= 0 && rowt >= 0 && rowi >= 0 && rowi < size && coli >= 0 && coli < size)
            {
                if (board[rowi, coli] == 0)
                {
                    transparent[rowi, coli] = true;
                    canFall = true;
                }
                else
                {
                    canFall = false;
                }
            }
            else
            {
                canFall = false;
            }

            if (canFall && Input.GetMouseButtonUp(0))
            {
                if(FallChess(rowi, coli, GameDataMgr.Instance.turn ? 1 : 2)==true)
                {
                    BoardMsg msg = new BoardMsg();
                    msg.x = coli;//列
                    msg.y = rowi;//行
                    ClientAsyncNet.Instance.Send(msg);

                    if(OnlinegameChessBoardMgr.Instance.GetOnlinePlayerPanel().audioSource != null)
                    {
                        MusicMgr.Instance.StopSound(OnlinegameChessBoardMgr.Instance.GetOnlinePlayerPanel().audioSource);
                    }

                    myselfStopStep = false;

                    chessmanual.AddStone(new Stone(GameDataMgr.Instance.turn, coli, rowi, ++step));
                    ShowTextObj(step, !GameDataMgr.Instance.turn, chesses[rowi, coli].transform.position - new Vector3(0, 0, 0.2f));
                    GameDataMgr.Instance.turn = !GameDataMgr.Instance.turn;
                }
            }
        }
        
        DrawBoard();
    }

    private void DrawBoard()
    {
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                if (board[i, j] == 0)
                {
                    chesses[i, j].material.color = new Color(chesses[i, j].material.color.r, chesses[i, j].material.color.g, chesses[i, j].material.color.b, 0f);
                    if (transparent[i, j] == true && GameDataMgr.Instance.turn == GameDataMgr.Instance.isWhite)//轮到自己下
                    {
                        if (GameDataMgr.Instance.turn)
                            chesses[i, j].material.color = new Color(1, 1, 1, 0.5f);
                        else if (!GameDataMgr.Instance.turn)
                            chesses[i, j].material.color = new Color(0, 0, 0, 0.5f);
                        transparent[i, j] = false;
                    }
                }
                if (board[i, j] == 1)
                {
                    chesses[i, j].material.color = Color.white;
                }
                else if (board[i, j] == 2)
                {
                    chesses[i, j].material.color = Color.black;
                }
            }
        }
    }

    //认输
    public void Concede()
    {
        //向服务器发消息 是谁赢了
        ConcedeMsg msg = new ConcedeMsg();
        if (initialColor == false) msg.iswhitewin = true;
        else msg.iswhitewin = false;
        ClientAsyncNet.Instance.Send(msg);
    }

    //停一手
    public void StopOneStep()
    {
        myselfStopStep = true;
        GameDataMgr.Instance.turn = !GameDataMgr.Instance.turn;
        if(SettingMgr.Instance.isXushouOn)
            MusicMgr.Instance.PlaySound("虚手", false);
        if (opponentStopStep == true)//双方都停一手 申请终局判断
        {
            GameInfoMsg msg = new GameInfoMsg();
            msg.sgfContent = ParseSgfHelper.Chessmanual2Sgf(chessmanual);
            ClientAsyncNet.Instance.Send(msg);
            //接下来就等服务器返回结果 不用做什么操作
        }
        else
        {
            //向服务器发消息
            StopStepMsg msg = new StopStepMsg();
            ClientAsyncNet.Instance.Send(msg);
        }
        DelayCanPlay();
    }

    private IEnumerator WaitGameResultThenShowPanel()
    {
        while(GameDataMgr.Instance.gameResult == null)
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }
        GameResult result = GameDataMgr.Instance.gameResult;
        GameResultPanel panel = UIMgr.Instance.ShowPanel<GameResultPanel>();
        panel.action = () => { UIMgr.Instance.ShowPanel<OnlinePausePanel>(); };
        bool winnerIsWhite = result.winner == 'W' ? true : false;
        if(result.reason == "R")
        {
            panel.ChangeTxtInfo(winnerIsWhite == true ? "白方胜" : "黑方胜", winnerIsWhite == true ? "黑方认输" : "白方认输");
        }
        else if(result.reason == "T")
        {
            panel.ChangeTxtInfo(winnerIsWhite == true ? "白方胜" : "黑方胜", winnerIsWhite == true ? "黑方超时判负" : "白方超时判负");
        }
        else
        {
            panel.ChangeTxtInfo(winnerIsWhite == true ? "白方胜" : "黑方胜", (winnerIsWhite == true ? "白方领先" : "黑方领先") + result.reason + "目");
        }   
        isgamefinish = true;

        //保存棋局
        string savePath = Application.persistentDataPath + "/RecentGames";
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
        savePath += "/sgf_" + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000 + ".sgf";
        ParseSgfHelper.SaveContent(savePath, chessmanual);
    }

    public void DelayCanPlay()
    {
        StartCoroutine("DelayPlay");
    }

    private IEnumerator DelayPlay()
    {
        yield return null;
        canplay = true;
    }

    protected void ShowTextObj(int _step, bool _whitecolor, Vector3 _position)
    {
        if (textStep == null || textStep.gameObject.activeSelf == false)
            textStep = PoolMgr.Instance.GetObj("Text/3dStep").GetComponent<TextStep>();
        textStep.InitText(_step, _whitecolor, _position, size == 19 ? 0.8f : (size == 13 ? 0.9f : 1f));
    }

    bool hasAir(int i, int j, int type)
    {
        if (board[i, j] == 0) return true;
        if (board[i, j] != type) return false;

        visited[i, j] = true;
        tempCheeses.Add(new Tuple<int, int>(i, j));
        if (j < size-1 && !visited[i, j + 1] && hasAir(i, j + 1, type))
        {
            return true;
        }
        if (i > 0 && !visited[i - 1, j] && hasAir(i - 1, j, type))
        {
            return true;
        }
        if (j > 0 && !visited[i, j - 1] && hasAir(i, j - 1, type))
        {
            return true;
        }
        if (i < size-1 && !visited[i + 1, j] && hasAir(i + 1, j, type))
        {
            return true;
        }
        return false;
    }

    void ResetVisited()
    {
        for (int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                visited[i, j] = false;
    }

    void eatChesses(out int cnt, out int[] firstEatenChess)
    {
        cnt = 0;
        firstEatenChess = new int[2] { -1, -1 };
        foreach (var item in eatenChesses)
        {
            cnt++;
            board[item.Item1, item.Item2] = 0;
            if (cnt == 1)
            {
                firstEatenChess[0] = item.Item1;
                firstEatenChess[1] = item.Item2;
            }
        }
        eatenChesses.Clear();
    }

    bool FallChess(int i, int j, int type)
    {
        board[i, j] = type;

        bool self_hasAir = hasAir(i, j, type);
        tempCheeses.Clear();
        ResetVisited();
        int opposite_type = (type == 1 ? 2 : 1);
        bool other_hasAir = true;
        bool playmusic = true;
        int eatcount = 0;

        if (j < size-1 && !visited[i, j + 1] && board[i, j + 1] == opposite_type)
        {
            if (hasAir(i, j + 1, opposite_type) == false)
            {
                other_hasAir = false;
                foreach (var item in tempCheeses)
                {
                    eatenChesses.Add(item);
                }
            }
            tempCheeses.Clear();
            ResetVisited();
        }

        if (i > 0 && !visited[i - 1, j] && board[i - 1, j] == opposite_type)
        {
            if (hasAir(i - 1, j, opposite_type) == false)
            {
                other_hasAir = false;
                foreach (var item in tempCheeses)
                {
                    eatenChesses.Add(item);
                }
            }
            tempCheeses.Clear();
            ResetVisited();
        }

        if (j > 0 && !visited[i, j - 1] && board[i, j - 1] == opposite_type)
        {
            if (hasAir(i, j - 1, opposite_type) == false)
            {
                other_hasAir = false;
                foreach (var item in tempCheeses)
                {
                    eatenChesses.Add(item);
                }
            }
            tempCheeses.Clear();
            ResetVisited();
        }

        if (i < size-1 && !visited[i + 1, j] && board[i + 1, j] == opposite_type)
        {
            if (hasAir(i + 1, j, opposite_type) == false)
            {
                other_hasAir = false;
                foreach (var item in tempCheeses)
                {
                    eatenChesses.Add(item);
                }
            }
            tempCheeses.Clear();
            ResetVisited();
        }

        eatChesses(out eatcount, out int[] eatenChess);
        if (eatcount == 1)
        {
            if (prevJie == true && eatenChess[0] == jie.Item1 && eatenChess[1] == jie.Item2)
            {
                board[i, j] = 0;
                board[eatenChess[0], eatenChess[1]] = opposite_type;
                playmusic = false;
                print("同一个劫打劫不能循环!");
            }
            else
            {
                jie = new Tuple<int, int>(i, j);
                prevJie = true;
            }
        }
        else
        {
            prevJie = false;
        }

        if (self_hasAir == false && other_hasAir == true)
        {
            board[i, j] = 0;
            playmusic = false;
            print("禁下区哦!");
        }

        if (playmusic == true)//代表是正确的落子
        {
            if(GameDataMgr.Instance.turn==false)//黑棋吃子
            {
                GameDataMgr.Instance.eatenCount2 += eatcount;
            }
            else
            {
                GameDataMgr.Instance.eatenCount1 += eatcount;
            }
            MusicMgr.Instance.PlaySound("落子", false);
            return true;
        }
        return false;
    }

    void OnDestroy()
    {
        GameDataMgr.Instance.Reset();
    }
}
