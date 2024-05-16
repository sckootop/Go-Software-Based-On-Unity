using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : BasePanel
{
    public Text txtName;
    public Text txtVictoryNums;
    public Text txtDefeatNums;
    public Text txtRankScore;
    public Text txtQuestionNums;
    public Text txtWinRatePer;
    public Image Circle;

    protected override void Init()
    {
        UserInfo info = GameDataMgr.Instance.GetLocalUserInfo();
        txtName.text = info.userName;
        txtVictoryNums.text = info.victoryNums.ToString();
        txtDefeatNums.text = info.defeatNums.ToString();
        txtRankScore.text = info.rankScore.ToString();
        txtQuestionNums.text = info.questionNums.ToString();
        if ((info.defeatNums | info.victoryNums) == 0)
        {
            txtWinRatePer.text = "нч";
            Circle.fillAmount = 0;
            return;
        }
        double value = Math.Round(((double)info.victoryNums) / (info.defeatNums + info.victoryNums), 3);
        txtWinRatePer.text = (value * 100).ToString() + "%";
        Circle.fillAmount = (float)value;
    }
}
