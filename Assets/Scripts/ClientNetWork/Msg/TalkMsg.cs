using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

public class TalkMsg : BaseMsg
{
    public string sentence;
    public string username;
    public override int GetBytesNum()
    {
        return 8 + Encoding.UTF8.GetBytes(sentence).Length + 4 + Encoding.UTF8.GetBytes(username).Length + 4;
    }

    public override int GetID()
    {
        return 1005;
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        sentence = ReadString(bytes, ref index);
        username = ReadString(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int bytesNum = GetBytesNum();
        byte[] bytes = new byte[bytesNum];
        WriteInt(bytes, GetID(), ref index);
        WriteInt(bytes, bytesNum - 8, ref index);
        WriteString(bytes, sentence, ref index);
        WriteString(bytes, username, ref index);
        return bytes;
    }
}
