using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
    [Space(10)]
    [Header("Reference")]
    public SpriteRenderer spriteRenderer;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this,nameof(spriteRenderer),spriteRenderer);
    }
#endif
    #endregion
}
