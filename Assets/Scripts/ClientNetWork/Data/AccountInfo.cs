using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class AccountInfo : BaseData
{
    public string account;
    public string password;

    public AccountInfo() { }
    public AccountInfo(string _account, string _password)
    {
        this.account = _account;
        this.password = _password;
    }

    public override int GetBytesNum()
    {
        return 4 + Encoding.UTF8.GetByteCount(account) + 4 + Encoding.UTF8.GetByteCount(password);
    }

    public override int Reading(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        account = ReadString(bytes, ref index);
        password = ReadString(bytes, ref index);
        return index - beginIndex;
    }

    public override byte[] Writing()
    {
        int index = 0;
        int byteNums = GetBytesNum();
        byte[] bytes = new byte[byteNums];
        WriteString(bytes, account, ref index);
        WriteString(bytes, password, ref index);
        return bytes;
    }
}

