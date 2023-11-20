using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetials_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC AMMO DETAILS")]
    public string ammoName;
    public bool isPlayerAmmo;

    [Space(10)]
    [Header("AMMO SPRITE, PREFAB & MATERIALS")]
    public Sprite ammoSprite;
    public GameObject[] ammoPrefabArray;
    public Material ammoMaterial;
    public float ammoChargeTime = .1f;
    public Material ammoChargeMaterial;

    [Space(10)]
    [Header("AMMO HIT EFFECT")]
    public AmmoHitEffectSO ammoHitEffect;

    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]
    public int ammoDamage = 1;
    public float ammoSpeedMin = 20f;
    public float ammoSpeedMax = 20f;
    public float ammoRange = 20f;
    public float ammoRotationSpeed = 1f;

    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    public float ammoSpreadMin = 0f;
    public float ammoSpreadMax = 0f;

    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    public int ammoSpawnAmountMin = 1;
    public int ammoSpawnAmountMax = 1;
    public float ammoSpawnIntervalMin = 0f;
    public float ammoSpawnIntervalMax = 0f;

    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]
    public bool isAmmoTrail = false;
    public float ammoTrailTime = 3f;
    public Material ammoTrailMaterial;
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtility.ValidateCheckNullValue(this, nameof(ammoSprite),ammoSprite);
        HelperUtility.ValidateCheckEnumerableValue(this,nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtility.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0) 
        {
            HelperUtility.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoMaterial);
        }
        HelperUtility.ValidateCheckPostiveValue(this, nameof(ammoDamage), ammoDamage,false);
        HelperUtility.ValidateChackPositveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtility.ValidateChackPositveRange(this,nameof(ammoSpeedMin),ammoSpreadMin,nameof(ammoSpreadMax),ammoSpreadMax,false);
        HelperUtility.ValidateChackPositveRange(this,nameof(ammoSpawnAmountMin),ammoSpawnAmountMin,nameof(ammoSpawnAmountMax),ammoSpawnAmountMax,false);
        HelperUtility.ValidateChackPositveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, false);

        if (isAmmoTrail)
        {
            HelperUtility.ValidateCheckPostiveValue(this,nameof(ammoTrailTime), ammoTrailTime,false);
            HelperUtility.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtility.ValidateCheckPostiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtility.ValidateCheckPostiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }

    }
#endif
    #endregion
}
