using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PVEchessBoard : MonoBehaviour
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

    bool canplay = false;

    bool turn = false;//false黑棋下 true白棋下
    string pos;

    TextStep textStep = null;

    int step = 0;

    void Awake()
    {
        initialColor = turn;//本地玩家执什么棋

        board = new int[size, size];
        chesses = new Renderer[size, size];
        transparent = new bool[size, size];
        visited = new bool[size, size];
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

        Task.Run(() =>
        {
            KatagoHelper.katago.Clear_Board();
            if (size != 19)
            {
                KatagoHelper.katago.SetKomi(6.5f);
            }
            else
            {
                KatagoHelper.katago.SetKomi(7.5f);
            }
            KatagoHelper.katago.ChangeBoardSize(size);
        }).ContinueWith(task => { canplay = true; }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           UIMgr.Instance.ShowPanel<PVEPausePanel>();
            canplay = false;
        }

        if (!canplay) return;
        if (turn != initialColor)
        {
            //AI行棋
            canplay = false;

            //string pos = KatagoHelper.katago.Genmove(turn ? 'w' : 'b');//这样写会造成阻塞，Unity主线程被阻塞，上一个音乐播放不出来
            Task.Run(async () =>
                    {
                       pos = await KatagoHelper.katago.Genmove(turn ? 'w' : 'b');
                    }
            ).ContinueWith(task =>
            {
                print("pos:" + pos);
                if (pos == "pass") //AI pass有两种情况
                {
                    GameResult res = KatagoHelper.katago.Get_Final_Score();
                    if (initialColor && res.winner=='W' || !initialColor && res.winner=='B')//玩家赢 AI认输
                    {
                        //显示GameResultPanel
                        GameResultPanel panel = UIMgr.Instance.ShowPanel<GameResultPanel>();
                        panel.action += () => { UIMgr.Instance.ShowPanel<PVEPausePanel>(); };

                        panel.ChangeTxtInfo(res.winner == 'W' ? "白方胜" : "黑方胜", (res.winner == 'W' ? "白方领先" : "黑方领先") + res.reason + "目");

                        //游戏结束
                        canplay = false;
                    }
                    else //AI大优
                    {
                        //不下 玩家下
                        print("AI大优");
                        MusicMgr.Instance.PlaySound("虚手", false);
                        turn = !turn;
                        canplay = true;
                    }
                    return;
                }

                int[] t = new int[2];//落子位置 列 行 顺序
                t[0] = ParseSgfHelper.alphabet.IndexOf(pos[0]);
                t[1] = int.Parse(pos.Substring(1)) - 1;

                FallChess(t[1], t[0], turn ? 1 : 2);
                ShowTextObj(++step, !turn, chesses[t[1], t[0]].transform.position - new Vector3(0, 0, 0.2f));
                MusicMgr.Instance.PlaySound("落子", false);
                turn = !turn;
                canplay = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        else
        {
            //玩家行棋
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
                if (FallChess(rowi, coli, turn ? 1 : 2) == true)
                {
                    ShowTextObj(++step, !turn, chesses[rowi, coli].transform.position - new Vector3(0, 0, 0.2f));
                    MusicMgr.Instance.PlaySound("落子", false);
                    KatagoHelper.katago.Play(turn ? 'w' : 'b', coli, rowi);
                    turn = !turn;
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
                    if (transparent[i, j] == true && turn == initialColor)//轮到自己下
                    {
                        if (turn)
                            chesses[i, j].material.color = new Color(1, 1, 1, 0.5f);
                        else if (!turn)
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

    //玩家认输
    public void Concede()
    {
        canplay = false;
        //显示GameResultPanel
        GameResultPanel panel = UIMgr.Instance.ShowPanel<GameResultPanel>();
        panel.action += () => { UIMgr.Instance.ShowPanel<PVEPausePanel>(); };

        GameResult result = KatagoHelper.katago.Get_Final_Score();
        panel.ChangeTxtInfo(result.winner == 'W' ? "白方胜" : "黑方胜", (result.winner == 'W' ? "白方领先" : "黑方领先") + result.reason + "目");
    }

    protected void ShowTextObj(int _step, bool _whitecolor, Vector3 _position)
    {
        if (textStep == null || textStep.gameObject.activeSelf == false)
            textStep = PoolMgr.Instance.GetObj("Text/3dStep").GetComponent<TextStep>();
        textStep.InitText(_step, _whitecolor, _position, size == 19 ? 0.8f : (size == 13 ? 0.9f : 1f));
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

    bool hasAir(int i, int j, int type)
    {
        if (board[i, j] == 0) return true;
        if (board[i, j] != type) return false;

        visited[i, j] = true;
        tempCheeses.Add(new Tuple<int, int>(i, j));
        if (j < size - 1 && !visited[i, j + 1] && hasAir(i, j + 1, type))
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
        if (i < size - 1 && !visited[i + 1, j] && hasAir(i + 1, j, type))
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

        if (j < size - 1 && !visited[i, j + 1] && board[i, j + 1] == opposite_type)
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

        if (i < size - 1 && !visited[i + 1, j] && board[i + 1, j] == opposite_type)
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
            return true;
        }
        return false;
    }

    void OnDestroy()
    {
        
    }
}
