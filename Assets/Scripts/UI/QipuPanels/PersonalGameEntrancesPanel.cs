using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PersonalGameEntrancesPanel : BasePanel
{
    public ScrollRect sr;
    protected override void Init()
    {
        string contentPath = Application.persistentDataPath + "/RecentGames";
        if(!Directory.Exists(contentPath)) Directory.CreateDirectory(contentPath);
        if (QipuChessBoardMgr.Instance.qijuList.Count == 0)
        {
            //加载棋局数据
            string[] files = Directory.GetFiles(contentPath, "*.sgf");
            for (int i = 0; i < files.Length; ++i)
            {
                QipuChessBoardMgr.Instance.filepaths.Add(files[i]);//存储路径
                QipuChessBoardMgr.Instance.qijuList.Add(ParseSgfHelper.Sgf2Chessmanual(files[i]));//存储棋局

                GameObject item = Instantiate(Resources.Load<GameObject>("UI/PersonalGameEntrance"));
                PersonalGameEntrance entrance = item.GetComponent<PersonalGameEntrance>();
                entrance.InitEntrance(i);
                item.transform.SetParent(sr.content.transform, false);
            }
        }
        else
        {
            for (int i = 0; i < QipuChessBoardMgr.Instance.qijuList.Count; ++i)
            {
                GameObject item = Instantiate(Resources.Load<GameObject>("UI/PersonalGameEntrance"));
                PersonalGameEntrance entrance = item.GetComponent<PersonalGameEntrance>();
                entrance.InitEntrance(i);
                item.transform.SetParent(sr.content.transform, false);
            }
        }
    }
}
