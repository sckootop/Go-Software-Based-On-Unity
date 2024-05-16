using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMsg : BaseData
{
    public abstract override int GetBytesNum();

    public abstract override int Reading(byte[] bytes, int beginIndex = 0);

    public abstract override byte[] Writing();

    public abstract int GetID();
}
