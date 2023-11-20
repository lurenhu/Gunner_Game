using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyEvent))]
[DisallowMultipleComponent]
public class Destroyed : MonoBehaviour
{
    DestroyEvent destroyEvent;

    private void Awake()
    {
        destroyEvent = GetComponent<DestroyEvent>();
    }

    private void OnEnable()
    {
        destroyEvent.OnDestroyed += DestroyEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        destroyEvent.OnDestroyed -= DestroyEvent_OnDestroyed;
    }

    private void DestroyEvent_OnDestroyed(DestroyEvent destroyEvent, DestroyEventArgs destroyEventArgs)
    {
        if (destroyEventArgs.isPlayerDied)
        {
            gameObject.SetActive(false);
        }else
        {
            Destroy(gameObject);
        }

    }
}
