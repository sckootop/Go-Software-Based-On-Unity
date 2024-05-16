using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//服务器传来的数据
public class QuestionData : BaseData
{
    public string name;
    public short difficulty;//1入门，2初级，3中级，4高级
    public bool isComplete;
    public string content;//sgf文本

    public QuestionData(string name, short difficulty, bool isComplete, string content)
    {
        this.name = name;
        this.difficulty = difficulty;
        this.isComplete = isComplete;
        this.content = content;
    }

    public override int GetBytesNum()
    {
        return 4 + Encoding.UTF8.GetByteCount(name) + 2 + 1 + 4 + Encoding.UTF8.GetByteCount(content);
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        name = ReadString(bytes, ref index);
        difficulty = ReadShort(bytes, ref index);
        isComplete = ReadBool(bytes, ref index);
        content = ReadString(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteString(bytes, name, ref index);
        WriteShort(bytes, difficulty, ref index);
        WriteBool(bytes, isComplete, ref index);
        WriteString(bytes, content, ref index);
        return bytes;
    }
}
