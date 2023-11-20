using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DestroyEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(SetWeaponActiveEvent))]
[RequireComponent(typeof(WeaponFireEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Animator))]

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    HealthEvent healthEvent;
    Health health;
    [HideInInspector] public EnemyDetailSO EnemyDetailSO;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    FireWeapon fireWeapon;
    SetWeaponActiveEvent setWeaponActiveEvent;  
    EnemyMovementAI enemyMovementAI;
    MaterializeEffect materializeEffect;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public SpriteRenderer[] spriteRenderersArray;
    CircleCollider2D circleCollider2D;
    PolygonCollider2D polygonCollider2D;
    [HideInInspector] public Animator animator;

    private void Awake()
    {
        health = GetComponent<Health>();
        healthEvent = GetComponent<HealthEvent>();
        fireWeapon = GetComponent<FireWeapon>();
        setWeaponActiveEvent = GetComponent<SetWeaponActiveEvent>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        materializeEffect = GetComponent<MaterializeEffect>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRenderersArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        if (healthEventArgs.healthAmount <= 0)
        {
            EnemyDestroyed();
        }
    }

    private void EnemyDestroyed()
    {
        DestroyEvent destroyEvent = GetComponent<DestroyEvent>();

        destroyEvent.CallDestroyedEvent(false, health.GetStartingHealth());
    }

    public void EnemyInitialization(EnemyDetailSO enemyDetailSO, int enemySpawnNumber, DungeonLevelSO dungeonLevelSO)
    {
        this.EnemyDetailSO = enemyDetailSO;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingHealth(dungeonLevelSO);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();

        StartCoroutine(MaterializeEnemy());
    }

    private void SetEnemyStartingHealth(DungeonLevelSO dungeonLevelSO)
    {
        foreach (EnemyHealthDetail enemyHealthDetail in EnemyDetailSO.enemyHealthDetailsArray)
        {
            if (enemyHealthDetail.dungeonLevel == dungeonLevelSO)
            {
                health.SetStartingHealth(enemyHealthDetail.enemyHealthAmount);
                return;
            }
        }

        health.SetStartingHealth(Settings.defaultEnemyHealth);
    }

    private void SetEnemyStartingWeapon()
    {
        if (EnemyDetailSO.enemyWeapon != null)
        {
            Weapon weapon = new Weapon()
            {
                weaponDetails = EnemyDetailSO.enemyWeapon,
                weaponReloadTimer = 0f,
                weaponClipRemainingAmmo = EnemyDetailSO.enemyWeapon.weaponClipAmmoCapacity,
                weaponRemainingAmmo = EnemyDetailSO.enemyWeapon.weaponAmmoCapacity,
                isWeaponReloading = false
            };

            setWeaponActiveEvent.CallSetWeaponActiveEvent(weapon);
        }
    }

    private IEnumerator MaterializeEnemy()
    {
        EnemyEnable(false);

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(EnemyDetailSO.enemyMaterializeShader, EnemyDetailSO.enemyMaterializeColor
            , EnemyDetailSO.enemyMaterializeTime, spriteRenderersArray, EnemyDetailSO.enemyStandarMaterial));

        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnable)
    {
        circleCollider2D.enabled = isEnable;
        polygonCollider2D.enabled = isEnable;

        enemyMovementAI.enabled = isEnable;

        fireWeapon.enabled = isEnable;
    }

    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber % Settings.targetFrameRateToSpreadPathfindingOver);
    }

    private void SetEnemyAnimationSpeed()
    {
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimation;
    }
}
