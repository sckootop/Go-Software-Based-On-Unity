using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class FileDialog
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class DialogShow
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] FileDialog dialog);  //这个方法名称必须为GetOpenFileName

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] FileDialog dialog);
    
}

// 这个是提供给C#调用的方法类
public class FileOperationByWin32
{
    public static void OpenSgfFile()
    {
        FileDialog dialog = new FileDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        dialog.filter = "sgf\0*.sgf\0All Files\0*.*\0\0";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //默认路径

        dialog.title = "Select a SGF File";

        dialog.defExt = "sgf";//显示文件的类型
        //注意一下项目不一定要全选 但是0x00000008项不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetOpenFileName(dialog))
        {
            ChessManual cm = ParseSgfHelper.Sgf2Chessmanual(dialog.file);
            BaiPuMgr.Instance.cm= cm;
            int size = cm.getSize();
            if(BaiPuMgr.Instance.curboardsize!=size)
            {
                BaiPuMgr.Instance.curboardsize = size;
                SceneManager.LoadScene("BaiPuScene" + size);
            }
            else
            {
                BaiPuMgr.Instance.GetCurrentBoard().InitChessManual(cm);
                BaiPuMgr.Instance.cm = null;
            }
            
            Debug.Log(dialog.file);
        }
    }

    public static string Get_SavingSgfFilePath()
    {
        FileDialog dialog = new FileDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        dialog.filter = "sgf\0*.sgf\0All Files\0*.*\0\0";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //默认路径

        dialog.title = "Save Game as SGF";

        dialog.defExt = "sgf";//显示文件的类型
        //注意一下项目不一定要全选 但是0x00000008项不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetSaveFileName(dialog))
        {
            Debug.Log(dialog.file);
        }
        return dialog.file;
    }

    public static string Get_OpeningImageFilePath()
    {
        FileDialog dialog = new FileDialog();

        dialog.structSize = Marshal.SizeOf(dialog);

        dialog.filter = "所有图片格式(.bmp;.jpeg;.jpg;.png;)\0*.bmp;*.jpeg;*.jpg;*.png;";

        dialog.file = new string(new char[256]);

        dialog.maxFile = dialog.file.Length;

        dialog.fileTitle = new string(new char[64]);

        dialog.maxFileTitle = dialog.fileTitle.Length;

        dialog.initialDir = UnityEngine.Application.dataPath;  //默认路径

        dialog.title = "Select a Image";

        //dialog.defExt = "所有图片格式(.bmp;.jpeg;.jpg;.png;)";//默认显示文件的类型

        //注意一下项目不一定要全选 但是0x00000008项不要缺少
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR

        if (DialogShow.GetOpenFileName(dialog))
        {
            Debug.Log(dialog.file);
        }

        return dialog.file;
    }
}
