using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Objects/Player/Player Details")]
public class PlayerDetailSO : ScriptableObject
{
    [Space(10)]
    [Header("PLAYER BASE DETAIILS")]
    public string playerCharacterName;
    public GameObject playerPrefab;
    public RuntimeAnimatorController runtimeAnimatorController;

    [Space(10)]
    [Header("PLAER HEALTH")]
    public int playerHealthAmount;
    public bool isImmuneAfterHit = false;
    public float hitImmunityTime;

    [Space(10)]
    [Header("WEAPON")]
    public WeaponDtailsSO startingWeapon;
    public List<WeaponDtailsSO> startingWeaponList;

    [Space(10)]
    [Header("OTHER")]
    public Sprite playerMiniMapIcon;
    public Sprite playerHandSprite;


#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this, nameof(playerCharacterName), playerCharacterName);
        HelperUtility.ValidateCheckNullValue(this,nameof(playerPrefab), playerPrefab);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(playerHealthAmount), playerHealthAmount, false);
        HelperUtility.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        HelperUtility.ValidateCheckNullValue(this, nameof(playerMiniMapIcon), playerMiniMapIcon);
        HelperUtility.ValidateCheckNullValue(this,nameof(playerHandSprite), playerHandSprite);
        HelperUtility.ValidateCheckNullValue(this,nameof(runtimeAnimatorController),runtimeAnimatorController);
        HelperUtility.ValidateCheckEnumerableValue(this, nameof(startingWeaponList), startingWeaponList);

        if (isImmuneAfterHit)
        {
            HelperUtility.ValidateCheckPostiveValue(this,nameof(hitImmunityTime),hitImmunityTime,false);
        }
    }

#endif
}
