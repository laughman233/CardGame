using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOCContainer
{
    public static Dictionary<Type, object> mInstance = new Dictionary<Type, object>();

    public static void Register<T>(T instance)
    {
        var key = typeof(T);
        if (mInstance.ContainsKey(key))
        {
            mInstance[key] = instance;
        }
        else
        {
            mInstance.Add(key,instance);
        }
    }

    public static T Get<T>() where T : class
    {
        var key = typeof(T);
        if (mInstance.TryGetValue(key, out var retInstance))
            return retInstance as T;
        return null;
    }
}
