using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class KatagoHelper
{
    public static Katago katago;
}

public class Katago
{

    string katagoPath = Application.streamingAssetsPath + "\\katago-v1.13.0-opencl-windows-x64\\katago.exe";
    string configPath = Application.streamingAssetsPath + "\\katago-v1.13.0-opencl-windows-x64\\gtp_custom.cfg";
    string modelPath = Application.streamingAssetsPath + "\\katago-v1.13.0-opencl-windows-x64\\model.bin";
    Process katagoProcess = new Process();

    
    public Katago()
    {
        katagoProcess.StartInfo.FileName = katagoPath;
        katagoProcess.StartInfo.Arguments = $"gtp -model {modelPath} -config {configPath}";
        katagoProcess.StartInfo.UseShellExecute = false;
        katagoProcess.StartInfo.CreateNoWindow = true;
        katagoProcess.StartInfo.RedirectStandardInput = true;
        katagoProcess.StartInfo.RedirectStandardOutput = true;
        katagoProcess.StartInfo.RedirectStandardError = true;

        katagoProcess.Start();

        Thread printThread = new Thread(() =>
        {
            while (!katagoProcess.StandardError.EndOfStream)
            {
                var data = katagoProcess.StandardError.ReadLine();
                if (!string.IsNullOrEmpty(data))
                {
                    UnityEngine.Debug.Log("KataGo: " + data);
                }
            }
        });
        printThread.Start();
    }

    ~Katago()
    {
        Close();
    }

    public void Close()
    {
        katagoProcess?.Kill();
        katagoProcess?.Close();
        katagoProcess?.Dispose();
    }

    /// <summary>
    /// 向katago引擎发送加载sgf文件的命令
    /// </summary>
    /// <param name="path">客户端传来的路径</param>
    public void LoadSgf(string path)
    {
        katagoProcess.StandardInput.WriteLine("loadsgf " + path);//发送命令
        katagoProcess.StandardInput.Flush();//确保命令发送

        StreamReader sr = katagoProcess.StandardOutput;//获取返回值 

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log("Sgf加载完成: " + outputData);
    }

    public void LoadSgf(string path,int step)
    {
        katagoProcess.StandardInput.WriteLine("loadsgf " + path + " " + step);
        katagoProcess.StandardInput.Flush();//确保命令发送

        StreamReader sr = katagoProcess.StandardOutput;//获取返回值 

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log("Sgf加载到第" + step + "步");
    }

