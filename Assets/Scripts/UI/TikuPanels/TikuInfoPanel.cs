using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TikuInfoPanel : MonoBehaviour
{
    public ScrollRect sr;
    public RectTransform tip;
    public Text sentence;//解题的提示文字

    public Button btnQuit;//回到选题界面
    public Button btnTry;//试下
    public Button btnRestart;//重新开始
    public Button btnNext;//下一题

    private bool istry;
    private int timuIndex = 0;
    private List<QuestionItem> questions = new List<QuestionItem>();

    // Start is called before the first frame update
    void Start()
    {
        //还要读取已做完题目的数据

        //在面板上显示题目
        List<GameTree> timu = TikuMgr.Instance.tikuDic[TikuMgr.Instance.curTiku];
        for (int i = 0; i < timu.Count; ++i)
        {
            GameObject item = Instantiate(Resources.Load<GameObject>("Question"));
            item.transform.SetParent(sr.content.transform, false);
            QuestionItem question = item.GetComponent<QuestionItem>();
            questions.Add(question);
            question.InitInfo("对杀" + (i + 1), "入门", timu[i], i);
        }

        //显示第一道题的面板
        TikuMgr.Instance.ShowChessBoard(timu[0].size);
        //该棋盘setquestion
        TikuMgr.Instance.GetCurrentBoard().SetQuestion(timu[0]);

        btnQuit.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("UserScene");
            UIMgr.Instance.ShowPanel<MainPanel>();
            UIMgr.Instance.ShowPanel<GoPanel>();
            UIMgr.Instance.ShowPanel<TikuEntrancesPanel>();
        });

        btnTry.onClick.AddListener(() =>
        {
            //试下的时候为红色 解题为白色
            istry = !istry;
            if(istry==false)
            {
                btnTry.image.color = Color.white; 
            }
            else
            {
                btnTry.image.color = Color.yellow;
            }
            TikuMgr.Instance.GetCurrentBoard().ChangeTryCase(istry);
            HideTip();
        });

        btnRestart.onClick.AddListener(() =>
        {
            HideTip();
            TikuMgr.Instance.GetCurrentBoard().ResetBoard();
        });

        btnNext.onClick.AddListener(() =>
        {
            if (timuIndex + 1 < timu.Count) timuIndex++;
            else return;

            HideTip();
            TikuMgr.Instance.ShowChessBoard(timu[timuIndex].size);
            //该棋盘setquestion
            TikuMgr.Instance.GetCurrentBoard().SetQuestion(timu[timuIndex]);
            TikuMgr.Instance.GetCurrentBoard().ResetBoard();
        });
    }

    public void ShowTip(bool success)
    {
        //显示文字
        if(success)
        {
            questions[timuIndex].SetComplete(true);//设置已完成
            sentence.text = "阁下一定是高手，一出手就不同凡响";
        }
        else
        {
            sentence.text = "没那么简单呢";
            StartCoroutine("DelayHide", 1.5f);
        }
        tip.offsetMax = new Vector2(tip.offsetMax.x, 0);
    }

    public void HideTip()
    {
        tip.offsetMax = new Vector2(tip.offsetMax.x, 600);
    }

    public void SetIndex(int i)
    {
        timuIndex = i;
    }

    public void ResetTry()
    {
        istry = false;
        btnTry.image.color = Color.white;
    }

    IEnumerator DelayHide(float time)
    {
        yield return new WaitForSeconds(time);
        HideTip();
    }

    private void OnDestroy()
    {
        //还要保存已做题目的数据

        TikuMgr.Instance.curboardSize = 0;
    }
}
