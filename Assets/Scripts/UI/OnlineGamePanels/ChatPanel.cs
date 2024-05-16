using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : MonoBehaviour
{
    public Text talks;
    public Button btnEnter;
    public InputField inputField;
    private UserInfo myinfo;

    private string tempstr = "";
    private string tempname;
    
    void Start()
    {
        inputField.onEndEdit.AddListener(UpdateDialog);
        btnEnter.onClick.AddListener(() =>
        {
            UpdateDialog(inputField.text);
        });
        UIMgr.Instance.chatPanel = GameObject.Find("ChatPanel").GetComponent<ChatPanel>();
        myinfo = GameDataMgr.Instance.GetLocalUserInfo();
    }

    private void UpdateDialog(string str)
    {
        if (str == "") return;
        talks.text += "\n" + "[" + myinfo.userName + "]:" + str;
        inputField.text = "";
        TalkMsg msg = new TalkMsg();
        msg.sentence = str;//传消息只需传str
        msg.username = myinfo.userName;
        ClientAsyncNet.Instance.Send(msg);
    }

    /// <summary>
    /// 提供给外部更新对话框的方法
    /// </summary>
    /// <param name="str"></param>
    public void UpdateText(string str,string name)
    {
        tempstr = str;
        tempname = name;
    }

    void Update()
    {
        if (tempstr != "")
        {
            talks.text += "\n" + "[" + tempname + "]:" + tempstr;
            tempstr = "";
            tempname = "";
        }
    }

    void OnDestroy()
    {
        UIMgr.Instance.chatPanel = null;
    }
}
