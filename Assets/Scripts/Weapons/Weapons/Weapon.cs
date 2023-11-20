using UnityEngine;

[System.Serializable]
public class Weapon
{
    public WeaponDtailsSO weaponDetails;
    public int weaponListPosition;
    public float weaponReloadTimer;
    public int weaponClipRemainingAmmo;
    public int weaponRemainingAmmo;
    public bool isWeaponReloading;
}
