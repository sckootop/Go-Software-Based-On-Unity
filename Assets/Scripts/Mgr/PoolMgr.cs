using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMgr : MonoBehaviour
{
    //挂载到gameScene的Pool对象上，切换场景组件就会新创建一个PoolMgr单例

    private static PoolMgr instance;
    public static PoolMgr Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj = new GameObject("PoolMgr");
                instance = obj.AddComponent<PoolMgr>();
            }
            return instance;
        }
    }

    public Dictionary<string, List<GameObject>> poolDic = new Dictionary<string, List<GameObject>>();


    //name 资源路径
    public GameObject GetObj(string name)
    {
        //name为资源路径
        GameObject obj = null;

        if (poolDic.ContainsKey(name) && poolDic[name].Count != 0)
        {
            int num = poolDic[name].Count;
            obj = poolDic[name][num - 1];
            poolDic[name].RemoveAt(num - 1);
        }
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        }

        obj.SetActive(true);
        return obj;
    }

    public void PushObj(string name, GameObject obj)
    {
        obj.SetActive(false);//先失效

        //接下来都是poolDic的访问
        if (poolDic == null) return;

        if (poolDic.ContainsKey(name))
        {
            poolDic[name].Add(obj);
        }
        else
        {
            poolDic.Add(name, new List<GameObject>() { obj });
        }
    }

    public void Clear()
    {
        poolDic.Clear();//这个Clear的API只会让字典中的元素为null，但是实际上Count还是不变，字典依旧有元素，但是都为null
        poolDic = null;//所以置个null，好习惯
    }

    private void OnDestroy()
    {
        Clear();
    }
}
