using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public Button btnCancel;
    public Button btnSure;
    public InputField txtAccount;
    public InputField txtPassword;
    public InputField txtName;
    public Text txtAccountTip;
    public Text txtPasswordTip;
    public Text txtNameTip;

    private string tempstr="";//注意不初始化是为null的，=""意味着str将引用一个实际存在的空字符串对象
    private bool bAcc,bPas,bName;
    private bool canClickSure = true;
    private bool saveLoginData = false;
    private bool showPanels = false;
    protected override void Init()
    {
        txtAccountTip.text = "";
        txtPasswordTip.text = "";
        txtNameTip.text = "";

        btnCancel.onClick.AddListener(() =>
        {
            UIMgr.Instance.ShowPanel<LoginPanel>();
            UIMgr.Instance.HidePanel<RegisterPanel>();
        });

        btnSure.onClick.AddListener(() =>
        {
            if (canClickSure == false) return;
            if( bAcc && bPas && bName)
            {
                //发送注册消息
                RegisterMsg msg = new RegisterMsg();
                msg.accountInfo = new AccountInfo(txtAccount.text, txtPassword.text);
                msg.userName = txtName.text;
                ClientAsyncNet.Instance.Send(msg);
                print("正在注册中");
            }
            canClickSure = false;
        });

        txtAccount.onEndEdit.AddListener((str) =>
        {
            if (str.Length > 20)
            {
                txtAccountTip.text = "账号超出20长度限制";
                bAcc = false;
            }
            else if(str.Length<5)
            {
                txtAccountTip.text = "账号长度过短";
                bAcc = false;
            }
            else
            {
                txtAccountTip.text = "";
                bAcc = true;
            }
        });

        txtPassword.onEndEdit.AddListener((str) =>
        {
            if (str.Length > 20)
            {
                txtPasswordTip.text = "密码超出20长度限制";
                bPas = false;
            }
            else if(str.Length <5)
            {
                txtPasswordTip.text = "密码长度过短";
                bPas = false;
            }
            else
            {
                txtPasswordTip.text = "";
                bPas = true;
            }   
        });

        txtName.onEndEdit.AddListener((str) =>
        {
            if (str.Length > 10)
            {
                bName = false;
                txtNameTip.text = "用户名超出10长度限制";
            }
            else if(str.Length != 0)
            {
                bName = true;
                txtNameTip.text = "";
            } 
        });

        updateActions += () =>
        {
            if (tempstr != "")
            {
                txtNameTip.text = "用户名已存在";
                tempstr = "";
            }
            if(saveLoginData==true)
            {
                //本地记录信息，方便登录
                LoginData data = new LoginData();
                data.account = txtAccount.text;
                data.password = txtPassword.text;
                data.rememberPw = false;
                GameDataMgr.Instance.SaveLoginData(data);
                saveLoginData = false;
            }
            if(showPanels)
            {
                //面板显隐
                UIMgr.Instance.ShowPanel<LoginPanel>();
                UIMgr.Instance.GetPanel<LoginPanel>().ChangeInfo(txtAccount.text, txtPassword.text);
                UIMgr.Instance.HidePanel<RegisterPanel>();
                showPanels = false;
            }
        };
    }

    //网络模块来调用
    public void TryRegister(bool can)
    {
        //获得为true则注册成功，进入登录面板
        if (can==true)
        {
            saveLoginData = true;
            showPanels = true;
        }
        //获得为false 用户名重名则修改提示
        else
        {
            tempstr = "a";
        }
        canClickSure = true;
    }
}
