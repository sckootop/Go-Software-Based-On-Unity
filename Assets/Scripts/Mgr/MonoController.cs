using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    public event UnityAction updateEvent;
    public event UnityAction destoryEvent;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
            updateEvent.Invoke();
    }

    //给外部提供 添加帧更新事件的函数
    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    //提供给外部 用于移除帧更新事件函数
    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }

    private void OnDestroy()
    {
        destoryEvent?.Invoke();
    }

    public void AddDestoryListener(UnityAction fun)
    {
        destoryEvent += fun;
    }

    public void RemoveDestoryListener(UnityAction fun)
    {
        destoryEvent -= fun;
    }
}
