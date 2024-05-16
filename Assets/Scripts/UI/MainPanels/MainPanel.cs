using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    public Button btnPlay;
    public Button btnLibrary;
    public Button btnInfo;
    public Button btnSetting;
    public Button btnQuit;

    protected override void Init()
    {
        btnPlay.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel(false, "UserInfoPanel", "SettingPanel", "GoPanel","TikuEntrancesPanel","RecognizeImagePanel", "PersonalGameEntrancesPanel");
            UIMgr.Instance.ShowPanel<PlayPanel>();
        });

        btnLibrary.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel(false, "UserInfoPanel", "SettingPanel", "PlayPanel");
            GoPanel panel = UIMgr.Instance.ShowPanel<GoPanel>();
            if(UIMgr.Instance.GetPanel<TikuEntrancesPanel>()==null && UIMgr.Instance.GetPanel<RecognizeImagePanel>() == null && UIMgr.Instance.GetPanel<PersonalGameEntrancesPanel>() == null)
                panel.ShowPanel(PanelName.TikuEntrancesPanel);
        });

        btnInfo.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel(false, "PlayPanel", "SettingPanel", "GoPanel","TikuEntrancesPanel", "RecognizeImagePanel", "PersonalGameEntrancesPanel");
            UIMgr.Instance.ShowPanel<UserInfoPanel>();
        });

        btnSetting.onClick.AddListener(() =>
        {
            UIMgr.Instance.HidePanel(false, "UserInfoPanel", "PlayPanel", "GoPanel", "TikuEntrancesPanel", "RecognizeImagePanel", "PersonalGameEntrancesPanel");
            UIMgr.Instance.ShowPanel<SettingPanel>();
        });

        btnQuit.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        //btnPlay.onClick.Invoke();


        //进入主面板，检查是否有题库文件，没有就请求服务器预加载，使用多线程进行IO保存

    }
}
