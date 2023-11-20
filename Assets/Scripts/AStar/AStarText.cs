using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AStarText : MonoBehaviour
{
    InstantiateRoom instantiateRoom;
    Grid Grid;
    Tilemap frontTilemap;
    Tilemap pathTilemap;
    Vector3Int startGridPosition;
    Vector3Int endGridPosition;
    TileBase startPathTile;
    TileBase finalPathTile;

    Vector3Int noValue = new Vector3Int(9999,9999,9999);
    Stack<Vector3> pathStack;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        pathStack = null;
        instantiateRoom = roomChangedEventArgs.room.instantiateRoom;
        frontTilemap = instantiateRoom.transform.Find("Grid/Tilemap4_Front").GetComponent<Tilemap>();
        Grid = instantiateRoom.transform.GetComponentInChildren<Grid>();
        startGridPosition = noValue;
        endGridPosition = noValue;

        SetUpPathTileMap();
    }

    private void SetUpPathTileMap()
    {
        Transform tilemapCloneTransform = instantiateRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        if (tilemapCloneTransform == null)
        {
            pathTilemap = Instantiate(frontTilemap, Grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.gameObject.tag = "Untagged";
        }else
        {
            pathTilemap = instantiateRoom.transform.Find("Grid/Tilemap4_Front(Clone)").GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    private void Start()
    {
        startPathTile = GameResources.Instance.PreferredEnemyPathTile;
        finalPathTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];

    }

    private void Update()
    {
        if (instantiateRoom == null || startPathTile == null || finalPathTile == null || Grid == null || pathTilemap == null) return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();
            SetStartPosition();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();
            SetEndPosition();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPath();
        }
    }

    private void DisplayPath()
    {
        if(startGridPosition == noValue || endGridPosition == noValue) return;

        Debug.Log("调用Build函数");
        pathStack = AStar.BuildPath(instantiateRoom.room,startGridPosition, endGridPosition);

        if (pathStack == null)
        {
            return;
        }

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(Grid.WorldToCell(worldPosition), startPathTile);
        }
    }

    private void ClearPath()
    {
        if (pathStack == null)
        {
            return;
        }

        foreach (Vector3 worldPosition in pathStack)
        {
            pathTilemap.SetTile(Grid.WorldToCell(worldPosition), null);
        }

        pathStack = null;

        endGridPosition = noValue;
        startGridPosition = noValue;
    }

    private void SetEndPosition()
    {
        if (endGridPosition == noValue)
        {
            endGridPosition = Grid.WorldToCell(HelperUtility.GetMouseWorldPosition());

            if (!isPositionWithinBonds(endGridPosition))
            {
                endGridPosition = noValue;
                return;
            }

            Debug.Log(endGridPosition);
            pathTilemap.SetTile(endGridPosition, finalPathTile);
        }
        else
        {
            pathTilemap.SetTile(endGridPosition, null);
            endGridPosition = noValue;
        }
    }

    private void SetStartPosition()
    {
        if (startGridPosition == noValue)
        {
            startGridPosition = Grid.WorldToCell(HelperUtility.GetMouseWorldPosition());

            if (!isPositionWithinBonds(startGridPosition))
            {
                startGridPosition = noValue;
                return;
            }

            Debug.Log(startGridPosition);
            pathTilemap.SetTile(startGridPosition, startPathTile);
        }
        else
        {
            pathTilemap.SetTile(startGridPosition, null);
            startGridPosition = noValue;
        }
    }

    private bool isPositionWithinBonds(Vector3Int Position)
    {
        if (Position.x < instantiateRoom.room.templateLowerBounds.x || Position.x > instantiateRoom.room.templateUpperBounds.x
            || Position.y < instantiateRoom.room.templateLowerBounds.y || Position.y > instantiateRoom.room.templateUpperBounds.y)
        {
            Debug.Log("超出范围");
            return false;
        }
        else
        {
            Debug.Log("范围内");
            return true;
        }
    }
}
