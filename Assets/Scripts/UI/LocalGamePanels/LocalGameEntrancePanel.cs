using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LocalGameEntrancePanel : BasePanel
{
    public Text txtSize;
    public Button btnSizeLeft;
    public Button btnSizeRight;

    public Text txtMode;
    public Button btnModeLeft;
    public Button btnModeRight;

    string[] SizeSelections = new string[] { "9×9", "13×13", "19×19" };
    private int sizeindex = 2;
    string[] ModeSelections = new string[] { "摆谱模式", "玩家约战", "人机对弈" };
    private int modeindex = 0;

    public Button btnStart;
    public Button btnReturn;

    protected override void Init()
    {
        txtSize.text = SizeSelections[2];
        txtMode.text = ModeSelections[modeindex];

        btnModeLeft.onClick.AddListener(() =>
        {
            modeindex = modeindex - 1 >= 0 ? modeindex - 1 : ModeSelections.Length - 1;
            txtMode.text = ModeSelections[modeindex];
        });

        btnModeRight.onClick.AddListener(() =>
        {
            modeindex = modeindex + 1 < ModeSelections.Length ? modeindex + 1 : 0;
            txtMode.text = ModeSelections[modeindex];
        });

        btnSizeLeft.onClick.AddListener(() =>
        {
            sizeindex = sizeindex - 1 >= 0 ? sizeindex - 1 : SizeSelections.Length - 1;
            txtSize.text = SizeSelections[sizeindex];
        });

        btnSizeRight.onClick.AddListener(() =>
        {
            sizeindex = sizeindex + 1 < SizeSelections.Length ? sizeindex + 1 : 0;
            txtSize.text = SizeSelections[sizeindex];
        });

        btnStart.onClick.AddListener(() =>
        {
            StartCoroutine("LoadSceneAndDoSomething");
        });

        btnReturn.onClick.AddListener(() =>
        {
            UIMgr.Instance.ShowPanel<PlayPanel>();
            UIMgr.Instance.HidePanel<LocalGameEntrancePanel>();
        });
    }

    private IEnumerator LoadSceneAndDoSomething()
    {
        if(modeindex==0)
        {
            int size = 0;
            if (sizeindex == 0) size = 9;
            else if (sizeindex == 1) size = 13;
            else if (sizeindex == 2) size = 19;
            BaiPuMgr.Instance.curboardsize = size;
            yield return SceneManager.LoadSceneAsync("BaiPuScene" + size);
        }
        else if(modeindex==1)
        {
            yield return SceneManager.LoadSceneAsync("LocalGameScene");
            // 在场景加载完毕后执行操作
            int size = 0;
            if (sizeindex == 0) size = 9;
            else if (sizeindex == 1) size = 13;
            else if (sizeindex == 2) size = 19;
            LocalgameChessBoardMgr.Instance.ShowChessBoard(size); 
        }
        else if(modeindex==2)
        {
            int size = 0;
            if (sizeindex == 0) size = 9;
            else if (sizeindex == 1) size = 13;
            else if (sizeindex == 2) size = 19;
            BaiPuMgr.Instance.curboardsize = size;
            yield return SceneManager.LoadSceneAsync("PVEScene" + size);
        }
        UIMgr.Instance.HideAll();
    }
}
