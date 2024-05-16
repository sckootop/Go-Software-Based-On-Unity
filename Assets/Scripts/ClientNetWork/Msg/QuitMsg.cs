using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMsg : BaseMsg
{
    public override int GetBytesNum()
    {
        return 8;//ID+表示消息体长的int
    }

    public override int GetID()
    {
        return 1003;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        return 0;
    }

    public override byte[] Writing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index);//ID
        WriteInt(bytes, 0, ref index);//
        return bytes;
    }
}
