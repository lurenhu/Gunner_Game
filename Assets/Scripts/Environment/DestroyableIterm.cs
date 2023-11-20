using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyableIterm : MonoBehaviour
{
    [Header("HEALTH")]
    [SerializeField] private int startingHealthAmount = 1;
    [Header("SOUND EFFECT")]
    [SerializeField] private SoundEffectSO destroyedSoundEffect;
    Animator animator;
    BoxCollider2D boxCollider2D;
    HealthEvent healthEvent;
    Health health;
    ReseiveContactDamage reseiveContactDamage;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHealthAmount);
        reseiveContactDamage = GetComponent<ReseiveContactDamage>();
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
            StartCoroutine(PlayAnimation());
        }
    }

    private IEnumerator PlayAnimation()
    {
        Destroy(boxCollider2D);

        if (destroyedSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroyedSoundEffect);
        }

        animator.SetBool(Settings.destroy,true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroy))
        {
            yield return null;
        }

        Destroy(animator);
        Destroy(reseiveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        Destroy(this);

    }
}
