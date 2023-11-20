using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Objects/Dungeon/Dungeon Level")] 
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC LEVEL DETALS")]
    [Tooltip("The name for the level")]
    public string levelName;

    [Space(10)]
    [Header("ROOM TEMPLATE FOR THE LEVEL")]
    public List<RoomTemplateSO> roomTemplateList;


    [Space(10)]
    [Header("ROOM NODE GRAPH FOR THE LEVEL")]
    public List<RoomNodeGraphSO> roomNodeGraphList;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if (HelperUtility.ValidateCheckEnumerableValue(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }
        if (HelperUtility.ValidateCheckEnumerableValue(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }

        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (roomTemplate == null)
                return;

            if (roomTemplate.roomNodeType.isCorridorEW)
                isEWCorridor = true;

            if (roomTemplate.roomNodeType.isCorridorNS)
                isNSCorridor = true;

            if (roomTemplate.roomNodeType.isEntrance)
                isEntrance = true;
        }

        if (!isEntrance)
            Debug.Log("In " + this.name.ToString() + " : NO Entrance Corridor Room Type Specify");
        if (!isEWCorridor)
            Debug.Log("In " + this.name.ToString() + " : NO EW Corridor Room Type Specify");
        if (!isNSCorridor)
            Debug.Log("In " + this.name.ToString() + " : NO NS Corridor Room Type Specify");

        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return ;

            foreach (RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
            {
                if(roomNode == null)
                    continue ;

                if (roomNode.roomNodeType.isEntrance || roomNode.roomNodeType.isCorridor || roomNode.roomNodeType.isCorridorEW || 
                    roomNode.roomNodeType.isCorridorNS || roomNode.roomNodeType.isNone)
                    continue ;

                bool isRoomNodeTypeFound = false;

                foreach (RoomTemplateSO roomTemplate in roomTemplateList)
                {
                    if (roomTemplate == null)
                        continue;

                    if (roomTemplate.roomNodeType == roomNode.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break ;
                    }
                }

                if (!isRoomNodeTypeFound)
                    Debug.Log("In " + this.name.ToString() + " : NO room template " + roomNode.roomNodeType.name.ToString() +
                        " found for node graph " + roomNodeGraph.name.ToString());
            }

        }

    }

#endif
    #endregion
}
