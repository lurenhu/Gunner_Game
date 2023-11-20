using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_",menuName = "Scriptable Objects/Weapons/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    [Space(10)]
    [Header("WEAPON SHOOT EFFECT DETAILS")]

    public Gradient colorGradient;

    public float duration = 0.5f;

    public float startParticalSize = 0.25f;

    public float startParticalSpeed = 3f;

    public float startLifeTime = 0.5f;

    public int maxParticalNumber = 100;

    public int emissionRate = 100;

    public int burstParticalNumber = 20;

    public float effectGravity = -0.01f;

    public Sprite sprite;

    public Vector3 velocityOverLifeTimeMin;

    public Vector3 velocityOverLifeTimeMax;

    public GameObject weaponShootEffectPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckPostiveValue(this, nameof(duration), duration, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(startParticalSize), startParticalSize, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(startParticalSpeed), startParticalSpeed, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(startLifeTime), startLifeTime, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(maxParticalNumber), maxParticalNumber, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(emissionRate), emissionRate, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(burstParticalNumber), burstParticalNumber, false);

        HelperUtility.ValidateCheckNullValue(this, nameof(weaponShootEffectPrefab), weaponShootEffectPrefab);

    }
#endif

    #endregion


}
