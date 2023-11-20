using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiateRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decoration1Tilemap;
    [HideInInspector] public Tilemap decoration2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public int[,] aStarMovementPenalty;
    [HideInInspector] public int[,] aStarItemObstacles;
    [HideInInspector] public Bounds RoomColliderBounds;
    [HideInInspector] public List<MoveIterm> moveableItermsList = new List<MoveIterm>();

    private BoxCollider2D boxCollider2D;

    [Space(10)]
    [Header("OBJECT REFERENCE")]
    [SerializeField] private GameObject environmentGameObject;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();

        RoomColliderBounds = boxCollider2D.bounds;
    }

    private void Start()
    {
        UpdateMoveableObstacles();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Settings.playerTag && room != GameManager.Instance.GetCurrentRoom())
        {
            this.room.isPreviouslyVisited = true;

            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialise(GameObject roomGameObject)
    {
        PopulateTilemapMemberVariables(roomGameObject);

        BlockOffUnusedDoorway();

        AddObstaclesAndPreferredPaths();

        CreateItemObstaclesArray();

        AddDoorsToRooms();

        DisableCollisionTilemapRenderer();
    }

    private void PopulateTilemapMemberVariables(GameObject roomGameObject)
    {
        grid = roomGameObject.GetComponentInChildren<Grid>();

        Tilemap[] tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decoration1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decoration2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }
        }
    }

    private void BlockOffUnusedDoorway()
    {
        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected)
            {
                continue;
            }

            if (collisionTilemap != null)
            {
                BlockADoorwayOnTilemapplayer(collisionTilemap, doorway);
            }
            if (minimapTilemap != null)
            {
                BlockADoorwayOnTilemapplayer(minimapTilemap, doorway);
            }
            if (frontTilemap != null)
            {
                BlockADoorwayOnTilemapplayer(frontTilemap, doorway);
            }
            if (groundTilemap != null)
            {
                BlockADoorwayOnTilemapplayer(groundTilemap, doorway);
            }
            if (decoration1Tilemap != null)
            {
                BlockADoorwayOnTilemapplayer(decoration1Tilemap, doorway);
            }
            if (decoration2Tilemap != null)
            {
                BlockADoorwayOnTilemapplayer(decoration2Tilemap, doorway);
            }

        }
    }

    private void BlockADoorwayOnTilemapplayer(Tilemap tilemap, Doorway doorway)
    {
        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                BlockDoorwayHorizontally(tilemap, doorway);
                break;
            case Orientation.east:
            case Orientation.west:
                BlockDoorwayVertically(tilemap, doorway);
                break;
            case Orientation.none:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 在水平方向上堵塞门口
    /// </summary>
    private void BlockDoorwayHorizontally(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPositon = doorway.doorwayStartCopyPosition;

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                //获取当前瓦片的变换矩阵
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPositon.x + xPos, startPositon.y - yPos, 0));

                //进行瓦片替换
                tilemap.SetTile(new Vector3Int(startPositon.x + 1 + xPos, startPositon.y - yPos, 0), 
                    tilemap.GetTile(new Vector3Int(startPositon.x + xPos, startPositon.y - yPos, 0)));

                //将变换矩阵赋值
                tilemap.SetTransformMatrix(new Vector3Int(startPositon.x + 1 + xPos, startPositon.y - yPos, 0), transformMatrix);

            }
        }
    }

    /// <summary>
    /// 在垂直方向上堵塞门口
    /// </summary>
    private void BlockDoorwayVertically(Tilemap tilemap, Doorway doorway)
    {
        Vector2Int startPositon = doorway.doorwayStartCopyPosition;

        for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
        {
            for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
            {
                //获取当前瓦片的变换矩阵
                Matrix4x4 transformMatrix = tilemap.GetTransformMatrix(new Vector3Int(startPositon.x + xPos, startPositon.y - yPos, 0));

                //进行瓦片替换
                tilemap.SetTile(new Vector3Int(startPositon.x + xPos, startPositon.y - 1 - yPos, 0), 
                    tilemap.GetTile(new Vector3Int(startPositon.x + xPos, startPositon.y - yPos, 0)));

                //将变换矩阵赋值
                tilemap.SetTransformMatrix(new Vector3Int(startPositon.x + xPos, startPositon.y - 1 - yPos, 0), transformMatrix);

            }
        }
    }

    private void DisableCollisionTilemapRenderer()
    {
        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    private void AddObstaclesAndPreferredPaths()
    {
        aStarMovementPenalty  = new int [room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1];

        for (int x = 0; x < room.templateUpperBounds.x - room.templateLowerBounds.x + 1; x++)
        {
            for (int y = 0; y < room.templateUpperBounds.y - room.templateLowerBounds.y + 1; y++)
            {
                aStarMovementPenalty[x, y] = Settings.defaultAStarMovementPenalty;

                TileBase tile = collisionTilemap.GetTile(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

                foreach (TileBase collisionTile in GameResources.Instance.enemyUnwalkableCollisionTilesArray)
                {
                    if (tile == collisionTile)
                    {
                        aStarMovementPenalty[x, y] = 0;
                        break;
                    }
                }

                if (tile == GameResources.Instance.PreferredEnemyPathTile)
                {
                    aStarMovementPenalty[x, y] = Settings.preferredPathAStarMovementPenalty;
                }
            }
        }
    }

    private void AddDoorsToRooms()
    {
        if (room.roomNodeType.isCorridorEW || room.roomNodeType.isCorridorNS)
        {
            return;
        }

        foreach (Doorway doorway in room.doorwayList)
        {
            if (doorway.isConnected && doorway.doorPrefab != null)
            {
                float tileDistance = Settings.tileSizePixels / Settings.pixelsPerUnit;

                GameObject door = null;

                if (doorway.orientation == Orientation.north)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2, doorway.position.y + tileDistance, 0);
                }else if (doorway.orientation == Orientation.south)
                {
                    door = Instantiate(doorway.doorPrefab,gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance / 2, doorway.position.y, 0);
                }
                else if (doorway.orientation == Orientation.east)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x + tileDistance, doorway.position.y + tileDistance * 1.25f, 0);
                }
                else if (doorway.orientation == Orientation.west)
                {
                    door = Instantiate(doorway.doorPrefab, gameObject.transform);
                    door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y + tileDistance * 1.25f, 0);
                }

                Door doorComponent = door.GetComponent<Door>();

                if (room.roomNodeType.isBossRoom)
                {
                    doorComponent.isBossRoomDoor = true;

                    doorComponent.LockDoor();
                }
            }
        }
    }

    public void LockDoor()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.LockDoor();
        }

        DIsableDoorCollider();
    }

    public void EnableRoomCollider()
    {
        boxCollider2D.enabled = true;
    }


    public void DIsableDoorCollider()
    {
        boxCollider2D.enabled = false;
    }

    public void ActivateEnvironmentObject()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(true);
        }
    }

    public void DeactivateEnvironmentObject()
    {
        if (environmentGameObject != null)
        {
            environmentGameObject.SetActive(false);
        }
    }

    public void UnLockDoors(float doorUnlockDelay)
    {
        StartCoroutine(UnlockDoorsRoutine(doorUnlockDelay));
    }

    private IEnumerator UnlockDoorsRoutine(float doorUnlockDelay)
    {
        if (doorUnlockDelay > 0)
        {
            yield return new WaitForSeconds(doorUnlockDelay);
        }

        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            door.UnLockDoor();
        }

        EnableRoomCollider();
    }

    private void CreateItemObstaclesArray()
    {
        aStarItemObstacles = new int[room.templateUpperBounds.x - room.templateLowerBounds.x + 1, room.templateUpperBounds.y - room.templateLowerBounds.y + 1]; ;
    }

    private void InitializeItemObstaclesArray()
    {
        for (int x = 0; x < (room.templateUpperBounds.x - room.lowerBounds.x + 1); x++)
        {
            for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
            {
                aStarItemObstacles[x, y] = Settings.defaultAStarMovementPenalty;
            }
        }
    }

    public void UpdateMoveableObstacles()
    {
        InitializeItemObstaclesArray();

        foreach (MoveIterm move in moveableItermsList)
        {
            Vector3Int colliderBoundsMin = grid.WorldToCell(move.boxCollider2D.bounds.min);
            Vector3Int colliderBoundsMax = grid.WorldToCell(move.boxCollider2D.bounds.max);

            for (int i = colliderBoundsMin.x; i < colliderBoundsMax.x; i++)
            {
                for (int j = colliderBoundsMin.y; j < colliderBoundsMax.y; j++)
                {
                    aStarItemObstacles[i - room.templateLowerBounds.x, j - room.templateLowerBounds.y] = 0;
                }
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    for (int x = 0; x < (room.templateUpperBounds.x - room.lowerBounds.x + 1); x++)
    //    {
    //        for (int y = 0; y < (room.templateUpperBounds.y - room.templateLowerBounds.y + 1); y++)
    //        {
    //            if (aStarItemObstacles[x, y] == 0)
    //            {
    //                Vector3 worldCellPos = grid.CellToWorld(new Vector3Int(x + room.templateLowerBounds.x, y + room.templateLowerBounds.y, 0));

    //                Gizmos.DrawWireCube(new Vector3(worldCellPos.x + 0.5f, worldCellPos.y + 0.5f, 0), Vector3.one);
    //            }
    //        }
    //    }
    //}

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this, nameof(environmentGameObject), environmentGameObject);
    }
#endif
    #endregion
}
