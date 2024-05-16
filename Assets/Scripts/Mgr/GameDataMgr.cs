using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class GameDataMgr : BaseManager<GameDataMgr>
{
    //登录相关
    private LoginData loginData;
    private UserInfo localUserInfo;
    private UserInfo opponentUserInfo;

    //网络对战相关
    public bool isWhite;//网络对战中，本地机器玩家执什么棋
    public bool turn = false;//回合 true 白棋下 false 黑棋下，  分先 初始化为true白棋先行，通过网络可以设置 这里默认设置为黑先行
    private int[] chessPos = new int[2] { -1, -1 };

    public int eatenCount1;//白方吃子数
    public int eatenCount2;//黑方吃子数

    public GameResult gameResult = null;

    //获取最近一个用户的登录信息
    public LoginData GetLoginData()
    {
        if (loginData != null) return loginData;
        loginData = JsonMgr.Instance.LoadData<LoginData>("LoginData");
        return loginData;
    }

    //获取某个用户的登录信息
    public LoginData GetLoginData(string fileName)
    {
        LoginData data = JsonMgr.Instance.LoadData<LoginData>(fileName);
        return data;
    }

    public void SaveLoginData()
    {
        if (loginData == null) return;
        JsonMgr.Instance.SaveData(loginData, "LoginData");
    }

    public void SaveLoginData(LoginData data)
    {
        loginData = data;
        JsonMgr.Instance.SaveData(loginData, "LoginData");
    }

    public void ClearLoginData()
    {
        loginData.rememberPw = false;
    }

    public void NotifyServerUpdateUserInfo()
    {
        UserInfoMsg msg=new UserInfoMsg();
        msg.userInfo = localUserInfo;
        ClientAsyncNet.Instance.Send(msg);
    }

    public void SaveLocalUserInfo(UserInfo data)
    {
        localUserInfo = data;
    }

    public UserInfo GetLocalUserInfo()
    {
        return localUserInfo;
    }

    public void SaveOpponentUserInfo(UserInfo data)
    {
        opponentUserInfo = data;
    }

    public UserInfo GetOpponentUserInfo()
    {
        return opponentUserInfo;
    }

    public void SetChessPos(int x, int y)
    {
        chessPos[0] = x;
        chessPos[1] = y;
    }

    public int[] GetChessPos()
    {
        int[] t = new int[2] { chessPos[0], chessPos[1] };
        chessPos[0] = -1;
        chessPos[1] = -1;
        return t;
    }

    public void Reset()
    {
        eatenCount1 = 0;
        eatenCount2 = 0;

        gameResult = null;
        opponentUserInfo = null;
        chessPos = new int[] { -1, -1 };
    }
}
