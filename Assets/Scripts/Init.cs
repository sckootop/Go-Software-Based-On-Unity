using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EntryPoint
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        // 在场景加载之前执行的代码
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        // 在场景加载之后执行的代码
    }

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        //// 在程序启动时执行的代码
        Application.targetFrameRate = 20;
        MusicMgr.Instance.Init();
        SettingMgr.Instance.Init();
        MonoMgr.Instance.AddDestoryListener(() =>
        {
            //发送退出消息
            QuitMsg msg = new QuitMsg();
            ClientAsyncNet.Instance.Send(msg);

            //关闭katago进程
            KatagoHelper.katago.Close();

            Debug.Log("游戏结束");
        });
        UIMgr.Instance.ShowPanel<LoginPanel>();
        KatagoHelper.katago = new Katago();
        ClientAsyncNet.Instance.Connect("127.0.0.1", 9100);
    }
}
