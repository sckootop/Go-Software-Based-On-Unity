using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

//走法树中的节点
public class GameNode
{
    public GameNode parent;
    public List<GameNode> childrens = new List<GameNode>();
    public bool isWhite;
    public int x;//列
    public int y;//行

    public GameNode() { }

    public GameNode(GameNode p, bool _color, int _x, int _y)
    {
        this.parent = p;
        this.isWhite = _color;
        this.x = _x;
        this.y = _y;
    }

    public GameNode(GameNode _parent)
    {
        this.parent = _parent;
    }
}

public class GameTree
{
    //这个正则表达式规则是来解析sgf文件的
    //匹配非开头的'('，非结尾的')'，B[pd]
    private static string regexPattern = @"(?<!^)\((?!\))|(?<!\()\)(?!$)|;(?<color>[BW])\[(?<pos>.*?)\]";
    private static string divideAB = @"AB(\[([a-z]{2})\]|\r\n)+";
    private static string divideAW = @"AW(\[([a-z]{2})\]|\r\n)+";
    private static string regexSize = @"(?<key>(SZ))(\[(?<value>.*?)\])";
    private GameNode root = new GameNode();//走法树的根 指向摆好的最后一个棋子 题目必有摆好的棋子
    public List<Stone> placeStones { get; } = new List<Stone>();//记录棋盘上摆好的棋子
    public List<List<Stone>> answers { get; } = new List<List<Stone>>();//记录题目的正解
    public int size { get; }

    public GameTree(string sgfContent)
    {
        //解析棋盘大小
        MatchCollection sizetoken = Regex.Matches(sgfContent, regexSize);
        Match[] sizeinfo = sizetoken.Cast<Match>().ToArray();
        if (sizeinfo[0].Groups["key"].Success)
        {
            size = int.Parse(sizeinfo[0].Groups["value"].Value);
        }

        //解析预先摆放的棋子
        //AB
        MatchCollection tokens = Regex.Matches(sgfContent, divideAB, RegexOptions.Singleline);
        Match[] abStones = tokens.Cast<Match>().ToArray();
        foreach (Match match in abStones)
        {
            if (match.Success)
            {
                Group group = match.Groups[2];
                foreach (Capture capture in group.Captures)
                {
                    int col = capture.Value[0] - 97;
                    int row = size - capture.Value[1] + 96;
                    placeStones.Add(new Stone(false, col, row));//黑棋
                }
            }
        }
        //AW
        tokens = Regex.Matches(sgfContent, divideAW, RegexOptions.Singleline);
        Match[] awStones = tokens.Cast<Match>().ToArray();
        foreach (Match match in awStones)
        {
            if (match.Success)
            {
                Group group = match.Groups[2];
                foreach (Capture capture in group.Captures)
                {
                    int col = capture.Value[0] - 97;
                    int row = size - capture.Value[1] + 96;
                    placeStones.Add(new Stone(true, col, row));//白棋
                }
            }
        }
        //让curnode变为摆好的最后一个棋子的信息
        GameNode curnode = root;
        curnode.x = placeStones[placeStones.Count - 1].getX();
        curnode.y = placeStones[placeStones.Count - 1].getY();
        curnode.isWhite = placeStones[placeStones.Count - 1].getColor();//这个什么颜色无所谓 因为node里记录了颜色

        //解析后续落子
        tokens = Regex.Matches(sgfContent, regexPattern, RegexOptions.Singleline);
        Match[] infos = tokens.Cast<Match>().ToArray();

        int index = 0;
        createTree(ref index, infos, curnode);//开始构造
        GetTimu();
    }

    //构建一个走法树
    //注意i要加ref，不然构建完一个子树后，i还是在原来的位置
    public void createTree(ref int i, Match[] infos, GameNode _node)
    {
        GameNode currentNode = _node;

        while (i < infos.Length)
        {
            Match info = infos[i];
            if (info.Groups["color"].Success) // 棋子
            {
                //创建新节点 并设置父亲
                GameNode newnode = new GameNode(_parent: currentNode);
                currentNode.childrens.Add(newnode);
                currentNode = newnode;
                // 设置当前节点信息
                currentNode.x = info.Groups["pos"].Value[0] - 97;//列 a为0 b为1
                currentNode.y = size - info.Groups["pos"].Value[1] + 96;//行 a为size-(a-96)
                currentNode.isWhite = info.Groups["color"].Value == "W" ? true : false;
                i++;
            }
            else if (info.Value == "(") // 开始新子树
            {
                i++; // 移动到下一个信息
                     //建立新树
                createTree(ref i, infos, currentNode); //以currentNode为根节点 递归构建新子树
            }
            else if (info.Value == ")") // 结束当前子树
            {
                i++; // 移动到下一个信息
                return; // 返回当前节点，结束这一层的递归
            }
            else
            {
                // 未知的信息或格式错误，可能需要进行错误处理或跳过
                i++;
            }
        }
    }

    //中序遍历
    public void midorderDFS(GameNode node)
    {
        if (node == null) return;
        Console.WriteLine("颜色:" + (node.isWhite == true ? "白" : "黑") + " " + "位置:" + "[" + node.x + "," + node.y + "]" + " 父节点位置:" + ((node.parent == null) ? "*" : (node.parent.isWhite == true ? "白" : "黑") + "[" + node.parent.x + "," + node.parent.y + "]"));
        for (int i = 0; i < node.childrens.Count; ++i)
        {
            midorderDFS(node.childrens[i]);
        }
    }

    public void midorderDFS()
    {
        midorderDFS(this.root);
    }

    //获得所有从node开始到每个叶子节点的路径到allPaths，顺序是从左到右
    private void FindPaths(GameNode node, List<Stone> currentPath)
    {
        if (node == null) return;

        // 将当前节点添加到路径中
        currentPath.Add(new Stone(node.isWhite, node.x, node.y));

        // 如果当前节点是叶子节点，将它的路径添加到结果列表中
        if (node.childrens.Count == 0)
        {
            answers.Add(new List<Stone>(currentPath));//记录解法
        }
        else
        {
            // 否则，遍历它的子节点
            foreach (var child in node.childrens)
            {
                FindPaths(child, currentPath);
            }
        }

        // 回溯：在返回之前移除当前节点
        currentPath.RemoveAt(currentPath.Count - 1);
    }

    //获得题目
    public void GetTimu()
    {
        List<Stone> curpath = new List<Stone>();
        FindPaths(this.root, curpath);

        foreach (var path in answers)
        {
            path.RemoveAt(0);//移除根结点，保留正解
        }
    }

    public GameNode GetRoot()
    {
        return root;
    }
}

