using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ParseSgfHelper
{
    public static string alphabet = "ABCDEFGHJKLMNOPQRSTUVWXYZ";

    // 拿到文件内容
    public static string GetContent(string path)
    {
        return File.ReadAllText(path);
    }

    public static void SaveContent(string despath, string content)
    {
        File.WriteAllText(despath, content);
    }

    public static void SaveContent(string despath, ChessManual cm)
    {
        File.WriteAllText(despath, Chessmanual2Sgf(cm));
    }

    /*
     * 由于在正则表达式中“ \ ”、“ ? ”、“ * ”、“ ^ ”、“ $ ”、“ + ”、“（”、“）”、“ | ”、“ { ”、“ [ ”等字符已经具有一定特殊意义，
     * 如果需要用它们的原始意义，则应该对它进行转义，例如希 望在字符串中至少有一个“ \ ”，那么正则表达式应该这么写： \\+ 。
     */
    //这个方法没用
    public static List<string> GetStrContainData(string str, string start, string end)
    {
        List<string> result = new List<string>();
        string regex = Regex.Escape(start) + "(.*?)" + Regex.Escape(end);
        Regex pattern = new Regex(regex);
        MatchCollection matches = pattern.Matches(str);
        foreach (Match match in matches)
        {
            string key = match.Groups[1].Value;
            if (!key.Contains(start) && !key.Contains(end))
            {
                result.Add(key);
            }
        }
        return result;
    }

    public static string Chessmanual2Sgf(ChessManual chessmanual)
    {
        StringBuilder builder = new StringBuilder(2000);
        builder.Append("(;");
        builder.Append(@$"FF[{ChessManual.sgfVersion}]");//sgf规范
        builder.Append($"GM[{chessmanual.gameType}]");//游戏类型
        builder.Append($"US[{chessmanual.getCreator()}]");//创建者
        builder.Append($"GN[{chessmanual.getGN()}]");//比赛名
        builder.Append($"RU[{chessmanual.getRule()}]");//比赛规则
        builder.Append($"SZ[{chessmanual.getSize()}]");//棋盘大小
        builder.Append($"PB[{chessmanual.getBlackName()}]");//黑方姓名
        builder.Append($"BR[{chessmanual.getBlackRank()}]");//黑方段位
        builder.Append($"PW[{chessmanual.getWhiteName()}]");//白方姓名
        builder.Append($"WR[{chessmanual.getWhiteRank()}]");//白方段位
        builder.Append($"RE[{chessmanual.getResult()}]");//棋局结果
        builder.Append($"KM[{chessmanual.getTiemu()}]");//获得黑棋贴目数
        builder.Append($"DT[{chessmanual.getTime()}]");//获得日期

        byte[] btNumber = new byte[1];
        ASCIIEncoding asciiEncoding = new ASCIIEncoding();
        //下棋
        List<Stone> list = chessmanual.getPlayList();
        foreach(Stone stone in list)
        {
            btNumber[0] = (byte)(stone.getX() + 97);
            string col = asciiEncoding.GetString(btNumber);
            btNumber[0] = (byte)(stone.getY() + 97);
            string row = asciiEncoding.GetString(btNumber);
            builder.Append(string.Format(";{0}[{1}{2}]", stone.getColor() ? 'W' : 'B', col, row));
        }
        builder.Append(")");
        return builder.ToString();
    }

    public static ChessManual Sgf2Chessmanual(string path)
    {
        string sgfcontent = GetContent(path);
        ChessManual chessManual = new ChessManual(sgfcontent);
        return chessManual;
    }
}
