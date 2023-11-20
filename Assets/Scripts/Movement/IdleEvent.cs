using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class IdleEvent : MonoBehaviour
{
   public event Action<IdleEvent> OnIdle;

    /// <summary>
    /// 调用事件OnIdle中的方法
    /// </summary>
    public void CallIdleEvent()
    {
        OnIdle?.Invoke(this);
    }
}
