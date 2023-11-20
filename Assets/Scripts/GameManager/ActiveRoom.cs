using System.Collections.Generic;
using UnityEngine;

public class ActiveRoom : MonoBehaviour
{
    [SerializeField] Camera miniMapCamera;

    Camera cameraMain;

    private void Start()
    {
        cameraMain = Camera.main;

        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        HelperUtility.CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldPositionLowerBounds, out Vector2Int miniMapCameraWorldPositionUpperBounds, miniMapCamera);

        HelperUtility.CameraWorldPositionBounds(out Vector2Int mainMapCameraWorldPositionLowerBounds, out Vector2Int mainMapCameraWorldPositionUpperBounds, cameraMain);


        foreach (KeyValuePair<string,Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;


            if ((room.lowerBounds.x <= miniMapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= miniMapCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= miniMapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= miniMapCameraWorldPositionLowerBounds.y))
            {
                room.instantiateRoom.gameObject.SetActive(true);

                if ((room.lowerBounds.x <= mainMapCameraWorldPositionUpperBounds.x && room.lowerBounds.y <= mainMapCameraWorldPositionUpperBounds.y) &&
                (room.upperBounds.x >= mainMapCameraWorldPositionLowerBounds.x && room.upperBounds.y >= mainMapCameraWorldPositionLowerBounds.y))
                {
                    room.instantiateRoom.ActivateEnvironmentObject();
                }
                else
                {
                    room.instantiateRoom.DeactivateEnvironmentObject();
                }

            }
            else
            {
                room.instantiateRoom.gameObject.SetActive(false);
            }
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
    }
#endif
    #endregion

}
