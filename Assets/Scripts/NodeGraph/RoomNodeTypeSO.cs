using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeType_", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    [Header("Only flag the RoomNodeType that should be visible in the editor")]
    #endregion
    public bool displayInNodeGraphEditor = true;
    #region Header
    [Header("One type should be a Corridor")]
    #endregion
    public bool isCorridor;
    public bool isCorridorNS;
    public bool isCorridorEW;
    #region Header
    [Header("One type should be an Entrance")]
    #endregion
    public bool isEntrance;
    #region Header
    [Header("One type should be a Boss Room")]
    #endregion
    public bool isBossRoom;
    #region Header
    [Header("One type should be None(Unassigned)")]
    #endregion
    public bool isNone;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this,nameof(roomNodeTypeName),roomNodeTypeName);
    }
#endif

}
