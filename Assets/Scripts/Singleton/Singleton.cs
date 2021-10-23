using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    public static T GetInstance
    {
        get { return _instance; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this as T;
    }
}
