using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaipuInfoPanel : MonoBehaviour
{
    public ScrollRect sv;
    List<BaipuItem> list = new List<BaipuItem>();//这里的数组少第0步，也就是初始局面

    //每下一步棋，就要添加BaipuItem，在BaipuchessBoard中调用
    public void AddItem(GameObject item)
    {
        item.transform.SetParent(sv.content.transform, false);
        list.Add(item.GetComponent<BaipuItem>());
        //每一次更新都移到最下面
        sv.verticalNormalizedPosition = 0f;
    }

    //提供外部删除的接口，删除第start步后的所有
    public void RemoveAfter(int start)
    {
        for(int i= list.Count-1; i>=start; i--)
        {
            GameObject.Destroy(list[i].gameObject);
        }
        list.RemoveRange(start, list.Count - start);
    }

    public BaipuItem GetItem(int i)
    {
        return list[i];
    }

    private void OnDestroy()
    {
        BaiPuMgr.Instance.curboardsize = 0;
    }
}
