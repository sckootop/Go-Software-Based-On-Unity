using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public Button btnLogin;
    public Button btnRegister;
    public Toggle togRemPass;
    public InputField txtAccount;
    public InputField txtPassword;
    public Text txtTip;

    private bool canClickLogin = true;
    //切换用户功能，还要搞个下拉框，懒的搞
    private bool tempTip = false;
    private bool saveLoginData = false;
    private bool changeAccountInfo = false;
    private AccountInfo accountInfo=new AccountInfo();
    private bool showPanels = false;
    private LoginData loginData; 

    protected override void Init()
    {     
        txtTip.text = "";

        //读取本地存储的数据 方便玩家
        loginData = GameDataMgr.Instance.GetLoginData();
        togRemPass.isOn = loginData.rememberPw;
        txtAccount.text = loginData.account;
        if (loginData.rememberPw == true)
            txtPassword.text = loginData.password;

        btnLogin.onClick.AddListener(() =>
        {
            if (canClickLogin == false) return;
            canClickLogin = false;
            //判断账户密码长度是否正确
            if (txtAccount.text.Length <= 20 && txtPassword.text.Length <= 20 && txtAccount.text.Length >= 5 && txtPassword.text.Length >= 5)
            {
                //向服务器发登录消息 判断账号信息是否正确
                LoginMsg msg = new LoginMsg();
                msg.accountInfo = new AccountInfo(txtAccount.text, txtPassword.text);
                ClientAsyncNet.Instance.Send(msg);
            }
            else
            {
                txtTip.text = "账号或密码错误";
                canClickLogin = true;
            }
        });

        btnRegister.onClick.AddListener(() =>
        {
            //显示注册面板
            UIMgr.Instance.ShowPanel<RegisterPanel>();
            UIMgr.Instance.HidePanel<LoginPanel>();
        });

        txtAccount.onValueChanged.AddListener((str) =>
        {
            txtTip.text = "";
        });

        txtPassword.onValueChanged.AddListener((str) =>
        {
            txtTip.text = "";
        });

        togRemPass.onValueChanged.AddListener((ison) =>
        {
            loginData.rememberPw = ison;
            GameDataMgr.Instance.SaveLoginData();
        });

        updateActions += () =>
        {
            if (tempTip)
            {
                txtTip.text = "账号或密码错误";
                tempTip = false;
            }
            if (saveLoginData == true)
            {
                //本地记录信息，方便登录
                loginData.account = txtAccount.text;
                loginData.password = txtPassword.text;
                GameDataMgr.Instance.SaveLoginData();
                saveLoginData = false;
            }
            if(changeAccountInfo)
            {
                txtAccount.text = accountInfo.account;
                txtPassword.text=accountInfo.password;
                changeAccountInfo = false;
            }
            if(showPanels)
            {
                UIMgr.Instance.ShowPanel<MainPanel>();
                UIMgr.Instance.ShowPanel<PlayPanel>();
                UIMgr.Instance.HidePanel<LoginPanel>(false);
                showPanels = false;
            }
        };
    }

    //根据服务器返回消息的 bool 判断是否登陆成功
    public void LoginIn(bool can)
    {
        if (can)
        {
            showPanels = true;
            saveLoginData = true;
        }
        else
        {
            tempTip = true;
        }
        canClickLogin = true;
    }

    public void ChangeInfo(string _account,string _password)
    {
        changeAccountInfo = true;
        accountInfo.account = _account;
        accountInfo.password = _password;
    }
}
