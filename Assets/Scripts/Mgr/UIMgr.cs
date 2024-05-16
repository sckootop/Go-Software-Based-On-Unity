using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIMgr
{
    private static UIMgr instance = new UIMgr();
    public static UIMgr Instance => instance;

    //存储面板的容器
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    private Transform canvasTrans;
    public ChatPanel chatPanel;
    public OnlinePlayerPanel onlinePlayerPanel;

    private UIMgr()
    {
        //得到场景上创建好的 Canvas对象
        canvasTrans = GameObject.Find("Canvas").transform;
        GameObject.DontDestroyOnLoad(canvasTrans.gameObject);
    }

    public T ShowPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;

        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;

        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));//面板都是重新创建的
        panelObj.transform.SetParent(canvasTrans, false);

        T panel = panelObj.GetComponent<T>();
        panelDic.Add(panelName, panel);
        panel.ShowMe();

        return panel;
    }

    public BasePanel ShowPanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as BasePanel;

        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/" + panelName));//面板都是重新创建的
        panelObj.transform.SetParent(canvasTrans, false);

        BasePanel panel = panelObj.GetComponent<BasePanel>();
        panelDic.Add(panelName, panel);
        panel.ShowMe();
        return panel;
    }

    public void HidePanel<T>(bool isFade = true) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            if (isFade)
            {
                panelDic[panelName].HideMe(() =>
                {
                    GameObject.Destroy(panelDic[panelName].gameObject);
                    panelDic.Remove(panelName);
                });
            }
            else
            {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
        }
    }

    public void HidePanel(bool isFade = true, params string[] names)
    {
        string panelName;
        foreach(string name in names)
        {
            panelName = name;
            if (panelDic.ContainsKey(panelName.ToString()))
            {
                if (isFade)
                {
                    panelDic[panelName].HideMe(() =>
                    {
                        GameObject.Destroy(panelDic[panelName].gameObject);
                        panelDic.Remove(panelName);
                    });
                }
                else
                {
                    GameObject.Destroy(panelDic[panelName].gameObject);
                    panelDic.Remove(panelName);
                }
            }
        }
    }

    public void HideAll(bool isFade = false)
    {
        foreach(KeyValuePair<string,BasePanel> pair in panelDic)
        {
            if(!isFade)
            {
                GameObject.Destroy(panelDic[pair.Key].gameObject);
            }
            else
            {
                pair.Value.HideMe(() =>
                {
                    GameObject.Destroy(panelDic[pair.Key].gameObject);
                });
            }
        }
        panelDic.Clear();
    }

    public T GetPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        return null;
    }
}
