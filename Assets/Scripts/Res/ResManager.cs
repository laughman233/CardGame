using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResManager : BaseManager<ResManager>
{
    //sync
    public T Load<T>(string name)where T:Object
    {
        T res= Resources.Load<T>(name);
        if (res is GameObject)
        {
            return GameObject.Instantiate(res);
        }
        return res;
    }
    //async
    public void LoadAsync<T>(string name,UnityAction<T> callback) where T:Object
    {
        MonoController.GetInstance.StartCoroutine(LoadAsyncCoroutine(name,callback));
    }

    private IEnumerator LoadAsyncCoroutine<T>(string name,UnityAction<T> callback) where T:Object
    {
        ResourceRequest rq=Resources.LoadAsync<T>(name);
        yield return rq;
        if (rq.asset is GameObject)
            callback(GameObject.Instantiate(rq.asset) as T);
        else
            callback(rq.asset as T);            
    }
}
