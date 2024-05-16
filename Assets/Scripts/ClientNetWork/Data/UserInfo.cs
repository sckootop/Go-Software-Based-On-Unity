using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UserInfo : BaseData
{
    public string userName;
    public int victoryNums;
    public int defeatNums;
    public int rankScore;
    public int questionNums;

    public override int GetBytesNum()
    {
        return Encoding.UTF8.GetByteCount(userName) + 4 + 4 + 4 + 4 + 4;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        userName = ReadString(bytes, ref index);
        victoryNums = ReadInt(bytes, ref index);
        defeatNums = ReadInt(bytes, ref index);
        rankScore = ReadInt(bytes, ref index);
        questionNums = ReadInt(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int bytesNum = GetBytesNum();
        byte[] bytes = new byte[bytesNum];
        WriteString(bytes, userName, ref index);
        WriteInt(bytes, victoryNums, ref index);
        WriteInt(bytes, defeatNums, ref index);
        WriteInt(bytes, rankScore, ref index);
        WriteInt(bytes, questionNums, ref index);
        return bytes;
    }

    public string GetLevel()
    {
        if (rankScore < 1000) return "19k";
        else if(rankScore < 1900)//1000-1900为50分一段
        {
            int sub = (1900 - rankScore) / 50 + 1;
            return $"{19 - sub}k";
        }
        else if(rankScore < 2500)//1900-2500位100分一段
        {
            int add = (rankScore - 1900) / 100 + 1;
            return $"{add}d";
        }
        else if(rankScore >2500)//2500以上200分一段
        {
            int add = (rankScore - 2500) / 200 + 1;
            if (add > 9) add = 9;
            return $"{add}p";
        }
        return null;
    }
}
