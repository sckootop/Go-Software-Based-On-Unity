using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognizeImageMgr : BaseManager<RecognizeImageMgr>
{
    public ChessManual cm { get; set; }
    public int curboardsize;
    public RecognizechessBoard GetCurrentBoard()
    {
        RecognizechessBoard script = GameObject.Find("ChessBoard").GetComponent<RecognizechessBoard>();
        return script;
    }
}
