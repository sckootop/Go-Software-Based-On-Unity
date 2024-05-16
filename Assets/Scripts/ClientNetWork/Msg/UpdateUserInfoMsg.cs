using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateUserInfoMsg : BaseMsg
{
    public UserInfo userInfo = new UserInfo();
    public override int GetBytesNum()
    {
        return 8 + userInfo.GetBytesNum();
    }

    public override int GetID()
    {
        return 1016;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        userInfo = ReadData<UserInfo>(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, byteNums - 8, ref index);
        WriteData(bytes, userInfo, ref index);
        return bytes;
    }
}
