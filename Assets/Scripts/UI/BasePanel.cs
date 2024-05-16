using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float alphaSpeed = 10;
    private bool isShow = true;
    private UnityAction hideCallBack;
    protected UnityAction updateActions;

    private void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    /// <summary>
    /// 主要用于 初始化 按钮事件监听等等内容
    /// </summary>
    protected abstract void Init();

    public virtual void ShowMe()
    {
        isShow = true;
        canvasGroup.alpha = 0;
    }

    public virtual void HideMe(UnityAction callBack)
    {
        isShow = false;
        canvasGroup.alpha = 1;
        //记录 传入的 当淡出成功后会执行的函数
        hideCallBack = callBack;
    }

    // Update is called once per frame
    void Update()
    {
        updateActions?.Invoke();
        //淡入
        if (isShow && canvasGroup.alpha != 1)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
                canvasGroup.alpha = 1;
        }
        //淡出
        else if (!isShow && canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
                canvasGroup.alpha = 0;
            //应该让管理器 删除自己
            hideCallBack?.Invoke();
        }
    }
}
