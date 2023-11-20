using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReseiveContactDamage : MonoBehaviour
{
    [SerializeField] private int contactDamageAmount;
    Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeContactDamage(int damageAmount = 0)
    {
        if (contactDamageAmount > 0)
        {
            damageAmount = contactDamageAmount;
        }

        health.TakeDamage(damageAmount);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckPostiveValue(this, nameof(contactDamageAmount), contactDamageAmount, false);
    }
#endif
    #endregion
}
