using System;
using UnityEngine;

public class DestroyEvent : MonoBehaviour
{
    public event Action<DestroyEvent, DestroyEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool isPlayerDied, int points)
    {
        OnDestroyed?.Invoke(this, new DestroyEventArgs()
        {
            isPlayerDied = isPlayerDied,
            points = points
        });
    }
}

public class DestroyEventArgs : EventArgs
{
    public bool isPlayerDied;
    public int points;

}