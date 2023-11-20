using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonMonoBehaviours<EnemySpawner>
{
    int enemiesToSpawn;
    int currentEnemyCount;
    int enemierSpawnerSoFar;
    int enemyMaxConcurrentSpawnNumber;
    Room currentRoom;
    RoomEnemySpawnParameters roomEnemySpawnParameters;

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
        enemierSpawnerSoFar = 0;
        currentEnemyCount = 0;

        currentRoom = roomChangedEventArgs.room;

        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
        {
            return;
        }

        if (currentRoom.isClearedOfEnemies)
        {
            return;
        }

        enemiesToSpawn = currentRoom.GetNumberOfEnemyToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

        if (enemiesToSpawn == 0)
        {
            currentRoom.isClearedOfEnemies = true;

            return;
        }

        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

        currentRoom.instantiateRoom.LockDoor();

        SpawnEnemies();
    }


    private void SpawnEnemies()
    {
        if (GameManager.Instance.gameState == GameState.bossStage)
        {
            GameManager.Instance.previousGameState = GameState.bossStage;
            GameManager.Instance.gameState = GameState.engagingBoss;
        }

        else if (GameManager.Instance.gameState == GameState.playingLevel)
        {
            GameManager.Instance.previousGameState = GameState.playingLevel;
            GameManager.Instance.gameState = GameState.engagingEnemies;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        Grid grid = currentRoom.instantiateRoom.grid;

        RandownSpawnableObject<EnemyDetailSO> randomEnemyHelperClass = new RandownSpawnableObject<EnemyDetailSO>(currentRoom.enemiesByLevelList);

        if (currentRoom.spawnPositonArray.Length > 0)
        {
            for (int i = 0; i < enemiesToSpawn ; i++)
            {
                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
                {
                    yield return null;
                }

                Vector3Int CellPosition = (Vector3Int)currentRoom.spawnPositonArray[Random.Range(0, currentRoom.spawnPositonArray.Length)];

                CreatEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(CellPosition));

                yield return new WaitForSeconds(GetEnemySpawnInterval());
            }
        }
    }

    private void CreatEnemy(EnemyDetailSO enemyDetailSO, Vector3 position)
    {
        enemierSpawnerSoFar++;

        currentEnemyCount++;

        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

        GameObject enemy = Instantiate(enemyDetailSO.enemyPrefab, position, Quaternion.identity, transform);

        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetailSO,enemierSpawnerSoFar,dungeonLevel);

        enemy.GetComponent<DestroyEvent>().OnDestroyed += EnemySpawner_OnDestroyed;

    }

    private void EnemySpawner_OnDestroyed(DestroyEvent destroyEvent, DestroyEventArgs destroyEventArgs)
    {
        destroyEvent.OnDestroyed -= EnemySpawner_OnDestroyed;

        currentEnemyCount--;

        StaticEventHandler.CallPointsScoredEvent(destroyEventArgs.points);

        if (currentEnemyCount <= 0 && enemierSpawnerSoFar == enemiesToSpawn)
        {
            currentRoom.isClearedOfEnemies = true;

            if (GameManager.Instance.gameState == GameState.engagingEnemies)
            {
                GameManager.Instance.gameState = GameState.playingLevel;
                GameManager.Instance.previousGameState = GameState.engagingEnemies;
            }
            else if (GameManager.Instance.gameState == GameState.engagingBoss)
            {
                GameManager.Instance.gameState = GameState.bossStage;
                GameManager.Instance.previousGameState = GameState.engagingBoss;
            }

            currentRoom.instantiateRoom.UnLockDoors(Settings.doorUnlockDelay);

            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
        }
    }

    private float GetEnemySpawnInterval()
    {
        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
    }


    private int GetConcurrentEnemies()
    {
        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
    }

}
