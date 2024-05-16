using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RequestUserInfoMsg : BaseMsg
{
    public string userName;
    public override int GetBytesNum()
    {
        return 8 + Encoding.UTF8.GetByteCount(userName) + 4;
    }

    public override int GetID()
    {
        return 1014;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        userName = ReadString(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        WriteString(bytes, userName, ref index);
        return bytes;
    }
}

