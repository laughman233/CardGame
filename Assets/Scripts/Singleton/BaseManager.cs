using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 单例模式基类
/// </summary>
public class BaseManager<T> where T : new()
{
    private static T _instance;
    private static readonly object _padlock = new object();
    
    public static T GetInstance
    {
        get
        {
            if (_instance == null)
            {
                lock (_padlock)
                {
                    if(_instance==null)
                        _instance = new T();
                }
            }
            return _instance;
        }
    }
}
//public class GameManager
//{
//    private static GameManager instance;

//    单例模式
//    public static GameManager GetInstance()
//    {
//        if (instance == null)
//            instance = new GameManager();
//        return instance;
//    }
//}
public class ObjectManager:BaseManager<ObjectManager>
{
}
