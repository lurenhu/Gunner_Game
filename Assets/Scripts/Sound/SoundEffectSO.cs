using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_",menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    [Space(10)]
    [Header("SOUNDE EFFECT DETAILS")]
    public string soundEffectName;
    public GameObject soundPrefab;
    public AudioClip soundEffectClip;
    [Range(0f, 1.5f)] public float soundEffectPitchRandomVariationMin = .8f;
    [Range(0f, 1.5f)] public float soundEffectPitchRandomVariationMax = 1.2f;
    [Range(0f, 1f)] public float soundEffectVolume = 1f;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtility.ValidateCheckNullValue(this, nameof(soundPrefab), soundPrefab);
        HelperUtility.ValidateCheckNullValue(this, nameof(soundEffectClip), soundEffectClip);
        HelperUtility.ValidateChackPositveRange(this, nameof(soundEffectPitchRandomVariationMin), soundEffectPitchRandomVariationMin,
            nameof(soundEffectPitchRandomVariationMax), soundEffectPitchRandomVariationMax, false);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(soundEffectVolume), soundEffectVolume, true);
    }
#endif
    #endregion
}
