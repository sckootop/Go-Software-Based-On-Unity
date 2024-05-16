using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultMsg : BaseMsg
{
    public bool result;
    public bool isWhite;
    public override int GetBytesNum()
    {
        return 10;
    }

    public override int GetID()
    {
        return 1007;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        result = ReadBool(bytes, ref index);
        isWhite = ReadBool(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        WriteBool(bytes, result, ref index);
        WriteBool(bytes, isWhite, ref index);
        return bytes;
    }
}
