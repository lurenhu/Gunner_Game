using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] Transform weaponShootPosition;
    Enemy enemy;
    EnemyDetailSO enemyDetailSO;
    float firingIntervalTimer;
    float firingDurationTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();  
    }

    private void Start()
    {
        enemyDetailSO = enemy.EnemyDetailSO;

        firingIntervalTimer = WeaponShootInterval();
        firingDurationTimer = WeaponShootDuration();
    }

    private void Update()
    {
        firingIntervalTimer -= Time.deltaTime;

        if (firingIntervalTimer < 0)
        {
            if (firingDurationTimer >= 0)
            {
                firingDurationTimer -= Time.deltaTime;

                FireWeapon();
            }
            else
            {
                firingIntervalTimer = WeaponShootInterval();
                firingDurationTimer = WeaponShootDuration();
            }
        }
    }

    private void FireWeapon()
    {
        Vector3 playerDirectionVector = GameManager.Instance.GetPlayer().GetPlayerPosition() - transform.position;

        Vector3 weaponDirection = GameManager.Instance.GetPlayer().GetPlayerPosition() - weaponShootPosition.position;

        float weaponAngleDegree = HelperUtility.GetAngleFromVector(weaponDirection);

        float enemyAngleDegree = HelperUtility.GetAngleFromVector(playerDirectionVector);

        AimDirection enemyAimDirection = HelperUtility.GetAimDirection(enemyAngleDegree);

        Debug.Log(playerDirectionVector);
        Debug.Log(weaponDirection);
        Debug.Log(weaponAngleDegree);
        Debug.Log(enemyAngleDegree);
        Debug.Log(enemyAimDirection);

        enemy.aimWeaponEvent.CallAimWeaponEvent(enemyAimDirection,enemyAngleDegree,weaponAngleDegree,weaponDirection);

        if (enemyDetailSO.enemyWeapon != null)
        {
            float enemyAmmoRang = enemyDetailSO.enemyWeapon.weaponCurrentAmmo.ammoRange;

            if (playerDirectionVector.magnitude <= enemyAmmoRang)
            {
                if (enemyDetailSO.firingLineOfSightRequired && !isPlayerInLineOfSight(weaponDirection, enemyAmmoRang)) return;

                enemy.fireWeaponEvent.CallFireWeaponEvent(true, true, enemyAimDirection, enemyAngleDegree, weaponAngleDegree, weaponDirection);
            }
        }
    }

    private bool isPlayerInLineOfSight(Vector3 weaponDirection, float enemyAmmoRang)
    {
        RaycastHit2D rayCastHit2D = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirection, enemyAmmoRang, layerMask);

        if (rayCastHit2D && rayCastHit2D.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;

    }

    private float WeaponShootDuration()
    {
        return Random.Range(enemyDetailSO.firingDurationMin, enemyDetailSO.firingDurationMax);
    }

    private float WeaponShootInterval()
    {
        return Random.Range(enemyDetailSO.firingIntervalMin, enemyDetailSO.firingIntervalMax);
    }


    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this, nameof(weaponShootPosition), weaponShootPosition);
    }
#endif
    #endregion

}
