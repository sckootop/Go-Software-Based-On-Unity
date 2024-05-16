using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultMsg : BaseMsg
{
    public GameResult result = new GameResult();

    public override int GetBytesNum()
    {
        return 8 + result.GetBytesNum();
    }

    public override int GetID()
    {
        return 1019;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        result = ReadData<GameResult>(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        WriteData(bytes, result, ref index);
        return bytes;
    }
}
