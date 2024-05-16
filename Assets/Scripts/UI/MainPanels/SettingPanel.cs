using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public LayoutElement contentLayout;//Scroll View里的Content的
    public RectTransform lastElement;//最下面的UI组件

    //音效
    public Text txtMusicvalue;
    public Slider sliderMusic;
    public Text txtSoundvalue;
    public Slider sliderSound;
    public Text txtVoicevalue;
    public Slider sliderVoice;

    //提示音 暂时不做字体淡入淡出动画
    public Text txtCountdown;
    public Button btnLeftCountdown;
    public Button btnRightCountdown;
    string[] CountdownSelections = new string[] { "开启", "关闭" };

    public Text txtDumiao;
    public Button btnLeftDumiao;
    public Button btnRightDumiao;
    string[] DumiaoSelections = new string[] { "开启", "关闭" };

    public Text txtXushou;
    public Button btnLeftXushou;
    public Button btnRightXushou;
    string[] XushouSelections = new string[] { "开启", "关闭" };

    protected override void Init()
    {
        contentLayout.preferredHeight = Mathf.Abs(lastElement.localPosition.y) + lastElement.sizeDelta.y + 2;//这个改了没效果?
        RectTransform rect = contentLayout.gameObject.transform as RectTransform;
        
        //content height大小自适应
        Vector2 v2 = rect.sizeDelta;
        v2.y = contentLayout.preferredHeight;
        rect.sizeDelta = v2;

        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);//当前帧强制更新

        //根据本地数据更新面板
        float tvalue = MusicMgr.Instance.bkValue;
        sliderMusic.value = tvalue;
        tvalue = (float)(System.Math.Round(MusicMgr.Instance.bkValue, 2) * 100);
        txtMusicvalue.text = tvalue.ToString();

        tvalue = MusicMgr.Instance.soundValue;
        sliderSound.value = tvalue;
        tvalue = (float)(System.Math.Round(MusicMgr.Instance.soundValue, 2) * 100);
        txtSoundvalue.text = tvalue.ToString();
        

        tvalue = MusicMgr.Instance.voiceValue;
        sliderVoice.value = tvalue;
        tvalue = (float)(System.Math.Round(MusicMgr.Instance.voiceValue, 2) * 100);
        txtVoicevalue.text = tvalue.ToString();
        

        txtCountdown.text = CountdownSelections[SettingMgr.Instance.isCountdownOn ? 0 : 1];
        txtDumiao.text = DumiaoSelections[SettingMgr.Instance.isDumiaoOn ? 0 : 1];
        txtXushou.text = XushouSelections[SettingMgr.Instance.isXushouOn ? 0 : 1];

        sliderMusic.onValueChanged.AddListener((v) =>
        {
            MusicMgr.Instance.SetBKValue(sliderMusic.value);
            txtMusicvalue.text = (System.Math.Round(v, 2) * 100).ToString();
        });

        sliderSound.onValueChanged.AddListener((v) =>
        {
            MusicMgr.Instance.SetSoundValue(sliderSound.value);
            txtSoundvalue.text = (System.Math.Round(v, 2) * 100).ToString();
        });

        sliderVoice.onValueChanged.AddListener((v) =>
        {
            MusicMgr.Instance.SetVocieValue(sliderVoice.value);
            txtVoicevalue.text = (System.Math.Round(v, 2) * 100).ToString();
        });

        btnLeftCountdown.onClick.AddListener(() =>
        {
            SettingMgr.Instance.isCountdownOn = !SettingMgr.Instance.isCountdownOn;
            txtCountdown.text = CountdownSelections[SettingMgr.Instance.isCountdownOn ? 0 : 1];
        });

        btnRightCountdown.onClick.AddListener(() =>
        {
            SettingMgr.Instance.isCountdownOn = !SettingMgr.Instance.isCountdownOn;
            txtCountdown.text = CountdownSelections[SettingMgr.Instance.isCountdownOn ? 0 : 1];
        });

        btnLeftDumiao.onClick.AddListener(() =>
        {
            SettingMgr.Instance.isDumiaoOn = !SettingMgr.Instance.isDumiaoOn;
            txtDumiao.text = DumiaoSelections[SettingMgr.Instance.isDumiaoOn ? 0 : 1];
        });

        btnRightDumiao.onClick.AddListener(() =>
        {
            SettingMgr.Instance.isDumiaoOn = !SettingMgr.Instance.isDumiaoOn;
            txtDumiao.text = DumiaoSelections[SettingMgr.Instance.isDumiaoOn ? 0 : 1];
        });

        btnLeftXushou.onClick.AddListener(() =>
        {
            SettingMgr.Instance.isXushouOn = !SettingMgr.Instance.isXushouOn;
            txtXushou.text = XushouSelections[SettingMgr.Instance.isXushouOn ? 0 : 1];
        });

        btnRightXushou.onClick.AddListener(() =>
        {
            SettingMgr.Instance.isXushouOn = !SettingMgr.Instance.isXushouOn;
            txtXushou.text = XushouSelections[SettingMgr.Instance.isXushouOn ? 0 : 1];
        });
    }
}