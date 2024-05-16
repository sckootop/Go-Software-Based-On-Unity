using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RecommendMovesPanel : MonoBehaviour
{
    public ScrollRect sv;
    List<RecommendMovesItem> items=new List<RecommendMovesItem>();

    public void Clear()
    {
        for(int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
    }

    public void AddItem(RecommendMovesItem item)
    {
        item.gameObject.transform.SetParent(sv.content, false);
        items.Add(item);
    }
}
