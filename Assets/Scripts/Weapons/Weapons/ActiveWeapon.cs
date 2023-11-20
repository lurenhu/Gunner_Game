using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetWeaponActiveEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;
    [SerializeField] private Transform weaponShootPositionTransform;
    [SerializeField] private Transform weaponEffectPositionTransform;

    private SetWeaponActiveEvent setWeaponEvent;
    private Weapon currentWeapon;

    private void Awake()
    {
        setWeaponEvent = GetComponent<SetWeaponActiveEvent>();
    }

    private void OnEnable()
    {
        setWeaponEvent.OnSetWeaponActive += SetWeaponEvent_OnSetWeaponActive;
    }

    private void OnDisable()
    {
        setWeaponEvent.OnSetWeaponActive -= SetWeaponEvent_OnSetWeaponActive;
    }

    /// <summary>
    /// 设置角色所携带的武器的初始值（初始化武器属性）
    /// </summary>
    private void SetWeaponEvent_OnSetWeaponActive(SetWeaponActiveEvent setWeaponActiveEvent, SetWeaponActiveEventArgs setWeaponActiveEventArgs)
    {
        SetWeapon(setWeaponActiveEventArgs.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        weaponSpriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;

        if (weaponPolygonCollider2D != null && weaponSpriteRenderer.sprite != null)
        {
            List<Vector2> spritePhysicsShapePointsList = new List<Vector2>();
            weaponSpriteRenderer.sprite.GetPhysicsShape(0, spritePhysicsShapePointsList);

            weaponPolygonCollider2D.points = spritePhysicsShapePointsList.ToArray();

        }

        weaponShootPositionTransform.localPosition = currentWeapon.weaponDetails.weaponShootPosition;
    }

    public AmmoDetailSO GetCurrentAmmo()
    {
        return currentWeapon.weaponDetails.weaponCurrentAmmo;
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public Vector3 GetShootPosition()
    {
        return weaponShootPositionTransform.position;
    }

    public Vector3 GetShootEffectPosition()
    {
        return weaponEffectPositionTransform.position;
    }

    public void RemoveCurrentWeapon()
    {
        currentWeapon = null;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this,nameof(weaponSpriteRenderer),weaponSpriteRenderer);
        HelperUtility.ValidateCheckNullValue(this,nameof(weaponPolygonCollider2D),weaponPolygonCollider2D);
        HelperUtility.ValidateCheckNullValue(this, nameof(weaponShootPositionTransform), weaponShootPositionTransform);
        HelperUtility.ValidateCheckNullValue(this, nameof(weaponEffectPositionTransform), weaponEffectPositionTransform);
    }
#endif
    #endregion
}
