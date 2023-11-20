using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    private Player player;
    Coroutine immunityCoroutine;
    bool isImmuneAfterHit = false;
    float immunityTime = 0; 
    SpriteRenderer spriteRenderer = null;
    const float spriteFlashInterval = 0.2f;
    WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        CallHealthEvent(0); 
        
        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();

        if (player != null)
        {
            if (player.playerDetail.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = player.playerDetail.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy.EnemyDetailSO.isImmuneAfterHit)
            {
                isImmuneAfterHit = true;
                immunityTime = enemy.EnemyDetailSO.hitImmunityTime;
                spriteRenderer = enemy.spriteRenderersArray[0];
            }
        }

        if (enemy != null && enemy.EnemyDetailSO.isHealthBarDisplayed == true && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if (healthBar != null)
        {
            healthBar.DisableHealthBar();
        }

    }

    public void TakeDamage(int damageAmount)
    {
        bool isRolling = false; 

        if (player != null)
        {
            isRolling = player.playerControl.isPlayerRolling; 
        }

        if (isDamageable && !isRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            PostHitImmunity();

            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
            }
        }
    }

    private void PostHitImmunity()
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }

        if (isImmuneAfterHit)
        {
            if (immunityCoroutine != null)
                StopCoroutine(immunityCoroutine);

            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));

        }
    }

    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamageable = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return WaitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return WaitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;
        }

        isDamageable = true;
    }

    private void CallHealthEvent(int damageAmount)
    {
        healthEvent.CallHealthChangedEvent((float)currentHealth / (float)startingHealth, currentHealth, damageAmount);

    }

    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return startingHealth;
    }

}