    public GameResult Get_Final_Score()
    {
        katagoProcess.StandardInput.WriteLine("final_score");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;
        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }
        UnityEngine.Debug.Log("对局结果分析成功: " + outputData);
        GameResult result = new GameResult();
        result.String2Result(outputData.ToString());
        return result;
    }

    //Debug用
    public void ShowBoard()
    {
        katagoProcess.StandardInput.WriteLine("showboard");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;
        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }
        UnityEngine.Debug.Log("显示棋盘: " + outputData);
    }

    public void SetKomi(float mushu)
    {
        katagoProcess.StandardInput.WriteLine("komi " + mushu);
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;
        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }
        UnityEngine.Debug.Log("修改贴目数为:" + mushu);
    }

    public void ChangeBoardSize(int size)
    {
        katagoProcess.StandardInput.WriteLine("boardsize " + size);
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log("修改棋盘大小: " + size);
    }

    public async Task<List<MoveInfo>> Analyze(int maxmoves)
    {
        katagoProcess.StandardInput.WriteLine($"lz-genmove_analyze maxmoves {maxmoves}");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        List<MoveInfo> moves = new List<MoveInfo>();
        string regex= @"info move [A-Z]\d+ visits \d+ winrate \d+ prior \d+ lcb \d+ order \d+ pv ((([A-Z]\d+ )|([A-Z]\d+))+)";
        MatchCollection tokens = Regex.Matches(outputData.ToString(), regex);
        foreach (Match match in tokens)
        {
            moves.Add(new MoveInfo(match.Value));
        }

        UnityEngine.Debug.Log("下一步分析完成");

        return moves;
    }

    public void ClearCache()
    {
        katagoProcess.StandardInput.WriteLine("clear_cache");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log("清除缓存成功");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="col">下标0开始</param>
    /// <param name="row">下标0开始</param>
    public void Play(char color,int col,int row)
    {
        katagoProcess.StandardInput.WriteLine($"play {color} {ParseSgfHelper.alphabet[col]}{row + 1}");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log("落子成功");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">w or b</param>
    /// <returns>D16这样坐标位置 or pass</returns>
    public async Task<string> Genmove(char color)
    {
        katagoProcess.StandardInput.WriteLine($"genmove {color}");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log("AI落子");
        return outputData.ToString().Split(' ')[1].Trim();
    }

    public void Clear_Board()
    {
        katagoProcess.StandardInput.WriteLine("clear_board");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log("重置盘面");
    }

    //Debug用
    public void PrintSgf()
    {
        katagoProcess.StandardInput.WriteLine("printsgf");
        katagoProcess.StandardInput.Flush();

        StreamReader sr = katagoProcess.StandardOutput;

        var outputData = new StringBuilder();
        string line;

        while ((line = sr.ReadLine()) != "")
        {
            outputData.Append(line + '\n');
        }

        UnityEngine.Debug.Log(outputData);
    }
}

public class MoveInfo : IComparable<MoveInfo>
{
    //外面分割 传入一个这样的字符串 注意预测的第一步是move
    //info move Q4 visits 2584 winrate 3815 prior 2447 lcb 3803 order 0 pv Q4 D16 F17 R3 R4 Q3 P4 O2 C14 E14 C12 O17 M17 N3
    public MoveInfo(string info)
    {
        string regex = @"move (\w*)";
        MatchCollection tokens = Regex.Matches(info, regex);
        foreach (Match match in tokens)
        {
            if (match.Success)
            {
                string pos = match.Groups[1].Value;
                col = alphabet[pos[0]];
                row = int.Parse(pos.Substring(1)) - 1;
            }
        }

        regex = @"winrate (\w*)";
        tokens = Regex.Matches(info, regex);
        foreach (Match match in tokens)
        {
            if (match.Success)
            {
                winrate = Math.Round(double.Parse(match.Groups[1].Value) / 100d, 2);
            }
        }

        regex = @"order (\w*)";
        tokens = Regex.Matches(info, regex);
        foreach (Match match in tokens)
        {
            if (match.Success)
            {
                order = int.Parse(match.Groups[1].Value);
            }
        }

        regex = @"pv (\w+ |\w+)+";
        tokens = Regex.Matches(info, regex);
        foreach (Match match in tokens)
        {
            if (match.Success)
            {
                string pvMoves = match.Value.Trim().Substring(3);
                string[] positions = pvMoves.Split(' ');

                foreach (string pos in positions)
                {
                    predict_moves.Add(new Tuple<int, int>(alphabet[pos[0]], int.Parse(pos.Substring(1)) - 1));//列 行
                }
            }
        }
    }

    private static Dictionary<char, int> alphabet = new Dictionary<char, int>
        {
            {'A',0 },{'B',1},{'C',2},{'D',3},{'E',4},{'F',5},{'G',6},{'H',7},{'J',8},{'K',9},{'L',10},{'M',11},{'N',12},{'O',13},{'P',14},
            {'Q',15},{'R',16},{'S',17},{'T',18},{'U',19},{'V',20},{'W',21},{'X',22},{'Y',23},{'Z',24}
        };

    public int col { get; set; }
    public int row { get; set; }
    public double winrate { get; set; }
    public int order { get; set; }
    public List<Tuple<int, int>> predict_moves { get; set; } = new List<Tuple<int, int>>();//item1

    public static bool operator ==(MoveInfo a, MoveInfo b)
    {
        return a.order == b.order ? true : false;
    }

    public static bool operator !=(MoveInfo a, MoveInfo b)
    {
        return a.order == b.order ? false : true;
    }

    public static bool operator <(MoveInfo a, MoveInfo b)
    {
        return a.order < b.order ? true : false;
    }

    public static bool operator >(MoveInfo a, MoveInfo b)
    {
        return a.order > b.order ? true : false;
    }

    public int CompareTo(MoveInfo info)
    {
        if (info == null) return 1;

        if (this.order > info.order) return -1;//order越小越大
        else if (this.order == info.order) return 0;
        else return 1;
    }
}
