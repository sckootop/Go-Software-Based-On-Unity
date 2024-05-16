using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaiPuMgr : BaseManager<BaiPuMgr>
{
    public ChessManual cm { get; set; }
    public int curboardsize;
    public BaipuchessBoard GetCurrentBoard()
    {
        BaipuchessBoard script = GameObject.Find("ChessBoard").GetComponent<BaipuchessBoard>();
        return script;
    }
}
