using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitPanel : BasePanel
{
    public Text txt;

    protected override void Init()
    {
        updateActions += () =>
        {
            if (OnlinegameChessBoardMgr.Instance.readytoLoad == true)
            {
                OnlinegameChessBoardMgr.Instance.readytoLoad = false;
                //匹配成功加载场景
                StartCoroutine("LoadSceneAndDoSomething");
            }
        };

        StartCoroutine(AutoTxt());
    }

    IEnumerator AutoTxt()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            txt.text = "正在匹配旗鼓相当的对手";
            yield return new WaitForSecondsRealtime(0.5f);
            txt.text = "正在匹配旗鼓相当的对手.";
            yield return new WaitForSecondsRealtime(0.5f);
            txt.text = "正在匹配旗鼓相当的对手..";
            yield return new WaitForSecondsRealtime(0.5f);
            txt.text = "正在匹配旗鼓相当的对手...";
        }
    }

    private IEnumerator LoadSceneAndDoSomething()
    {
        yield return SceneManager.LoadSceneAsync("OnlineGameScene" + OnlinegameChessBoardMgr.Instance.boardsize);
        // 在场景加载完毕后执行操作
        OnlinegameChessBoardMgr.Instance.GetCurrentBoard();
        //消除所有面板 包括自己
        UIMgr.Instance.HideAll();
    }
}
