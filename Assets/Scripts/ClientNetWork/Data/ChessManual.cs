using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public struct Stone
{
    private bool isWhite;//true为白 false为黑
    private int x;//列
    private int y;//行
    private int step;

    public Stone(bool color, int x, int y, int step)
    {
        this.isWhite = color;
        this.x = x;
        this.y = y;
        this.step = step;
    }

    public Stone(bool color, int x, int y)
    {
        this.isWhite = color;
        this.x = x;
        this.y = y;
        this.step = 0;//结构体必须赋值
    }

    public static bool operator ==(Stone a,Stone b)
    {
        if(a.x==b.x && a.y==b.y && a.isWhite ==b.isWhite && a.step==b.step)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Stone a, Stone b)
    {
        if (a.x == b.x && a.y == b.y && a.isWhite == b.isWhite && a.step == b.step)
        {
            return false;
        }
        return true;
    }

    public bool getColor()
    {
        return isWhite;
    }

    public void setColor(bool color)
    {
        this.isWhite = color;
    }

    public int getX()
    {
        return this.x;
    }

    public void setX(int x)
    {
        this.x = x;
    }

    public int getY()
    {
        return this.y;
    }

    public void setY(int y)
    {
        this.y = y;
    }

    public int getStep()
    {
        return step;
    }

    public void setStep(int i)
    {
        this.step = i;
    }
}

//存储一场比赛需要 将类中的信息转换为sgf文本格式
public class ChessManual
{
    private static string regexPattern = @"(?<key>[A-Z]{1,2})(\[(?<value>.*?)\]){1,}";
    
    private string gameName;//比赛名
    private string time;//比赛时间
    private float tiemu;//贴目
    private string result;//结果
    private string place;//地点
    private string rule;//比赛规则

    public const short sgfVersion = 4;//sgf格式规范版本
    public short gameType = 1;//游戏类型 默认1 围棋
    private string creator;//sgf文件创建者
    private string playerBlack;//黑方
    private string blackRank;//黑方水平
    private string playerWhite;//白方
    private string whiteRank;//白方水平

    private int boardsize = 19;
    private List<Stone> playList=new List<Stone>();//落子顺序，对于对局，落子是唯一的，不用走法树

    public ChessManual() { }
    public ChessManual(string gameName, string time, string place, float tiemu, string result, string playerBlack,
            string blackRank, string playerWhite, string whiteRank)
    {
        this.gameName = gameName;
        this.time = time;
        this.place = place;
        this.tiemu = tiemu;
        this.result = result;
        this.playerBlack = playerBlack;
        this.blackRank = blackRank;
        this.playerWhite = playerWhite;
        this.whiteRank = whiteRank;
    }

    /// <summary>
    /// 构建游戏sgf文件
    /// </summary>
    /// <param name="sgfContent">包括全部内容</param>
    public ChessManual(string sgfContent)
    {
        int step = 0;
        MatchCollection tokens = Regex.Matches(sgfContent, regexPattern);
        foreach (Match token in tokens)
        {
            switch (token.Groups["key"].Value)
            {
                case "B"://黑棋下了一手
                    string bpos = token.Groups["value"].Value;
                    int bcol = bpos[0] - 97, brow = bpos[1] - 97;
                    playList.Add(new Stone(false, bcol, brow, ++step));
                    break;
                case "W"://白棋下了一手
                    string wpos = token.Groups["value"].Value;
                    int wcol = wpos[0] - 97, wrow = wpos[1] - 97;
                    playList.Add(new Stone(true, wcol, wrow, ++step));
                    break;
                case "GN"://比赛名
                    gameName = token.Groups["value"].Value;
                    break;
                case "DT"://比赛时间
                    time = token.Groups["value"].Value;
                    break;
                case "FF"://sgf文件格式规范版本
                    break;
                case "GM"://游戏种类 1为围棋
                    gameType = short.Parse(token.Groups["value"].Value);
                    break;
                case "AN"://对局注解者
                    break;
                case "PC"://比赛地点
                    place = token.Groups["value"].Value;
                    break;
                case "US"://该sgf文件创建者
                    creator = token.Groups["value"].Value;
                    break;
                case "SZ"://棋盘尺寸
                    int size = int.Parse(token.Groups["value"].Value);
                    boardsize = size;
                    break;
                case "TM"://时长限制
                    break;
                case "PB"://黑方玩家名字
                    playerBlack = token.Groups["value"].Value;
                    break;
                case "PW"://白方玩家名字
                    playerWhite = token.Groups["value"].Value;
                    break;
                case "WR"://白方水平 ..d ..k ..p
                    whiteRank = token.Groups["value"].Value;
                    break;
                case "BR"://黑方水平
                    blackRank = token.Groups["value"].Value;
                    break;
                case "KM"://贴目数 5.5 0 -10
                    tiemu = float.Parse(token.Groups["value"].Value);
                    break;
                case "AB"://在下子之前放在棋盘上的黑子
                    break;
                case "AW"://在下子之前放在棋盘上的白子
                    break;
                case "RU"://围棋规则  Japanese, Chinese, AGA, GOE etc.
                    rule = token.Groups["value"].Value;
                    break;
                case "HA"://让子数
                    break;
                case "CA"://字符集编码
                    break;
                case "RE"://比赛结果
                    result = token.Groups["value"].Value;
                    break;
                case "PL"://用于围棋题目 表示起手是哪方
                    break;
            }
        }
    }

    public string getGN()
    {
        return gameName;
    }

    public void setGN(string gameName)
    {
        this.gameName = gameName;
    }

    public string getTime()
    {
        return time;
    }

    public void setTime(string time)
    {
        this.time = time;
    }

    public string getPlace()
    {
        return place;
    }

    public void setPlace(string place)
    {
        this.place = place;
    }

    public float getTiemu()
    {
        return tiemu;
    }

    public void setTiemu(float tiemu)
    {
        this.tiemu = tiemu;
    }

    public string getResult()
    {
        return result;
    }

    public void setResult(string result)
    {
        this.result = result;
    }

    public string getBlackName()
    {
        return playerBlack;
    }

    public void setBlackName(string playerBlack)
    {
        this.playerBlack = playerBlack;
    }

    public string getBlackRank()
    {
        return blackRank;
    }

    public void setBlackRank(string blackRank)
    {
        this.blackRank = blackRank;
    }

    public string getWhiteName()
    {
        return playerWhite;
    }

    public void setWhiteName(string playerWhite)
    {
        this.playerWhite = playerWhite;
    }

    public string getWhiteRank()
    {
        return whiteRank;
    }

    public void setWhiteRank(string whiteRank)
    {
        this.whiteRank = whiteRank;
    }

    public void AddStone(Stone point)
    {
        playList.Add(point);
    }

    public List<Stone> getPlayList()
    {
        return playList;
    }

    public void setPlayList(List<Stone> playList)
    {
        this.playList = playList;
    }

    public void setCreator(string name)
    {
        this.creator = name;
    }

    public string getCreator()
    {
        return creator;
    }

    public int getSize()
    {
        return boardsize;
    }

    public void setSize(int size)
    {
        this.boardsize = size;
    }

    public short getGameType()
    {
        return gameType;
    }

    public void setGameType(short type=1)
    {
        this.gameType = type;
    }

    public void setRule(string rule)
    {
        this.rule = rule;
    }

    public string getRule()
    {
        return this.rule;
    }
}
