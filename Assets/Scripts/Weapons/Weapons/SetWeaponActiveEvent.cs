using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class SetWeaponActiveEvent : MonoBehaviour
{
    public event Action<SetWeaponActiveEvent, SetWeaponActiveEventArgs> OnSetWeaponActive;

    public void CallSetWeaponActiveEvent(Weapon weapon)
    {
        OnSetWeaponActive?.Invoke(this, new SetWeaponActiveEventArgs() { weapon = weapon });
    }
}


public class SetWeaponActiveEventArgs : EventArgs
{
    public Weapon weapon;
}