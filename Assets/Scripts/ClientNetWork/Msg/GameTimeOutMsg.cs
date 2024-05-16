using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeOutMsg : BaseMsg
{
    public bool iswhitewin;
    public override int GetBytesNum()
    {
        return 8 + 1;
    }

    public override int GetID()
    {
        return 1020;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        ReadBool(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        WriteBool(bytes, iswhitewin, ref index);
        return bytes;
    }
}
