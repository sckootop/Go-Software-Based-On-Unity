using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopStepMsg : BaseMsg
{
    public override int GetBytesNum()
    {
        return 8;
    }

    public override int GetID()
    {
        return 1018;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        return 0;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        return bytes;
    }
}
