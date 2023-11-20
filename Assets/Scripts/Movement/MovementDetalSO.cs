using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetals_" , menuName = "Scriptable Objects/Movement/MovementDetals")]
public class MovementDetalSO : ScriptableObject
{
    [Space(10)]
    [Header("MOVEMENT DETALS")]
    public float minMoveSpeed = 8f;
    public float maxMoveSpeed = 8f;
    public float rollSpeed;
    public float rollDistance;
    public float rollCooldownTime;

    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateChackPositveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollSpeed != 0 || rollDistance != 0 || rollCooldownTime != 0)
        {
            HelperUtility.ValidateCheckPostiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtility.ValidateCheckPostiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtility.ValidateCheckPostiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif
    #endregion
}
