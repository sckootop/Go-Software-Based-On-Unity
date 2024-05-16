using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//进入棋局的入口
public class PersonalGameEntrance : MonoBehaviour
{
    public Button btn;
    public Text topName;
    public Text bottomName;
    private int index;//题目下标

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(() =>
        {
            QipuChessBoardMgr.Instance.index = index;
            QipuChessBoardMgr.Instance.boardsize = QipuChessBoardMgr.Instance.qijuList[index].getSize();
            UIMgr.Instance.HideAll();
            //进入场景
            SceneManager.LoadScene("QipuScene");
        });
    }

    public void InitEntrance(int i)
    {
        index = i;
        //文字更新还没搞 可能有问题，因为在循环内
        topName.text = QipuChessBoardMgr.Instance.qijuList[i].getBlackName();
        bottomName.text = QipuChessBoardMgr.Instance.qijuList[i].getWhiteName();
        
    }
}
