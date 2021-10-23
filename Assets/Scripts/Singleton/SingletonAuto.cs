using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAuto<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _padlock;

    public static T GetInstance
    {
        get
        {
            if (_padlock == null)
                _padlock = new object();
            if (_instance == null)
            {
                lock (_padlock)
                {
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).ToString();
                        _instance = obj.AddComponent<T>();
                    }
                }
            }
            return _instance;
        }
    }
}