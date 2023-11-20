using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponReloadedEvent : MonoBehaviour
{
    public Action<WeaponReloadedEvent, WeaponReloadedEventArgs> OnWeaponReloaded;

    public void CallWeaponReloadedEvent(Weapon weapon)
    {
        OnWeaponReloaded?.Invoke(this, new WeaponReloadedEventArgs() { weapon = weapon });
    }
}

public class WeaponReloadedEventArgs : EventArgs
{
    public Weapon weapon;
}