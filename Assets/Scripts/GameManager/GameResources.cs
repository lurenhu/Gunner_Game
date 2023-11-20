using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
            {
                //加载文件名为Resources文件内的内容
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    [Space(10)]
    [Header("DUNGEON")]
    [Tooltip("Populate with the dungeon roomnodetypelistSO")]
    public RoomNodeTypeListSO roomNodeTypeList;

    [Space(10)]
    [Header("PLAYER")]
    public CurrentPlayerSO currentPlayer;

    [Space(10)]
    [Header("SOUNDs")]
    public AudioMixerGroup soundsMasterMixetGroup;
    public SoundEffectSO doorOpenCloseSoundEffect;

    [Space(10)]
    [Header("MATERIALS")]
    public Material dimmedMaterial;

    public Material litMaterial;

    public Shader variableLitShader;

    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase PreferredEnemyPathTile;

    [Space(10)]
    [Header("UI")]
    public GameObject ammoIconPrefab;
    public GameObject heartPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this,nameof(roomNodeTypeList),roomNodeTypeList);
        HelperUtility.ValidateCheckNullValue(this, nameof(soundsMasterMixetGroup), soundsMasterMixetGroup);
        HelperUtility.ValidateCheckNullValue(this,nameof(currentPlayer),currentPlayer);
        HelperUtility.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtility.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtility.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtility.ValidateCheckEnumerableValue(this, nameof(enemyUnwalkableCollisionTilesArray),enemyUnwalkableCollisionTilesArray);
        HelperUtility.ValidateCheckNullValue(this, nameof(PreferredEnemyPathTile), PreferredEnemyPathTile);
        HelperUtility.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
    }
#endif
    #endregion
}
