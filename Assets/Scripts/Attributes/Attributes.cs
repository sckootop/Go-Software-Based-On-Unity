using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//存放各个特性
public class IntOptionsAttribute : PropertyAttribute
{
    public int[] Options;

    public IntOptionsAttribute(params int[] options)
    {
        this.Options = options;
    }
}