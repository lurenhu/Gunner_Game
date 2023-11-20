using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]

public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] MovementDetalSO movementDetalSO;
    Enemy enemy;
    Stack<Vector3> movementSteps = new Stack<Vector3>();
    Vector3 playerReferencePosition;
    Coroutine moveEnemyRoutine;
    float currentEnemyPathRebuildCooldown;
    WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1;
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();  

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetalSO.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.EnemyDetailSO.chaseDistance)
        {
            chasePlayer = true;
        }

        if (!chasePlayer)
        {
            return;
        }

        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber)
        {
            return;
        }

        if (currentEnemyPathRebuildCooldown <= 0f || 
            Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath)
        {
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            CreatPath();

            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            while (Vector3.Distance(nextPosition,transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed, (nextPosition - transform.position).normalized , false);
                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;    
        }

        enemy.idleEvent.CallIdleEvent();
    }

    private void CreatPath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiateRoom.grid;

        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        if (movementSteps != null)
        {
            movementSteps.Pop();
        }else
        {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiateRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int Obstacle = Mathf.Min(currentRoom.instantiateRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x,adjustedPlayerCellPosition.y],
            currentRoom.instantiateRoom.aStarItemObstacles[adjustedPlayerCellPosition.x,adjustedPlayerCellPosition.y]);

        if (Obstacle != 0)
        {
            return playerCellPosition;
        }

        else
        {
            surroundingPositionList.Clear();

            for (int i = -1; i < 1; i++)
            {
                for (int j = -1; j < 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }

            for (int i = 0; i < 8; i++)
            {
                int index = Random.Range(0, surroundingPositionList.Count);

                try
                {
                    Obstacle = Mathf.Min(currentRoom.instantiateRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + surroundingPositionList[index].x,
                        adjustedPlayerCellPosition.y + surroundingPositionList[index].y], currentRoom.instantiateRoom.aStarItemObstacles[adjustedPlayerCellPosition.x
                        + surroundingPositionList[index].x, adjustedPlayerCellPosition.y + surroundingPositionList[index].y]);

                    if (Obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                    }
                }
                catch
                {

                }

                surroundingPositionList.RemoveAt(index);
            }

            return (Vector3Int)currentRoom.spawnPositonArray[Random.Range(0, currentRoom.spawnPositonArray.Length)];

            //for (int x = -1; x <= 1; x++)
            //{
            //    for (int y = -1; y <= 1;  y++)
            //    {
            //        if (x == 0 && y == 0)
            //        {
            //            continue;
            //        }

            //        try
            //        {
            //            Obstacle = currentRoom.instantiateRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + x, adjustedPlayerCellPosition.y + y];
            //            if (Obstacle != 0)
            //            {
            //                return new Vector3Int(playerCellPosition.x + x, playerCellPosition.y + y, 0);
            //            }
            //        }
            //        catch 
            //        {

            //            continue;
            //        }
            //    }
            //}
            // return playerCellPosition;
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this, nameof(movementDetalSO), movementDetalSO);
    }

#endif
    #endregion
}
