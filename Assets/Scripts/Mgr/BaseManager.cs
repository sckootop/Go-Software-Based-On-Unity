using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//À´÷ÿºÏ≤‚¿¡º”‘ÿ µ•¿˝
public abstract class BaseManager<T> where T: class, new()
{
    private volatile static T instance;
    private static readonly object locker = new object(); 

    public static T Instance
    {
        get
        {
            if(instance==null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }
}
