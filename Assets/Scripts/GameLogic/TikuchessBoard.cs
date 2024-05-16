using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikuchessBoard : MonoBehaviour
{
    private Transform LeftTop;
    private Transform RightBottom;

    Vector3 LTPos;
    Vector3 RBPos;

    //[IntOptions(9, 13, 19)]
    public int size;//弄成属性会有问题!!! 暂时不知道原因

    int[,] board;
    List<int[,]> board_history = new List<int[,]>();//棋盘历史状态，用于步数快速更新，0下标为初始棋盘
    Renderer[,] chesses;
    bool[,] transparent;

    float halfGridWidth = 1;
    float halfGridHeight = 1;
    int rowi = 0, coli = 0;
    [HideInInspector]
    public bool isWhite = false;
    bool canFall = false;
    bool[,] visited;
    List<Tuple<int, int>> eatenChesses = new List<Tuple<int, int>>();
    List<Tuple<int, int>> tempCheeses = new List<Tuple<int, int>>();
    Tuple<int, int> jie = new Tuple<int, int>(-1, -1);
    bool prevJie = false;

    private List<Stone> placeStones;
    List<List<Stone>> answers;
    GameNode root;
    GameNode curnode;
    private List<List<int[,]>> correctBoards = new List<List<int[,]>>();
    private int[,] tboard;//记录上一次棋盘状态
    bool isTry = false;//是否是试下
    private List<Stone> stones = new List<Stone>();//记录要显示的步数
    private int curstep = 0;
    List<GameObject> textStepObjs = new List<GameObject>();//步数文本对象

    bool waitPlay = false;//等待自动落子
    bool canPlay = true;

    private void OnEnable()
    {
        //假设先于SetQuestion执行 不行的话用协程处理setquestion
        if (board == null) board = new int[size, size];
        if (transparent == null) transparent = new bool[size, size];
        if (visited == null) visited = new bool[size, size];
        
        waitPlay = false;
        isTry = false;
        TikuMgr.Instance.GetTikuInfoPanel().ResetTry();

        canPlay = true;
    }

    private void OnDisable()
    {
        canPlay = false;
        prevJie = false;

        //失活意味着换题换了不同大小的棋盘，重置
        Array.Clear(board, 0, board.Length);
        Array.Clear(transparent, 0, transparent.Length);
        Array.Clear(visited, 0, visited.Length);
        stones.Clear();
        board_history.Clear();
        tboard = null;
        correctBoards.Clear();
        root = null;
        curnode = null;
        waitPlay = false; 
    }

    void Awake()
    {
        chesses = new Renderer[size, size];

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
    }

    void Update()
    {
        if (!canPlay) return;
        if (waitPlay) return;

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
            if (FallChess(rowi, coli, isWhite ? 1 : 2))
            {
                MusicMgr.Instance.PlaySound("落子", false);
                if (isTry == true)//试下
                {
                    board_history.Add(board);
                    board = (int[,])board.Clone();
                    isWhite = !isWhite;
                }
                else//做题
                {
                    bool canfind = false;//是否在孩子中找到该步
                    foreach(GameNode node in curnode.childrens)
                    {
                        if(coli==node.x && rowi==node.y)
                        {
                            curnode = node;
                            canfind = true;
                            break;
                        }
                    }

                    if(canfind==false)//解题错误
                    {
                        StartCoroutine("BackBoard");
                        TikuMgr.Instance.GetTikuInfoPanel().ShowTip(false);
                    }
                    else if(canfind==true)//找到
                    {
                        //加入步数UI
                        stones.Add(new Stone(curnode.isWhite, coli, rowi, ++curstep));
                        //Generate
                        GenerateStepTextObjs();

                        if(curnode.childrens.Count==0)//为叶子节点，解题成功
                        {
                            TikuMgr.Instance.GetTikuInfoPanel().ShowTip(true);
                            canPlay = false;//解题成功，不能落子，除非刷新
                            tboard = (int[,])board.Clone();//记录本次棋盘状态
                        }
                        else//还没下完，自动下一步，如果有
                        {
                            GameNode node = curnode.childrens[0];//选择最左边的
                            StartCoroutine("PlayChessLater", node);
                        }
                    }
                }
            }
        }

        DrawBoard(board);
    }

    //经测试发现，setquestion会快于awake和start先执行，就是这个执行顺序
    public void SetQuestion(GameTree t)
    {
        canPlay = false;//初始化时不允许手动下棋

        //题目切换时，大小不变，会在同一棋盘，要清除步数
        stones.Clear();
        ClearStepTextObjs();

        this.placeStones = t.placeStones;
        this.answers = t.answers;

        GameNode node = t.GetRoot();//获得根结点
        root = node;
        curnode = root;

        Array.Clear(board, 0, board.Length);

        int x, y;
        string color;
        foreach (Stone s in placeStones)
        {
            x = s.getX();
            y = s.getY();
            FallChess(y, x, s.getColor() ? 1 : 2);
        }
        board_history.Clear();
        board_history.Add((int[,])board.Clone());
        tboard = (int[,])board.Clone();

        curstep = placeStones.Count;

        //将每个正确题解转换为对应的棋盘状态数组
        for (int i=0;i<answers.Count; i++)
        {
            correctBoards.Add(new List<int[,]>());
            for(int j = 0; j < answers[i].Count; j++)
            {
                FallChess(answers[i][j].getY(), answers[i][j].getX(), answers[i][j].getColor() ? 1 : 2);
                correctBoards[i].Add((int[,])board.Clone());
            }
            board = (int[,])board_history[0].Clone();//到初始状态
        }
        DrawBoard(board);
        canPlay = true;
    }

    //重置棋盘，更新棋盘
    public void ResetBoard()
    {
        isWhite = answers[0][0].getColor() ? true : false;//解题者应该执什么颜色的棋 必有1个正确题解，题解最起码有1步
        board_history.RemoveRange(1, board_history.Count - 1);
        stones.Clear();
        curstep = placeStones.Count;
        ClearStepTextObjs();
        tboard = board_history[0];
        curnode = root;
        SetBoard(0);
        canPlay = true;
    }

    public void SetBoard(int i)
    {
        board = (int[,])board_history[i].Clone();//一定要深拷贝，否则会修改历史棋局
        DrawBoard(board);
    }

    public void ChangeTryCase(bool v)
    {
        isTry = v;
        ResetBoard();
    }

    private IEnumerator PlayChessLater(GameNode node)
    {
        waitPlay = true;
        yield return new WaitForSeconds(0.5f);
        FallChess(node.y, node.x, node.isWhite ? 1 : 2);
        MusicMgr.Instance.PlaySound("落子", false);
        curnode = node;
        tboard = (int[,])board.Clone();//记录本次棋盘状态
        //Add
        stones.Add(new Stone(node.isWhite, node.x, node.y, ++curstep));
        //Generate
        GenerateStepTextObjs();

        if (node.childrens.Count==0)//解题成功
        {
            TikuMgr.Instance.GetTikuInfoPanel().ShowTip(true);
            DrawBoard(board);
            canPlay = false;
        }
        waitPlay = false;
    }

    //回退到上一步
    private IEnumerator BackBoard()
    {
        waitPlay = true;
        yield return new WaitForSeconds(1f);
        board = (int[,])tboard.Clone();//回到上次的状态
        DrawBoard(board);
        waitPlay = false;
    }

    public void ClearStepTextObjs()
    {
        if (textStepObjs.Count == 0) return;
        for (int i = textStepObjs.Count - 1; i >= 0; i--)
        {
            PoolMgr.Instance.PushObj("Text/3dStep", textStepObjs[i]);
        }
        textStepObjs.Clear();
    }

    public void GenerateStepTextObjs()
    {
        ClearStepTextObjs();

        int[,] stepboard = new int[size, size];
        int col, row, step;
        for(int i=0;i<stones.Count;i++)
        {
            col = stones[i].getX();
            row = stones[i].getY();
            step = stones[i].getStep();

            stepboard[row, col] = step;
        }

        for (int i = 0; i < stones.Count; i++)
        {
            col = stones[i].getX();
            row = stones[i].getY();
            step = stones[i].getStep();

            if (board[row, col] == 0) continue;
            if (stepboard[row,col] == step)
            {
                GameObject obj = PoolMgr.Instance.GetObj("Text/3dStep");
                textStepObjs.Add(obj);
                TextStep script = obj.GetComponent<TextStep>();
                if (size == 19)
                    script.InitText(step, !stones[i].getColor(), chesses[row, col].transform.position - new Vector3(0, 0, 0.2f), 0.8f);
                else if (size == 13)
                    script.InitText(step, !stones[i].getColor(), chesses[row, col].transform.position - new Vector3(0, 0, 0.2f), 0.9f);
                else if (size == 9)
                    script.InitText(step, !stones[i].getColor(), chesses[row, col].transform.position - new Vector3(0, 0, 0.2f), 1f);
            }
        }
    }

    private void DrawBoard(int[,] print)
    {
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                if (print[i, j] == 0)
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
                if (print[i, j] == 1)
                {
                    chesses[i, j].material.color = Color.white;
                }
                else if (print[i, j] == 2)
                {
                    chesses[i, j].material.color = Color.black;
                }
            }
        }
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
        Array.Clear(visited, 0, visited.Length);
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
        if (board[i, j] != 0) return false;//有棋子
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
