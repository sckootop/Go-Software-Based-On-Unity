using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum JsonType
{
    JsonUtility,
    LitJson
}


//本质是根据不同的文件格式读取字符信息，通过反射实现序列化和反序列化
/// <summary>
/// Json数据管理类 主要用于进行 Json的序列化存储到硬盘 和 反序列化从硬盘中读取到内存中
/// </summary>
public class JsonMgr
{
    private static JsonMgr instance = new JsonMgr();
    public static JsonMgr Instance => instance;

    private JsonMgr() { }

    public void SaveData(object data,string fileName,JsonType type=JsonType.LitJson)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        string jsonStr = "";
        switch(type)
        {
            case JsonType.JsonUtility:
                jsonStr = JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr = JsonMapper.ToJson(data);
                break;
        }
        File.WriteAllText(path, jsonStr);
    }

    public T LoadData<T>(string fileName,JsonType type=JsonType.LitJson) where T : new()
    {
        string path = Application.streamingAssetsPath + "/" + fileName + ".json";
        //默认文件夹
        if(!File.Exists(path)) { path = Application.persistentDataPath + "/" + fileName + ".json"; }
        //读写文件夹
        if (!File.Exists(path)) return new T();
        //默认对象

        string jsonStr = File.ReadAllText(path);
        T data = default(T);
        switch(type)
        {
            case JsonType.JsonUtility:
                data=JsonUtility.FromJson<T>(jsonStr); break;
            case JsonType.LitJson:
                data=JsonMapper.ToObject<T>(jsonStr); break;
        }
        return data;
    }
}
