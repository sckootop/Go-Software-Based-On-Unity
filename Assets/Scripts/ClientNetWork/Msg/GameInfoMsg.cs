using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameInfoMsg : BaseMsg
{
    public string sgfContent;

    public override int GetBytesNum()
    {
        return 8 + Encoding.UTF8.GetByteCount(sgfContent) + 4;
    }

    public override int GetID()
    {
        return 1009;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        sgfContent = ReadString(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int bytesNum = GetBytesNum();
        byte[] bytes = new byte[bytesNum];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, bytesNum - 8, ref index);
        WriteString(bytes, sgfContent, ref index);
        return bytes;
    }
}
