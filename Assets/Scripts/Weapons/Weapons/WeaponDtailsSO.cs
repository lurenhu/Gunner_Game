using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDtails_",menuName = "Scriptable Objects/Weapons/Weapon Dtails")]
public class WeaponDtailsSO : ScriptableObject
{
    [Space(10)]
    [Header("WEAPON BASE DETAILS")]
    public string weaponName;
    public Sprite weaponSprite;

    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    public Vector3 weaponShootPosition;
    public AmmoDetailSO weaponCurrentAmmo;
    public SoundEffectSO weaponFiringSoundEffect;
    public SoundEffectSO weaponReloadSoundEffect;
    public WeaponShootEffectSO weaponShootEffect;

    [Space(10)]
    [Header("WEAPON OPERATING VALUES")]
    [Tooltip("�Ƿ������޵�ҩ")]
    public bool hasInfiniteAmmo = false;
    [Tooltip("�Ƿ������޵�������")]
    public bool hasInfiniteClipCapacity = false;
    [Tooltip("��������")]
    public int weaponClipAmmoCapacity = 6;
    [Tooltip("������ҩ����")]
    public int weaponAmmoCapacity = 100;
    [Tooltip("��������Ƶ��")]
    public float weaponFireRate = .2f;
    [Tooltip("����װ��ʱ��")]
    public float weaponPrechargeTime = 0f;
    [Tooltip("����װ��ʱ��")]
    public float weaponReloadTime = 0f;

    #region validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this,nameof(weaponName),weaponName);
        HelperUtility.ValidateCheckNullValue(this,nameof(weaponCurrentAmmo),weaponCurrentAmmo);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(weaponFireRate), weaponFireRate,false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(weaponPrechargeTime), weaponPrechargeTime, false);

        if (!hasInfiniteAmmo)
            HelperUtility.ValidateCheckPostiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity,false);

        if (!hasInfiniteClipCapacity)
            HelperUtility.ValidateCheckPostiveValue(this,nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity,false);
    }
#endif
    #endregion

}
