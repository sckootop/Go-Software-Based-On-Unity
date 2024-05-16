using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XuanDianPanel : BasePanel
{
    public TextMeshProUGUI txtXuandian;
    public TextMeshProUGUI txtWinrate;

    private Dictionary<int, string> dic = new Dictionary<int, string>()
    {
        {1,"一"},{2,"二"},{3,"三"},{4,"四"},{5,"五"},{6,"六"},{7,"七"},{8,"八"},{9,"九"},{10,"十"},{11,"十一"},{12,"十二"}
    };
    protected override void Init()
    {
        
    }

    public void InitText(bool iswhite, int order, double rate)
    {
        txtXuandian.text = (iswhite ? "白" : "黑") + "方第" + dic[order] + "选点";
        txtWinrate.text = "胜率：" + rate + "%";
    }
}
