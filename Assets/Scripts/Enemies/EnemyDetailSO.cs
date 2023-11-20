
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_" , menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailSO : ScriptableObject
{
    [Space(10)]
    [Header("BASE ENEMY DETAIL")]

    public string enemyName;
    public GameObject enemyPrefab;
    public float chaseDistance = 50f;

    [Space(10)]
    [Header("ENEMY MATERIAL")]
    public Material enemyStandarMaterial;

    [Space(10)]
    [Header("ENEMY MATERIAL SETTINGS")]
    public float enemyMaterializeTime;
    public Shader enemyMaterializeShader;
    public Color enemyMaterializeColor;

    [Space(10)]
    [Header("ENEMY WEAPON SETTINGS")]
    public WeaponDtailsSO enemyWeapon;
    public float firingIntervalMin = 0.1f;
    public float firingIntervalMax = 1f;
    public float firingDurationMin = 1f;
    public float firingDurationMax = 2f;
    public bool firingLineOfSightRequired;

    [Space(10)]
    [Header("ENEMY HEALTH")]
    public EnemyHealthDetail[] enemyHealthDetailsArray;
    public bool isImmuneAfterHit = false;
    public float hitImmunityTime;
    public bool isHealthBarDisplayed = false;


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this,nameof(enemyName),enemyName);
        HelperUtility.ValidateCheckNullValue(this,nameof(enemyPrefab),enemyPrefab);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(chaseDistance), chaseDistance,false);
        HelperUtility.ValidateCheckNullValue(this, nameof(enemyStandarMaterial), enemyStandarMaterial);
        HelperUtility.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtility.ValidateCheckPostiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        HelperUtility.ValidateChackPositveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingDurationMax, false);
        HelperUtility.ValidateChackPositveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        if (isImmuneAfterHit)
        {
            HelperUtility.ValidateCheckPostiveValue(this, nameof(hitImmunityTime), hitImmunityTime,false);
        }
    }
#endif

    #endregion
}
