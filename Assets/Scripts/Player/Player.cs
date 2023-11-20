using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(ReseiveContactDamage))]
[RequireComponent(typeof(DestroyEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(SetWeaponActiveEvent))]
[RequireComponent(typeof(WeaponFireEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
#endregion
public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailSO playerDetail;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public DestroyEvent destroyEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public SetWeaponActiveEvent setWeaponActiveEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public WeaponFireEvent weaponFireEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public FireWeaponEvent FireWeaponEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        destroyEvent = GetComponent<DestroyEvent>();
        playerControl = GetComponent<PlayerControl>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        FireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFireEvent = GetComponent<WeaponFireEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        setWeaponActiveEvent = GetComponent<SetWeaponActiveEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(PlayerDetailSO playerDetails)
    {
        this.playerDetail = playerDetails;

        CreatPlayerStartingWeapons();

        SetPlayerHealth();
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
        if (healthEventArgs.healthAmount <= 0f)
        {
            destroyEvent.CallDestroyedEvent(true,0);
        }
    }


    private void CreatPlayerStartingWeapons()
    {
        weaponList.Clear();

        foreach (WeaponDtailsSO weaponDetails in playerDetail.startingWeaponList)
        {
            AddWeaponToPlayer(weaponDetails);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Weapon AddWeaponToPlayer(WeaponDtailsSO weaponDetails)
    {
        Weapon weapon = new Weapon() { weaponDetails = weaponDetails, weaponReloadTimer = 0f, weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity, 
            weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity, isWeaponReloading = false };

        weaponList.Add(weapon);

        weapon.weaponListPosition = weaponList.Count;

        setWeaponActiveEvent.CallSetWeaponActiveEvent(weapon);

        return weapon;
    }

    private void SetPlayerHealth()
    {
        health.SetStartingHealth(playerDetail.playerHealthAmount);
    }
}
