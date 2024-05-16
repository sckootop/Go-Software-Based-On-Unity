using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UserInfoMsg : BaseMsg
{
    public UserInfo userInfo;
    public int userType;//0为本地用户数据 1为对手用户数据
    public override int GetBytesNum()
    {
        return 8 + userInfo.GetBytesNum() + 4;
    }

    public override int GetID()
    {
        return 1015;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        userInfo = ReadData<UserInfo>(bytes, ref index);
        userType = ReadInt(bytes, ref index);
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
        WriteInt(bytes, userType, ref index);
        return bytes;
    }
}
