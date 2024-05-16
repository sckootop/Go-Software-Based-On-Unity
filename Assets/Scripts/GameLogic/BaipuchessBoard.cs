using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class BaipuchessBoard : MonoBehaviour
{
    private Transform LeftTop;
    private Transform RightBottom;

    Vector3 LTPos;
    Vector3 RBPos;

    public int size;

    int[,] board;
    List<int[,]> board_history = new List<int[,]>();//棋盘历史状态，用于步数快速更新
    Renderer[,] chesses;
    bool[,] transparent;

    float halfGridWidth = 1;
    float halfGridHeight = 1;
    int rowi = 0, coli = 0;
    [HideInInspector]
    public bool isWhite = false;
    [HideInInspector]
    public bool turn = true;//是否轮流下
    bool canFall = false;
    bool[,] visited;
    List<Tuple<int, int>> eatenChesses = new List<Tuple<int, int>>();
    List<Tuple<int, int>> tempCheeses = new List<Tuple<int, int>>();
    Tuple<int, int> jie = new Tuple<int, int>(-1, -1);
    bool prevJie = false;

    ChessManual chessmanual = new ChessManual();//记载棋局信息
    int curstep = 0;//当前到了第几步
    int curshow = 0;//当前显示的是哪步的棋盘
    List<Stone> stones = new List<Stone>();//记录棋局落子，减少getplaylist调用

    public BaipuInfoPanel BaipuInfo;//涉及面板的更新所以定义

    private bool canPlay = true;

    TextStep textStep = null;

    void Awake()
    {
        board = new int[size, size];
        transparent = new bool[size, size];
        visited = new bool[size, size];
        chesses = new Renderer[size, size];

        Application.targetFrameRate = 20;
        chessmanual.setTime(System.DateTime.Now.ToString("f"));
        chessmanual.setSize(size);
        chessmanual.setRule("chinese");
        if (size != 19) chessmanual.setTiemu(6.5f);
        else chessmanual.setTiemu(7.5f);
        chessmanual.setBlackName("黑方名字");
        chessmanual.setBlackRank("15k");
        chessmanual.setWhiteName("白方名字");
        chessmanual.setWhiteRank("15k");
    }

    // Start is called before the first frame update
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
                chesses[i, j].material.color = new Color(chesses[i, j].material.color.r, chesses[i, j].material.color.g, chesses[i, j].material.color.b, 0);//一开始都透明不显示,Color是结构体，不影响GC
            }
        }
        board_history.Add((int[,])board.Clone());
        if(BaiPuMgr.Instance.cm!=null)
        {
            InitChessManual(BaiPuMgr.Instance.cm);
            BaiPuMgr.Instance.cm = null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIMgr.Instance.ShowPanel<BaipuPausePanel>();
            canPlay = false;
        }

        if (!canPlay) return;

        LTPos = Camera.main.WorldToScreenPoint(LeftTop.transform.position);
        RBPos = Camera.main.WorldToScreenPoint(RightBottom.transform.position);

        halfGridWidth = (RBPos.x - LTPos.x) / (size * 2 - 2);
        halfGridHeight = (LTPos.y - RBPos.y) / (size * 2 - 2);

        //确定鼠标放置位置的列坐标coli,行坐标rowi
        int colt = (int)((Input.mousePosition.x - LTPos.x) / halfGridWidth);
        int rowt = (int)((Input.mousePosition.y - RBPos.y) / halfGridHeight);
        coli = (colt + 1) / 2;
        rowi = (rowt + 1) / 2;
        if (colt >= 0 && rowt >= 0 && rowi >= 0 && rowi < size && coli >= 0 && coli < size)//在规定范围内才能落子
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
            if(FallChess(rowi, coli, isWhite ? 1 : 2))
            {
                MusicMgr.Instance.PlaySound("落子", false);
                //判断是否从历史棋局开始下棋，如是需要更新
                if (curshow != curstep && BaipuItem.isClick == true)
                {
                    BaipuInfo.RemoveAfter(curshow);//更新UI
                    board_history.RemoveRange(curshow + 1, board_history.Count - curshow - 1);//移除历史棋局
                    stones.RemoveRange(curshow, stones.Count - curshow);
                    
                    curstep = curshow;
                    BaipuItem.isClick = false;
                }

                ++curstep;
                curshow = curstep;
                Stone newstone = new Stone(isWhite, coli, rowi, curstep);
                stones.Add(newstone);
                ShowTextObj(curshow, !isWhite, chesses[rowi, coli].transform.position - new Vector3(0, 0, 0.2f));

                board_history.Add(board);//更新最新的棋局到历史棋局
                board = (int[,])board.Clone();//注意要深拷贝，否则下一步会修改同一块内存区域
                
                //更新面板
                GameObject item = Instantiate(Resources.Load<GameObject>("BaipuItem"));
                item.GetComponent<BaipuItem>().InitInfo(newstone);
                BaipuInfo.AddItem(item);

                if(turn)
                {
                    isWhite = !isWhite;
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

    protected void ShowTextObj(int _step, bool _whitecolor, Vector3 _position)
    {
        if (textStep == null || textStep.gameObject.activeSelf == false)
            textStep = PoolMgr.Instance.GetObj("Text/3dStep").GetComponent<TextStep>();
        textStep.InitText(_step, _whitecolor, _position, size == 19 ? 0.8f : (size == 13 ? 0.9f : 1f));
    }

    public void ShowTextObj(Stone s)
    {
        if (textStep == null || textStep.gameObject.activeSelf == false)
            textStep = PoolMgr.Instance.GetObj("Text/3dStep").GetComponent<TextStep>();
        textStep.InitText(s.getStep(), !s.getColor(), chesses[s.getY(), s.getX()].transform.position - new Vector3(0, 0, 0.2f), size == 19 ? 0.8f : (size == 13 ? 0.9f : 1f));
    }

    public void SetBoard(int i)
    {
        curshow = i;
        board = (int[,])board_history[i].Clone();//一定要深拷贝，否则会修改历史棋局
        DrawBoard();
        if(i==0 && textStep!=null)
            PoolMgr.Instance.PushObj("Text/3dStep", textStep.gameObject);
    }

    public void SetLatestBoard()
    {
        curshow = board_history.Count - 1;
        board = (int[,])board_history[curshow].Clone();
        DrawBoard();
    }

    //重置棋盘 以及 棋谱信息UI，更新棋盘
    public void ResetBoard()
    {
        BaipuInfo.RemoveAfter(0);
        board_history.RemoveRange(1, board_history.Count - 1);
        curstep = 0;
        BaipuItem.isClick = false;

        SetBoard(0);
    }

    public void InitChessManual(ChessManual cm)
    {
        chessmanual = cm;
        ResetBoard();
        if (chessmanual.getSize()!= size)
        {
            print("棋盘大小不匹配");
            return;
        }

        //读取新的棋谱记录的棋局有关信息
        chessmanual.setTime(System.DateTime.Now.ToString("f"));
        if (size != 19) chessmanual.setTiemu(6.5f);
        else chessmanual.setTiemu(7.5f);
        chessmanual.setBlackName(cm.getBlackName());
        chessmanual.setBlackRank(cm.getBlackRank());
        chessmanual.setWhiteName(cm.getWhiteName());
        chessmanual.setWhiteRank(cm.getWhiteRank());

        stones = chessmanual.getPlayList();
        int x,y;
        foreach (Stone stone in stones)
        {
            x = stone.getX();
            y = stone.getY();
            FallChess(y, x, stone.getColor() ? 1 : 2);//落子
            board_history.Add(board);
            board = (int[,])board.Clone();
            ++curstep;
            curshow = curstep;
            GameObject item = Instantiate(Resources.Load<GameObject>("BaipuItem"));
            item.GetComponent<BaipuItem>().InitInfo(stone);
            BaipuInfo.AddItem(item); 
        }
        DrawBoard();
        ShowTextObj(stones[stones.Count-1]);
    }

    public void SaveChessManual(string path)
    {
        chessmanual.setPlayList(stones);
        File.WriteAllText(path, ParseSgfHelper.Chessmanual2Sgf(chessmanual));
    }

    public void DelayCanPlay()
    {
        StartCoroutine("DelayPlay");
    }

    private IEnumerator DelayPlay()
    {
        yield return null;
        canPlay = true;
    }

    #region 围棋本身的游戏逻辑
    //判断是否有气
    bool hasAir(int i, int j, int type)
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
        for (int i = 0; i < size; ++i)
            for (int j = 0; j < size; ++j)
                visited[i, j] = false;
    }

    void eatChesses(out int cnt, out int[] firstEatenChess)
    {
        cnt = 0;
        firstEatenChess = new int[2] { -1, -1 };//记录第一个被吃的子的位置
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
        board[i, j] = type;//直接落子，好判断自己气的情况，以及对方棋子气的情况，如果是禁下区等会还原

        bool self_hasAir = hasAir(i, j, type);
        tempCheeses.Clear();
        ResetVisited();
        int opposite_type = (type == 1 ? 2 : 1);
        bool other_hasAir = true;//判断 是否所有相邻区域的对手棋都有气 一旦有一个没有就置为false
        bool playmusic = true;
        int eatcount = 0;

        if (j < size-1 && !visited[i, j + 1] && board[i, j + 1] == opposite_type)
        {
            if (hasAir(i, j + 1, opposite_type) == false)//没气
            {
                other_hasAir = false;
                foreach (var item in tempCheeses)
                {
                    eatenChesses.Add(item);
                }
            }
            tempCheeses.Clear();//一定要写外面，有hasAir就要加
            ResetVisited();//visited数组也一样,因为我的深度遍历不是找完，而是找到有空结束了
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
            //判断当前吃的子的是不是上次劫所在的位置
            if (prevJie == true && eatenChess[0] == jie.Item1 && eatenChess[1] == jie.Item2)
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

        if (self_hasAir == false && other_hasAir == true)//禁下区! 还原！
        {
            board[i, j] = 0;
            playmusic = false;
            print("禁下区哦!");
        }

        if (playmusic == true)
        {
            return true;
        }
        return false;
    }
    #endregion
}
