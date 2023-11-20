
using System.Collections.Generic;
using UnityEngine;

public class SpawnText : MonoBehaviour
{
    List<SpawnableObjectByLevel<EnemyDetailSO>> testLevelSpawnList;
    RandownSpawnableObject<EnemyDetailSO> randomEnemyHelperclass;
    List<GameObject> instantiateEnemyList = new List<GameObject>();

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
        if (instantiateEnemyList != null && instantiateEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiateEnemyList)
            {
                Destroy(enemy);
            }
        }

        RoomTemplateSO roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.room.templateId);

        if (roomTemplate != null)
        {
            testLevelSpawnList = roomTemplate.enemyByLevelList;

            randomEnemyHelperclass = new RandownSpawnableObject<EnemyDetailSO>(testLevelSpawnList);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            EnemyDetailSO enemyDetail = randomEnemyHelperclass.GetItem();

            if (enemyDetail != null)
            {
                instantiateEnemyList.Add( Instantiate(enemyDetail.enemyPrefab, 
                    HelperUtility.GetSpawnPositionNearestToPlayer(HelperUtility.GetMouseWorldPosition()), Quaternion.identity));
            }
        }
    }
}
