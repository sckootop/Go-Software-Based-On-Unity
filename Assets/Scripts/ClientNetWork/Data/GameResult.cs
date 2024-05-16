using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameResult : BaseData
{
    public char winner;//W or B
    public string reason;

    //s:= W+0.5
    //W+R
    //W+T
    public void String2Result(string s)
    {
        string str = s.Split(' ')[1];
        winner = str[0];//W B
        reason = str.Substring(2, str.Length - 2);//0.5 R T
    }

    public override int GetBytesNum()
    {
        return sizeof(char) + Encoding.UTF8.GetByteCount(reason) + 4;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        winner = ReadChar(bytes, ref index);
        reason = ReadString(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int bytesNum = GetBytesNum();
        byte[] bytes = new byte[bytesNum];
        WriteChar(bytes, winner, ref index);
        WriteString(bytes, reason, ref index);
        return bytes;
    }
}

