using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMsg : BaseMsg
{
    public AccountInfo accountInfo;

    public override int GetBytesNum()
    {
        return 8 + accountInfo.GetBytesNum();
    }

    public override int GetID()
    {
        return 1012;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        accountInfo = ReadData<AccountInfo>(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        WriteData(bytes, accountInfo, ref index);
        return bytes;
    }
}
