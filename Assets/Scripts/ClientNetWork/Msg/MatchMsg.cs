using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMsg : BaseMsg
{
    public short size;
    public override int GetBytesNum()
    {
        return 8 + 2;
    }

    public override int GetID()
    {
        return 1006;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        size = ReadShort(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        WriteShort(bytes, size, ref index);
        return bytes;
    }
}
