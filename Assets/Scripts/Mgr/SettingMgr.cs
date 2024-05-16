using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMgr : BaseManager<SettingMgr>
{
    public bool isCountdownOn = true;//µπº∆ ±
    public bool isDumiaoOn = true;//∂¡√Î
    public bool isXushouOn = true;//–È ÷

    public void Init()
    {
        MonoMgr.Instance.AddDestoryListener(SaveData);
        SettingData data = JsonMgr.Instance.LoadData<SettingData>("SettingData", JsonType.JsonUtility);
        this.isCountdownOn = data.isCountdownOn;
        this.isDumiaoOn = data.isDumiaoOn;
        this.isXushouOn = data.isXushouOn;
    }

    public void SaveData()
    {
        SettingData data = new SettingData(isCountdownOn, isDumiaoOn, isXushouOn);
        JsonMgr.Instance.SaveData(data, "SettingData");  
    }
}

public class SettingData
{
    public bool isCountdownOn = true;
    public bool isDumiaoOn = true;
    public bool isXushouOn = true;
    public SettingData() { isCountdownOn = true; isDumiaoOn = true; isXushouOn = true; }
    public SettingData(bool _isCountdownOn, bool _isDumiaoOn, bool _isXushouOn)
    {
        this.isCountdownOn = _isCountdownOn;
        this.isDumiaoOn = _isDumiaoOn;
        this.isXushouOn = _isXushouOn;
    }
}