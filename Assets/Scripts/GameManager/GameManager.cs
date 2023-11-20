using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonoBehaviours<GameManager>
{
    [Space(10)]
    [Header("GAMEOBJECT REFERENCE")]
    [SerializeField] private TextMeshProUGUI messageTextTMP;
    [SerializeField] private CanvasGroup canvasGroup;

    [Space(10)]
    [Header("DUNGEON LEVEL")]
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    [SerializeField] private int currentDungeonLevelListIndex = 0;
    private Room currentRoom;
    private Room PreviousRoom;
    private PlayerDetailSO playerDetail;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    long gameScore;
    int scoreMultiplier;
    InstantiateRoom bossRoom;

    protected override void Awake()
    {
        base.Awake();

        playerDetail = GameResources.Instance.currentPlayer.playerDetail;

        InstantiatePlayer();
    }

    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetail.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetail);
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplierArgs += StaticEventHandler_OnMultiplierArgs;

        player.destroyEvent.OnDestroyed += DestroyEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplierArgs -= StaticEventHandler_OnMultiplierArgs;

        player.destroyEvent.OnDestroyed += DestroyEvent_OnDestroyed;
    }


    private void DestroyEvent_OnDestroyed(DestroyEvent destroyEvent, DestroyEventArgs destroyEventArgs)
    {
        previousGameState = gameState;

        gameState = GameState.gameLost;

    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemyDefeated();
    }

    private void RoomEnemyDefeated()
    {
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiateRoom;
                continue;
            }

            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        if ((!isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;
            StartCoroutine(BossState());
        }

    }

    private IEnumerator BossState()
    {
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnLockDoors(0);

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f,1f,2f,new Color(0f,0f,0f,0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DOWN " + GameResources.Instance.currentPlayer.playerName + "! YOU'VE SURVIVED SO FAR\n" +
            "\nNOW FIND AND DEFEAT THE BOSS ........GOOD LUCK!", Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DOWN " + GameResources.Instance.currentPlayer.playerName + "! \n\nYOU'VE SURVIVED THIS DUNGEON LEVEL", Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null;

        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        GetPlayer().playerControl.DisablePlayer();

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("WELL DOWN " + GameResources.Instance.currentPlayer.playerName + "! \n\nYOU'VE DEFEATED THIS DUNGEON", Color.white, 3f));

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        GetPlayer().playerControl.DisablePlayer();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }

        yield return StartCoroutine(DisplayMessageRoutine("BADE LUCK " + GameResources.Instance.currentPlayer.playerName + "! YOU HAVE SUCCUMBED TO THE DUNGEON", Color.white, 2f));


        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    private void StaticEventHandler_OnMultiplierArgs(MultiplierArgs multiplierArgs)
    {
        if (multiplierArgs.multiplier)
        {
            scoreMultiplier++;
        }else
        {
            scoreMultiplier--;
        }

        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        StaticEventHandler.CallScoreChangeEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        gameScore += pointsScoredArgs.points * scoreMultiplier;

        StaticEventHandler.CallScoreChangeEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    // Start is called before the first frame update
    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;

        gameScore = 0;
        scoreMultiplier = 1;

        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
    }

    private IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSecounds, Color backGround)
    {
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backGround;

        float time = 0;

        while (time <= fadeSecounds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time/fadeSecounds);
            yield return null;
        }

    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    gameState = GameState.gameStarted;
        //}
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            //当处于游戏开始阶段
            case GameState.gameStarted:
                //开始第一阶段
                PlayDungeonLevel(currentDungeonLevelListIndex);
                
                gameState = GameState.playingLevel;

                RoomEnemyDefeated();

                break;

            case GameState.playingLevel:
                break;
            case GameState.engagingEnemies:
                break;
            case GameState.bossStage:
                break;
            case GameState.engagingBoss:
                break;
            case GameState.levelCompleted:

                StartCoroutine(LevelCompleted());

                break;
            case GameState.gameWon:

                if (previousGameState != GameState.gameWon)
                {
                    StartCoroutine(GameWon());
                }

                break;
            case GameState.gameLost:

                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines();
                    StartCoroutine(GameLost());
                }

                break;
            case GameState.gamePaused:
                break;
            case GameState.dungeonOverviewMap:
                break;
            case GameState.restartGame:

                RestartGame();

                break;
            default:
                break;
        }
    }

    public void SetCurrentRoom(Room room)
    {
        PreviousRoom = currentRoom;
        currentRoom = room;
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        bool dungeonBuildSuccessful = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuildSuccessful)
        {
            Debug.LogError("Couldn't build dungeon from specified room and node graph");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, 
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0);

        player.gameObject.transform.position = HelperUtility.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

        StartCoroutine(DisplayDungeonLevelText());

        //RoomEnemyDefeated();
    }

    private IEnumerator DisplayDungeonLevelText()
    {
        StartCoroutine(Fade(0f,1f,0f,Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList
            [currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        messageTextTMP.SetText(text);

        messageTextTMP.color = textColor;

        if (displaySeconds > 0f)
        {
            float time = displaySeconds;

            while (time > 0 && !Input.GetKeyDown(KeyCode.Return))
            {
                time -= Time.deltaTime;
                yield return null;  
            }
        }
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        messageTextTMP.SetText("");
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Sprite GetPlayerMinimapIcon()
    {
        return playerDetail.playerMiniMapIcon;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtility.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtility.ValidateCheckEnumerableValue(this, nameof(dungeonLevelList), dungeonLevelList);

    }
#endif
    #endregion
}
