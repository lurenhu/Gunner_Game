using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonoBehaviours<DungeonBuilder>
{
    public Dictionary<string,Room> dungeonBuilderRoomDictionary = new Dictionary<string,Room>();
    private Dictionary<string,RoomTemplateSO> roomTemplateDictionary = new Dictionary<string,RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList = null;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;

    private void OnEnable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    protected override void Awake()
    {
        base.Awake();

        LoadRoomNodeTypeList();
    }

    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        //����roomTemplate��roomTemplateList��
        roomTemplateList = currentDungeonLevel.roomTemplateList;
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = SelectRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraphList);

            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            if (dungeonBuildSuccessful)
            {
                InstantiateRoomGameObject();
            }
            
        }
        return dungeonBuildSuccessful;
    }

    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key In " + roomTemplateList);
            }
        }
    }

    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }else
        {
            Debug.Log("No entrance room Node");
            return false;
        }

        bool noRoomOverlaps = true;

        noRoomOverlaps = ProcessRoomInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        if (noRoomOverlaps && openRoomNodeQueue.Count == 0)
        {
            return true;
        }else
        {
            return false;
        }

    }

    /// <summary>
    /// �� Queue<RoomNodeSO> openRoomNodeQueue ���д����� roomNodeGraph �е�ȫ������ڵ���д���
    /// </summary>
    /// <returns>��������ϲ���û�з����ص� ����true�� ���򷵻� false</returns>
    private bool ProcessRoomInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id,room);
            }else
            {
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }

    /// <summary>
    /// �ж��Ƿ������û���� parentRoom �ص�������²��� roomNode
    /// </summary>
    /// <returns> �ɹ� true </returns>
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        while (roomOverlaps)
        {
            List<Doorway> unconnectedAvailableParentsDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorwayList).ToList();

            if (unconnectedAvailableParentsDoorways.Count == 0)
            {
                return false;
            }

            Doorway doorwayParent = unconnectedAvailableParentsDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentsDoorways.Count)];

            RoomTemplateSO roomTemplate = GetRandomTemplateForRoomConsistenWithParent(roomNode, doorwayParent);

            Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

            if (PlaceTheRoom(parentRoom,doorwayParent,room))
            {
                roomOverlaps = false;

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }
        return true;
    }

    /// <summary>
    /// ���������� doorwayParent �ķ����Լ� roomNode �����ͣ���������һ��ƥ��� roomTemplate
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistenWithParent(RoomNodeSO roomNode, Doorway doorwayParent)
    {
        RoomTemplateSO roomTemplate = null;

        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;
                case Orientation.east:
                case Orientation.west:
                    roomTemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;
                case Orientation.none:
                    break;
                default:
                    break;
            }
        }
        else
        {
            roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomTemplate;
    }

    /// <summary>
    /// ���Ը��� doorwayParent ���� parentRoom �� room 
    /// </summary>
    /// <returns>�ɹ� true�� ʧ�� false</returns>
    private bool PlaceTheRoom(Room parentRoom, Doorway doorwayParent, Room room)
    {
        Doorway doorway = GetOppositeDoorway(doorwayParent, room.doorwayList);

        if (doorway == null)
        {
            doorway.isUnavailable = true;
            return false;
        }

        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.none:
                break;
            default:
                break;
        }

        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            doorway.isUnavailable = true;
            doorway.isConnected = true;
            doorwayParent.isUnavailable = true;
            doorwayParent.isConnected = true;

            return true;
        }else
        {
            doorwayParent.isUnavailable = true;
            return false;
        }

    }

    /// <summary>
    /// �� doorwayList �з����� doorwayParent �����෴�� doorway
    /// </summary>
    private Doorway GetOppositeDoorway(Doorway doorwayParent, List<Doorway> doorwayList)
    {
        foreach (Doorway doorwayToCheck in doorwayList)
        {
            if (doorwayParent.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (doorwayParent.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (doorwayParent.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (doorwayParent.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;
    }

    /// <summary>
    /// ���������� room �Ƿ������������ֵ��е����� room �����ص�
    /// </summary>
    /// <returns> ����÷������ֵ��д��ڻ��� !isPositioned ����δ�����ص� �����ؿգ�����������������䷢���ص��������������� </returns>
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        foreach (KeyValuePair<string, Room> keyValuepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuepair.Value;

            if (room.id == roomToTest.id || !room.isPositioned)
            {
                continue;
            }

            if (IsOverlappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        return null;
    }

    /// <summary>
    /// �ж� room1 �� room2 �ص�
    /// </summary>
    private bool IsOverlappingRoom(Room room1, Room room2)
    {
        bool isOverlappingX = IsOverlappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);
        bool isOverlappingY = IsOverlappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }else
        {
            return false;
        }
    }

    /// <summary>
    /// �ж����߽��ص��㷨
    /// </summary>
    private bool IsOverlappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        if (Mathf.Max(imin1,imin2)<= Mathf.Min(imax1,imax2))
        {
            return true;
        }else
        {
            return false;
        }

    }

    /// <summary>
    /// ��roomTemplateList��Ѱ��������ҪroomNodeTypeƥ���roomNodeTemplate,������ƥ���roomNodeTemplate�����ѡ��һ������
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        if(matchingRoomTemplateList.Count == 0)
            return null;

        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }

    /// <returns> ���� !isConnected �� !isUnavailable �� doorway </returns>
    private IEnumerable<Doorway> GetUnconnectedAvailableDoorways(List<Doorway> roomDoorwayList)
    {
        foreach (Doorway doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
            {
                yield return doorway;
            }
        }
    }

    /// <summary>
    /// �������� roomTemplate �� roomNode �ж�Ӧ������ֵ���и�ֵ�� room ������
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        Room room = new Room();

        room.templateId = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositonArray = roomTemplate.spawnPositionArray;
        room.enemiesByLevelList = roomTemplate.enemyByLevelList;
        room.roomEnemySpawnParametersList = roomTemplate.roomEnemySpawnParameterList;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;

        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        if (roomNode.parentRoomNodeIDList.Count == 0)
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;

            GameManager.Instance.SetCurrentRoom(room);

        }else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        if (room.GetNumberOfEnemyToSpawn(GameManager.Instance.GetCurrentDungeonLevel()) == 0)
        {
            room.isClearedOfEnemies = true;
        }

        return room;
    }


    private RoomNodeGraphSO SelectRandomRoomNodeGraph(List<RoomNodeGraphSO> roomNodeGraphList)
    {
        if (roomNodeGraphList.Count > 0)
        {
            return roomNodeGraphList[UnityEngine.Random.Range(0, roomNodeGraphList.Count)];
        }
        else
        {
            Debug.Log("No Room Node Graph In List");
            return null; 
        }
    }

    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }
        return newStringList;
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;

            newDoorwayList.Add(newDoorway);
        }
        return newDoorwayList;
    }

    private void InstantiateRoomGameObject()
    {
        foreach (KeyValuePair<string,Room> keyValuepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyValuepair.Value;

            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0);

            GameObject roomGameObject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            InstantiateRoom instantiateRoom = roomGameObject.GetComponentInChildren<InstantiateRoom>();

            instantiateRoom.room = room;

            instantiateRoom.Initialise(roomGameObject);

            room.instantiateRoom = instantiateRoom;

            //if (!room.roomNodeType.isBossRoom)
            //{
            //    room.isClearedOfEnemies = true;
            //}
        }
    }

    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID,out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }else
        {
            return null;
        }
    }

    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID,out Room room))
        {
            return room;
        }else
        {
            return null;
        }
    }


    private void ClearDungeon()
    {
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string,Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if (room.instantiateRoom != null)
                {
                    Destroy(room.instantiateRoom.gameObject);
                }
            }
            dungeonBuilderRoomDictionary.Clear();
        }
    }

}
