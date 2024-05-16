using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GoRecognitionHelper
{
    public GoRecognitionHelper() { }

    /// <summary>
    /// 调用python识别图片转换为sgf文件
    /// </summary>
    /// <param name="image_path">被选中的图片路径 注意传入的路径不支持中文</param>
    /// <param name="output_sgf_path">要保存的sgf文件路径</param>
    public bool StartRecognize(string image_path, string output_sgf_path)
    {
        // 要执行的 Python 脚本路径
        string pythonExe = Application.streamingAssetsPath+"\\GoRecognize.exe";

        // 要传递给 Python 脚本的参数
        string scriptArguments = image_path + " " +output_sgf_path;  // 根据需要设置参数

        // 创建一个进程对象来执行 Python 解释器
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonExe,
            Arguments = scriptArguments,  // 将参数添加到命令行
            UseShellExecute = false,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            CreateNoWindow = true
        };

        // 启动进程并执行 Python 脚本
        try
        {
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }
        catch (Exception ex)
        {
            //解析错误
            UnityEngine.Debug.Log("解析错误: " + ex.Message);
            return false;
        }

        return true;
    }
}

