using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QipuchessBoard : MonoBehaviour
{
    private Transform LeftTop;
    private Transform RightBottom;

    Vector3 LTPos;
    Vector3 RBPos;

    public int size;
    float halfGridWidth = 1;
    float halfGridHeight = 1;
    int rowi = 0, coli = 0;

    int[,] board;
    bool[,] transparent;
    List<int[,]> board_history = new List<int[,]>();//棋盘历史状态，用于棋盘快速更新
    List<Stone> move_history = new List<Stone>();//全盘棋子 落子位置历史状态，用于步数快速更新
    Renderer[,] chesses;

    bool[,] visited;
    List<Tuple<int, int>> eatenChesses = new List<Tuple<int, int>>();
    List<Tuple<int, int>> tempCheeses = new List<Tuple<int, int>>();
    Tuple<int, int> jie = new Tuple<int, int>(-1, -1);

    public bool canplay = true;
    public bool isTry = false;
    bool canFall = true;
    bool prevJie = false;
    bool isWhite = false;
    bool initialColor;
    bool showWinRatePanel = false;

    int step;

    ChessManual chessmanual;
    int curindex = 0;//指向当前显示的棋盘board_history下标，0为空
    int tryindex = 0;//试下时predict_board_history的下标，0为第一步

    private string filepath;
    List<MoveInfo> moveinfos = new List<MoveInfo>();//AI预测的步
    List<GameObject> textPercentObjs = new List<GameObject>();//胜率文本对象
    List<GameObject> textStepObjs = new List<GameObject>();//步数文本对象

    List<Stone> stones = new List<Stone>();//记录AI的落子
    //第二个board_history 也是为了回退和前进 同时也要删除被吃子位置的数字
    List<int[,]> predict_board_history = new List<int[,]>();

    TextStep textStep = null;

    // Start is called before the first frame update
    void Start()
    {
        board = new int[size, size];
        chesses = new Renderer[size, size];
        visited = new bool[size, size];
        transparent = new bool[size, size];

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

        step = curindex;

        //获得对局
        chessmanual = QipuChessBoardMgr.Instance.GetChessManual();
        List<Stone> playList = chessmanual.getPlayList();//下棋
        board_history.Add((int[,])board.Clone());//添加空棋盘
        if (playList.Count > 0)
            isWhite = playList[0].getColor();//第一步的棋色
        else
            isWhite = false;//先暂时为黑 正确的应该看PL

        initialColor = isWhite;
        foreach (Stone stone in playList)
        {
            FallChess(stone.getY(), stone.getX(), stone.getColor() ? 1 : 2);
            move_history.Add(stone);
            int[,] t = (int[,])board.Clone();//深拷贝
            board_history.Add(t);
        }
        board = board_history[curindex];//下完了恢复到初始状态

        filepath = QipuChessBoardMgr.Instance.GetFilePath();

        //Katago信息重置
        KatagoHelper.katago.LoadSgf(filepath, 1);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIMgr.Instance.ShowPanel<QipuPausePanel>();
            canplay = false;
            return;
        }

        if (!canplay) return;
        showWinRatePanel = false;

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

        if (colt >= 0 && rowt >= 0 && rowi >= 0 && rowi < size && coli >= 0 && coli < size)//在规定范围内才能落子
        {
            if (board[rowi, coli] == 0)//只有空标志下才能落子 显示半透明
            {
                if(isTry)
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

        if (isTry) DrawTransparentChesses();//绘制透明区域应该放在前面

        if (canFall)
        {
            if (isTry == false)
            {
                isWhite = curindex % 2 == 1 ? !initialColor : initialColor;

                //悬停绿色区域 显示提示UI
                foreach (MoveInfo moveinfo in moveinfos)
                {
                    if (moveinfo.row == rowi && moveinfo.col == coli)
                    {
                        UIMgr.Instance.ShowPanel<XuanDianPanel>().InitText(isWhite, moveinfo.order + 1, moveinfo.winrate);
                        showWinRatePanel = true;
                        break;
                    }
                }
            }

            //鼠标左键点击
            if (Input.GetMouseButtonUp(0) && isTry==false)
            {
                bool clear = false;//记录是否点击到绿色区域
                foreach (MoveInfo moveinfo in moveinfos)
                {
                    //检测是否点到绿色区域，如是显示后续下法
                    if (moveinfo.row == rowi && moveinfo.col == coli)
                    {
                        board = (int[,])board_history[curindex].Clone();//获得当前棋盘状态

                        //进入试下状态 写前面
                        QipuChessBoardMgr.Instance.GetQipuBtnPanel().btnTry.onClick.Invoke();

                        step = curindex;

                        foreach (Tuple<int, int> tuple in moveinfo.predict_moves)
                        {
                            FallChess(tuple.Item2, tuple.Item1, isWhite ? 1 : 2);//tuple第一个是列
                            predict_board_history.Add((int[,])board.Clone());
                            stones.Add(new Stone(isWhite, tuple.Item1, tuple.Item2, ++step));//记录棋子，方便数字显示和回退
                            isWhite = !isWhite;
                        }

                        ClearWinrateTextObjs();
                        //显示棋盘
                        DrawBoard(board);
                        tryindex = moveinfo.predict_moves.Count;//更新当前显示的下标 moveinfo.predict_moves.Count + 一开始的

                        //显示 步数 数字
                        GenerateStepTextObjs();
                        clear = true;

                        //去除之前isTry为false时显示的步数对象
                        PoolMgr.Instance.PushObj("Text/3dStep", textStep.gameObject);
                        //找到退出
                        break;
                    }
                }
                if(clear) moveinfos.Clear();//点到绿色区域，清除AI预测的信息 这个可能要改
            }
            else if (Input.GetMouseButtonUp(0) && isTry == true)
            {
                board = (int[,])predict_board_history[tryindex].Clone();

                if (FallChess(rowi, coli, isWhite ? 1 : 2))
                {
                    //移除
                    if(tryindex < predict_board_history.Count - 1)
                    {
                        print($"predict_board:{tryindex + 1} {predict_board_history.Count - tryindex - 1}");
                        print($"stones:{tryindex + 1} {stones.Count - tryindex}");
                        predict_board_history.RemoveRange(tryindex + 1, predict_board_history.Count - tryindex - 1);
                        stones.RemoveRange(tryindex, stones.Count - tryindex);//注意这里坐标与上面的差别，Count始终少1
                    }
                    //添加
                    predict_board_history.Add((int[,])board.Clone());
                    step = curindex + tryindex + 1;
                    stones.Add(new Stone(isWhite, coli, rowi, step));

                    Play(1);
                    GenerateStepTextObjs();//放在Play后
                }
            }
        }

        if (showWinRatePanel == false) UIMgr.Instance.HidePanel<XuanDianPanel>();
    }

    //历史棋盘play
    public void Play(int delta)
    {
        if (delta == 0) return;
        
        if(isTry==false)
        {
            if (delta > 0)
            {
                if (curindex >= board_history.Count - 1) return;
                curindex = (curindex + delta) < board_history.Count ? curindex + delta : board_history.Count - 1;
                board = (int[,])board_history[curindex].Clone();
                DrawBoard(board_history[curindex]);
                MusicMgr.Instance.PlaySound("落子", false);
            }
            else if (delta < 0)
            {
                if (curindex <= 0) return;
                curindex = (curindex + delta) >= 0 ? curindex + delta : 0;
                board = (int[,])board_history[curindex].Clone();
                DrawBoard(board_history[curindex]);
            }

            if (curindex == 0)
            {
                PoolMgr.Instance.PushObj("Text/3dStep", textStep.gameObject);
                return;
            }
            
            GenerateStepTextObj();
        }
        else if(isTry==true)
        {
            if (delta > 0)
            {
                if (tryindex >= predict_board_history.Count - 1) return;
                tryindex = (tryindex + delta) < predict_board_history.Count ? tryindex + delta : predict_board_history.Count - 1;
                board = (int[,])predict_board_history[tryindex].Clone();
                DrawBoard(predict_board_history[tryindex]);
                isWhite = delta % 2 == 1 ? !isWhite : isWhite;//更新落子颜色

                GenerateStepTextObjs();
                MusicMgr.Instance.PlaySound("落子", false);
            }
            else if (delta < 0)
            {
                if (tryindex <= 0) return;
                tryindex = (tryindex + delta) >= 0 ? tryindex + delta : 0;
                board = (int[,])predict_board_history[tryindex].Clone();
                DrawBoard(predict_board_history[tryindex]);
                isWhite = delta % 2 == -1 ? !isWhite : isWhite;

                GenerateStepTextObjs();
            }
        }
    }

    public IEnumerator AutoPlay()
    {
        for(int i=curindex;i<board_history.Count;i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            Play(1);
        }
    }

    public void ResetBoard()
    {
        curindex = 0;
        isWhite = initialColor;
        DrawBoard(board_history[curindex]);
    }

    public void SetBoard(int index)
    {
        curindex = index;
        DrawBoard(board_history[curindex]);
    }

    public void CleanBoard()
    {
        predict_board_history.Clear();
        stones.Clear();
        tryindex = 0;

        moveinfos.Clear();

        DrawBoard(board_history[curindex]);
    }

    public void ClearWinrateTextObjs()
    {
        if (textPercentObjs.Count == 0) return;
        for (int i = textPercentObjs.Count-1; i>=0; i--)
        {
            PoolMgr.Instance.PushObj("Text/3dPercent", textPercentObjs[i]);
        }
        textPercentObjs.Clear();
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

    public void GenerateStepTextObj()
    {
        //单个步数对象显示
        if (textStep == null || textStep.gameObject.activeSelf == false)
            textStep = PoolMgr.Instance.GetObj("Text/3dStep").GetComponent<TextStep>();
        Stone stone = move_history[curindex - 1];
        if (size == 19)
            textStep.InitText(stone.getStep(), !stone.getColor(), chesses[stone.getY(), stone.getX()].transform.position - new Vector3(0, 0, 0.2f), 0.8f);
        else if(size==13)
            textStep.InitText(stone.getStep(), !stone.getColor(), chesses[stone.getY(), stone.getX()].transform.position - new Vector3(0, 0, 0.2f), 0.9f);
        else if(size==9)
            textStep.InitText(stone.getStep(), !stone.getColor(), chesses[stone.getY(), stone.getX()].transform.position - new Vector3(0, 0, 0.2f), 1f);
    }

    public void GenerateStepTextObjs()
    {
        ClearStepTextObjs();//清除

        int[,] tboard = (int[,])predict_board_history[tryindex].Clone();
        int[,] stepboard = new int[size,size];
        int col, row, step;
        Stone stone;
        //两次遍历
        for (int i=0; i<=tryindex-1; i++)
        {
            stone = stones[i];
            row = stone.getY();
            col = stone.getX();
            step = stone.getStep();
            if (tboard[row, col] == 0) continue;

            stepboard[row, col] = step;//更新步数 步数有可能被吃掉然后被新步数覆盖
        }

        for (int i = 0; i <= tryindex - 1; i++)
        {
            stone = stones[i];
            row = stone.getY();
            col = stone.getX();
            step = stone.getStep();
            if (tboard[row, col] == 0) continue;

            if (stepboard[row,col]==step)
            {
                GameObject obj = PoolMgr.Instance.GetObj("Text/3dStep");
                textStepObjs.Add(obj);
                TextStep script = obj.GetComponent<TextStep>();
                if (size == 19)
                    script.InitText(step, !stone.getColor(), chesses[row, col].transform.position - new Vector3(0, 0, 0.2f), 0.8f);
                else if(size==13)
                    script.InitText(step, !stone.getColor(), chesses[row, col].transform.position - new Vector3(0, 0, 0.2f), 0.9f);
                else if(size==9)
                    script.InitText(step, !stone.getColor(), chesses[row, col].transform.position - new Vector3(0, 0, 0.2f), 1f);
            }
        }
    }

    public void AIAnalyze()
    {
        board = (int[,])board_history[curindex].Clone();
        canplay = false;
        KatagoHelper.katago.LoadSgf(filepath, curindex + 1);
        Task.Run(async () =>
        {
            moveinfos = await KatagoHelper.katago.Analyze(6);//获得了AI预测的下法
            moveinfos.Sort();
        }).ContinueWith(task =>
        {
            float value = .8f;
            
            //显示绿色区域以及着点胜率
            foreach (MoveInfo moveinfo in moveinfos)
            {
                chesses[moveinfo.row, moveinfo.col].material.color = new Color(0.45f, 1, 0.35f, value);
                GameObject obj = PoolMgr.Instance.GetObj("Text/3dPercent");
                textPercentObjs.Add(obj);
                TextPercent script = obj.GetComponent<TextPercent>();
                if (size == 19)
                    script.InitText(moveinfo.winrate, chesses[moveinfo.row, moveinfo.col].transform.position, 0.8f);
                else if(size==13)
                    script.InitText(moveinfo.winrate, chesses[moveinfo.row, moveinfo.col].transform.position, 0.9f);
                else if(size==9)
                    script.InitText(moveinfo.winrate, chesses[moveinfo.row, moveinfo.col].transform.position, 1f);

                value = value - 0.4f / (moveinfos.Count - 1);
            };

            //右侧RecommendMovesPanel面板更新
            QipuChessBoardMgr.Instance.GetRecommendMovesPanel().Clear();
            bool twhite = curindex % 2 == 1 ? !initialColor : initialColor;//当前棋应是什么颜色
            foreach (MoveInfo moveinfo in moveinfos)
            {
                RecommendMovesItem item = Instantiate(Resources.Load<GameObject>("RecommendMovesItem")).GetComponent<RecommendMovesItem>();
                item.InitInfo(moveinfo, twhite);
                QipuChessBoardMgr.Instance.GetRecommendMovesPanel().AddItem(item);
            }
            canplay = true;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void InitPredictBoard()
    {
        predict_board_history.Clear();
        predict_board_history.Add((int[,])board_history[curindex].Clone());//初始化predict_board_history，0坐标为原始棋盘
    }

    public void ShowPredictMoves(MoveInfo moveinfo)
    {
        if (moveinfos.Count != 0) moveinfos.Clear();

        InitPredictBoard();
        stones.Clear();

        //进入试下状态 写前面
        isTry = true;
        QipuChessBoardMgr.Instance.GetQipuBtnPanel().EnterTryCondition();

        isWhite = curindex % 2 == 1 ? !initialColor : initialColor;
        board = (int[,])board_history[curindex].Clone();//获得当前棋盘状态
        step = curindex;

        foreach (Tuple<int, int> tuple in moveinfo.predict_moves)
        {
            FallChess(tuple.Item2, tuple.Item1, isWhite ? 1 : 2);//tuple第一个是列
            predict_board_history.Add((int[,])board.Clone());
            stones.Add(new Stone(isWhite, tuple.Item1, tuple.Item2, ++step));//记录棋子，方便数字显示和回退
            isWhite = !isWhite;
        }

        ClearWinrateTextObjs();
        //显示棋盘
        DrawBoard(board);
        tryindex = moveinfo.predict_moves.Count;//更新当前显示的下标 moveinfo.predict_moves.Count + 一开始的1个

        //显示 步数 数字
        GenerateStepTextObjs();

        //如果有的话，去除之前isTry为false时显示的步数对象
        PoolMgr.Instance.PushObj("Text/3dStep", textStep.gameObject);
    }

    private void OnDestroy()
    {
        QipuChessBoardMgr.Instance.Destory();
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

    private void DrawTransparentChesses()
    {
        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                if (board[i,j]==0)
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
        }
    }


    #region 围棋逻辑
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