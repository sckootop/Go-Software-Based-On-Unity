using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LocalchessBoard : MonoBehaviour
{
    //锚点位置，用于计算棋子落点
    private Transform LeftTop;
    private Transform RightBottom;

    //锚点在屏幕上的映射位置
    Vector3 LTPos;
    Vector3 RBPos;

    public int size;//编辑器里即可设置棋盘尺寸

    int[,] board;//记录棋盘落子情况
    Renderer[,] chesses;//size×size的所有棋子
    bool[,] transparent;//透明度，与board分开
    List<int[,]> board_history = new List<int[,]>();

    float halfGridWidth = 1; //棋盘网格宽度的一半
    float halfGridHeight = 1; //棋盘网格高度的一半
    int rowi = 0, coli = 0;
    bool isWhite = false;//白棋，黑棋轮流
    bool canFall = false;//坐标检测下，是否能落子
    bool[,] visited;//访问记忆数组
    List<Tuple<int, int>> eatenChesses = new List<Tuple<int, int>>();
    List<Tuple<int, int>> tempCheeses = new List<Tuple<int, int>>();
    Tuple<int, int> jie = new Tuple<int, int>(-1, -1);//记录最新的一个劫的位置，棋局中劫是会有很多个的
    bool prevJie = false;//上次落子可能是劫吗

    //棋局记录
    ChessManual chessmanual = new ChessManual();
    List<Stone> stones = new List<Stone>();
    int step = 0;

    private bool canplay = true;
    private bool prevStopStep = false;

    Katago katago = KatagoHelper.katago;

    TextStep textStep = null;

    void Awake()
    {
        board = new int[size, size];
        chesses = new Renderer[size, size];
        transparent = new bool[size, size];
        visited = new bool[size, size];
        board_history.Add((int[,])board.Clone());

        chessmanual.setTime(System.DateTime.Now.ToString("f"));
        if (size != 19) chessmanual.setTiemu(6.5f);
        else chessmanual.setTiemu(7.5f);
        chessmanual.setBlackName("黑方名字");
        chessmanual.setBlackRank("15k");
        chessmanual.setWhiteName("白方名字");
        chessmanual.setWhiteRank("15k");
        chessmanual.setRule("chinese");
        chessmanual.setSize(size);
    }

    void Start()
    {
        Transform cb = this.transform;
        LeftTop = cb.Find("LeftTop");
        RightBottom = cb.Find("RightBottom");

        Transform row;
        for (int i=0;i<size;++i)
        {
            row = cb.Find($"row{i + 1}");
            for (int j=0;j<size;++j)
            {
                chesses[i, j] = row.Find($"Chess{i + 1}_{j + 1}").gameObject.GetComponent<Renderer>();
                chesses[i, j].material.color = new Color(chesses[i, j].material.color.r, chesses[i, j].material.color.g, chesses[i, j].material.color.b, 0);//一开始都透明不显示,Color是结构体，不影响GC
            }
        }

        Task.Run(() =>
        {
            if (size != 19)
            {
                katago.SetKomi(6.5f);
            }
            else
            {
                katago.SetKomi(7.5f);
            }
            katago.ChangeBoardSize(size);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!canplay) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIMgr.Instance.ShowPanel<LocalPausePanel>();
            canplay = false;
            return;
        }
        if(Input.GetMouseButtonUp(1))
        {
            HuiQi();
        }

        ///
        /// 锚点位置和网格宽度会受分辨率影响，所以放在update里，优化办法是屏幕分辨率改变时，改变这些值，暂时还不知道怎么做
        ///
        //计算锚点位置
        LTPos = Camera.main.WorldToScreenPoint(LeftTop.transform.position);
        RBPos = Camera.main.WorldToScreenPoint(RightBottom.transform.position);

        //计算网格宽度
        halfGridWidth = (RBPos.x - LTPos.x) / (size * 2 - 2);//距离除以格子数*2
        halfGridHeight = (LTPos.y - RBPos.y) / (size * 2 - 2);

        //确定鼠标放置位置的列坐标coli,行坐标rowi
        int colt = (int)((Input.mousePosition.x - LTPos.x) / halfGridWidth);
        int rowt = (int)((Input.mousePosition.y - RBPos.y) / halfGridHeight);
        coli = (colt + 1) / 2;
        rowi = (rowt + 1) / 2;
        if(colt>=0 && rowt>=0 && rowi >= 0 && rowi < size && coli >= 0 && coli < size)//在规定范围内才能落子
        { 
            if(board[rowi, coli] == 0)//只有空标志下才能落子 显示半透明
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
            canFall=false;
        }

        if(canFall && Input.GetMouseButtonUp(0))
        {
            if(FallChess(rowi, coli, isWhite ? 1 : 2))
            {
                board_history.Add((int[,])board.Clone());
                step++;
                stones.Add(new Stone(isWhite, coli, rowi, step));
                ShowTextObj(step, !isWhite, chesses[rowi, coli].transform.position - new Vector3(0, 0, 0.2f));
                isWhite = !isWhite;
                prevStopStep = false;
            }
        }

        DrawBoard();
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

    public void SetBoard(int i)
    {
        board = (int[,])board_history[i].Clone();
        board_history.RemoveRange(i + 1, board_history.Count - i - 1);
        stones.RemoveRange(i, stones.Count - i);
        step = i;
        DelayCanPlay();
        DrawBoard();
    }

    //悔棋
    public void HuiQi()
    {
        if (step == 0)
        {
            DelayCanPlay();
            return;
        }
        else
        {
            isWhite = !isWhite;
            SetBoard(step - 1);
            if (step > 0)
                ShowTextObj(step, !stones[step - 1].getColor(), chesses[stones[step - 1].getY(), stones[step - 1].getX()].transform.position - new Vector3(0, 0, 0.2f));
            else
                PoolMgr.Instance.PushObj("Text/3dStep", textStep.gameObject);
        }
    }

    //重新开始
    public void ResetBoard()
    {
        stones.Clear();
        isWhite = false;
        step = 0;
        PoolMgr.Instance.PushObj("Text/3dStep", textStep.gameObject);
        SetBoard(0);
    }

    public void StopOneStep()
    {
        if(prevStopStep)
        {
            //双方相继都停一手，计算目数，显示比赛结果
            canplay = false;
            ParseSgfHelper.SaveContent(Application.streamingAssetsPath + "/LocalMatchPath/match.sgf", chessmanual);

            katago.LoadSgf(Application.streamingAssetsPath + "/LocalMatchPath/match.sgf");

            GameResult result = katago.Get_Final_Score(); 
            bool winnerIsWhite = result.winner == 'W' ? true : false;
            GameResultPanel gpanel = UIMgr.Instance.ShowPanel<GameResultPanel>();
            gpanel.action = () =>
            {
                UIMgr.Instance.ShowPanel<LocalPausePanel>();
            };
            gpanel.ChangeTxtInfo(winnerIsWhite == true ? "白方胜" : "黑方胜", (winnerIsWhite == true ? "白方领先" : "黑方领先") + result.reason + "目");

            chessmanual.setResult(winnerIsWhite == true ? "W" : "B" + "+" + result.reason);
            print(winnerIsWhite == true ? "W" : "B" + "+" + result.reason);
            ParseSgfHelper.SaveContent(Application.streamingAssetsPath + "/LocalMatchPath/match.sgf", chessmanual);
        }
        else
        {
            prevStopStep = true;
            isWhite = !isWhite;
            DelayCanPlay();
        }
    }

    protected void ShowTextObj(int _step, bool _whitecolor, Vector3 _position)
    {
        if (textStep == null || textStep.gameObject.activeSelf == false)
            textStep = PoolMgr.Instance.GetObj("Text/3dStep").GetComponent<TextStep>();
        textStep.InitText(_step, _whitecolor, _position, size == 19 ? 0.8f : (size == 13 ? 0.9f : 1f));
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
                    if (transparent[i, j] == true)
                    {
                        if (isWhite)
                            chesses[i, j].material.color = new Color(1, 1, 1, 0.5f);
                        else if (!isWhite)
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

    #region 围棋游戏逻辑
    //判断是否有气
    bool hasAir(int i,int j,int type)
    {
        if (board[i, j] == 0) return true;//为空有气
        if (board[i, j] != type) return false;//不为空且相反无气
        //同色继续处理

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
        for(int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                visited[i, j] = false;
    }

    void eatChesses(out int cnt, out int[] firstEatenChess)
    {
        cnt = 0;
        firstEatenChess = new int[2] { -1, -1 };//记录第一个被吃的子的位置
        foreach(var item in eatenChesses)
        {
            cnt++;
            board[item.Item1, item.Item2] = 0;
            if(cnt==1)
            {
                firstEatenChess[0] = item.Item1;
                firstEatenChess[1] = item.Item2;
            }
        }
        eatenChesses.Clear();
    }

    bool FallChess(int i,int j,int type)
    {
        board[i, j] = type;//直接落子，好判断自己气的情况，以及对方棋子气的情况，如果是禁下区等会还原

        bool self_hasAir = hasAir(i, j, type);
        tempCheeses.Clear();
        ResetVisited();
        int opposite_type = (type == 1 ? 2 : 1);
        bool other_hasAir = true;//判断 是否所有相邻区域的对手棋都有气 一旦有一个没有就置为false
        bool playmusic = true;
        int eatcount = 0;

        if (j<size-1 && !visited[i,j+1] && board[i,j+1] == opposite_type)
        {
            if (hasAir(i, j + 1, opposite_type) == false)//没气
            {
                other_hasAir = false;
                foreach(var item in tempCheeses)
                {
                    eatenChesses.Add(item);
                }
            }
            tempCheeses.Clear();//一定要写外面，有hasAir就要加
            ResetVisited();//visited数组也一样,因为我的深度遍历不是找完，而是找到有空结束了
        }

        if (i>0 && !visited[i-1,j] && board[i-1, j] == opposite_type)
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

        if (j>0 && !visited[i,j-1] && board[i,j-1] == opposite_type)
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

        if (i<size-1 && !visited[i+1,j] && board[i+1,j] == opposite_type)
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

        eatChesses(out eatcount,out int[] eatenChess);
        if (eatcount==1)
        {
            //判断当前吃的子的是不是上次劫所在的位置
            if (prevJie==true && eatenChess[0]==jie.Item1 && eatenChess[1]==jie.Item2)
            {
                //是，不能下，并且还原被吃的子
                board[i, j] = 0;//不能下
                board[eatenChess[0], eatenChess[1]] = opposite_type;//还原被吃的子
                playmusic = false;
                print("同一个劫打劫不能循环!");
            }
            else
            {
                //不是打同一个劫，可能是正常吃子，也有可能打的另外一个劫
                jie = new Tuple<int, int>(i, j);//只吃1子，可能是劫，记录位置
                prevJie = true;
            }
        }
        else
        {
            //没吃子，吃了大于一子的数量都代表 没打劫
            prevJie = false;
        }
        
        if (self_hasAir==false && other_hasAir==true)//禁下区! 还原！
        {
            board[i, j] = 0;
            playmusic = false;
            print("禁下区哦!");
        }

        if(playmusic==true)
        {
            chessmanual.AddStone(new Stone(isWhite, j, i, step+1));
            MusicMgr.Instance.PlaySound("落子", false);
            return true;
        }
        return false;
    }
    #endregion
}