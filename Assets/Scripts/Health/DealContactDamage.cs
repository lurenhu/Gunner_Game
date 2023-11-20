using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    [Space(10)]
    [Header("DEAL DAMAGE")]
    [SerializeField] private int contactDamageAmount;
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding)
        {
            return;
        }

        ContactDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isColliding)
        {
            return;
        }

        ContactDamage(collision);
    }

    private void ContactDamage(Collider2D collision)
    {
        int collisionObjectLayerMask = (1 << collision.gameObject.layer);

        if ((layerMask.value & collisionObjectLayerMask) == 0)
        {
            return ;
        }

        ReseiveContactDamage reseiveContactDamage = collision.gameObject.GetComponent<ReseiveContactDamage>();

        if (reseiveContactDamage != null)
        {
            isColliding = true;

            Invoke("ResetContactCollision",Settings.contactDamageCollisionResetDelay);

            reseiveContactDamage.TakeContactDamage(contactDamageAmount);
        }
    }

    private void ResetContactCollision()
    {
        isColliding = false;
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckPostiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }
#endif
    #endregion

}
