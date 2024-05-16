using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Types;

public class MusicMgr : BaseManager<MusicMgr>
{
    //背景音乐组件，因为必须要有才能调用API
    private AudioSource bkMusic = null;
    //背景音乐字典缓存
    Dictionary<string, AudioClip> bkMusicDic = new Dictionary<string, AudioClip>();
    //音乐大小
    public float bkValue = 0.5f;

    //音效依附对象
    private GameObject soundObj = null;
    //音效列表
    private List<AudioSource> soundList = new List<AudioSource>();
    //音效大小
    public float soundValue = 0.5f;

    //语音组件，播报语音
    private GameObject voiceObj;
    private List<AudioSource> voiceList = new List<AudioSource>();
    public float voiceValue = 0.5f;

    private class MusicData
    {
        public float bkValue;
        public float soundValue;
        public float voiceValue;
        public MusicData() { bkValue = 0.5f; soundValue = 0.5f; voiceValue = 0.5f; }
        public MusicData(float bkValue, float soundValue, float voiceValue)
        {
            this.bkValue = bkValue;
            this.soundValue = soundValue;
            this.voiceValue = voiceValue;
        }
    }

    ~ MusicMgr()
    {
        
    }

    private void SaveData()
    {
        MusicData data = new MusicData(bkValue, soundValue, voiceValue);
        JsonMgr.Instance.SaveData(data, "MusicData", JsonType.JsonUtility);
    }

    public void Init()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
        MonoMgr.Instance.AddDestoryListener(SaveData);//先设置为游戏结束自动保存
        MusicData data = JsonMgr.Instance.LoadData<MusicData>("MusicData", JsonType.JsonUtility);//读取之前的用户设置配置
        this.bkValue = data.bkValue;
        this.soundValue = data.soundValue;
        this.voiceValue = data.voiceValue;
    }

    private void Update()
    {
        //要从后面开始删
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }

        for (int i = voiceList.Count - 1; i >= 0; --i)
        {
            if (!voiceList[i].isPlaying)
            {
                GameObject.Destroy(voiceList[i]);
                voiceList.RemoveAt(i);
            }
        }
    }

    public void PlayBkMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BkMusic";
            bkMusic = obj.AddComponent<AudioSource>();
        }

        if(bkMusicDic.ContainsKey(name))
        {
            bkMusic.clip = bkMusicDic[name];
            bkMusic.Play();
        }
        else
        {
            //异步加载背景音乐 加载完成后 播放
            ResMgr.Instance.LoadAsync<AudioClip>("Music/" + name, (clip) =>
            {
                bkMusic.clip = clip;
                bkMusicDic.Add(name, clip);//加入字典缓存
                bkMusic.loop = true;
                bkMusic.volume = bkValue;
                bkMusic.Play();
            });
        }
    }

    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    public void SetBKValue(float v)
    {
        bkValue = Mathf.Clamp01(v);
        if (bkMusic == null)
            return;
        bkMusic.volume = bkValue;
    }

    public AudioSource PlaySound(string name, bool isLoop)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";
        }

        AudioSource source = null;

        //当音效资源异步加载结束后 再添加一个音效
        ResMgr.Instance.LoadAsync<AudioClip>("Sound/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();
            soundList.Add(source);
        });

        return source;
    }

    public void SetSoundValue(float value)
    {
        soundValue = Mathf.Clamp01(value);
        for (int i = 0; i < soundList.Count; ++i)
            soundList[i].volume = value;
    }

    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }

    public void PlayVoice(string name,bool isloop)
    {
        if(voiceObj == null)
        {
            GameObject obj = new GameObject();
            obj.name = "Voice";
        }

        ResMgr.Instance.LoadAsync<AudioClip>("Voice/" + name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isloop;
            source.volume = voiceValue;
            source.Play();
            voiceList.Add(source);
        });
    }

    public void SetVocieValue(float value)
    {
        voiceValue = Mathf.Clamp01(value);
        for (int i = 0; i < voiceList.Count; ++i)
            voiceList[i].volume = value;
    }

    public void StopVocie(AudioSource source)
    {
        if (voiceList.Contains(source))
        {
            voiceList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}
