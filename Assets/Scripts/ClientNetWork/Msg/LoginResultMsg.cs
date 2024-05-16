using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginResultMsg : BaseMsg
{
    public bool result;
    public override int GetBytesNum()
    {
        return 9;
    }

    public override int GetID()
    {
        return 1013;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        result = ReadBool(bytes, ref index);
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
        return bytes;
    }
}
