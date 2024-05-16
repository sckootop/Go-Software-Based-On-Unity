using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RegisterMsg : BaseMsg
{
    public AccountInfo accountInfo = new AccountInfo();
    public string userName;
    public override int GetBytesNum()
    {
        return 8 + accountInfo.GetBytesNum() + Encoding.UTF8.GetByteCount(userName) + 4;
    }

    public override int GetID()
    {
        return 1010;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        accountInfo = ReadData<AccountInfo>(bytes, ref index);
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
        WriteData(bytes, accountInfo, ref index);
        WriteString(bytes, userName, ref index);
        return bytes;
    }
}
