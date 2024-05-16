using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMsg : BaseMsg
{
    public int x;//落子的横坐标
    public int y;//落子的纵坐标

    public override int GetBytesNum()
    {
        return 4 + 4 + 2 * 4;
    }

    public override int GetID()
    {
        return 1004;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        x = ReadInt(bytes, ref index);
        y = ReadInt(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int bytesNum = GetBytesNum();
        byte[] bytes = new byte[GetBytesNum()];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, bytesNum - 8, ref index);
        WriteInt(bytes, x, ref index);
        WriteInt(bytes, y, ref index);
        return bytes;
    }
}
