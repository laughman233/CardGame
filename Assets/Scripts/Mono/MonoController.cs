using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MonoController : SingletonAuto<MonoController>
{
    private static UnityEvent _updateEvent= new UnityEvent();

    // Update is called once per frame
    void Update()
    {
        if (_updateEvent != null)
            _updateEvent.Invoke();
    }

    public void AddUpdateListener(UnityAction action)
    {
        _updateEvent.AddListener(action);
    }
    public void RemoveUpdateListener(UnityAction action)
    {
        _updateEvent.RemoveListener(action);
    }
}
