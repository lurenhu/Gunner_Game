using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperUtility
{
    public static Camera mainCamera;

    /// <summary>
    /// 获取鼠标的世界坐标，其中包含将鼠标从屏幕坐标投射到世界坐标中
    /// </summary>
    public static Vector3 GetMouseWorldPosition()
    {
        if(mainCamera == null) mainCamera = Camera.main;

        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0, Screen.height);

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }

    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds, out Vector2Int cameraWorldPositionUpperBounds, Camera camera)
    {
        Vector3 worldPositionViewPortBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 worldPositionViewPortTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewPortBottomLeft.x, (int)worldPositionViewPortBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewPortTopRight.x, (int)worldPositionViewPortTopRight.y);
    }

    /// <summary>
    /// 将向量转换为以x轴为起点的角度值
    /// </summary>
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);

        float degree = radians * Mathf.Rad2Deg;

        return degree;
    }

    /// <summary>
    /// 通过所给角度，获取单位方向向量
    /// </summary>
    public static Vector3 GetDirectionVectorFromAngle(float angle)
    {
        Vector3 directionVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
        return directionVector;
    }

    /// <summary>
    /// 根据角度赋值AimDirection，给定朝向
    /// </summary>
    public static AimDirection GetAimDirection(float angleDegree)
    {
        AimDirection aimDirection;

        if (angleDegree >= 22f && angleDegree <= 67f)
        {
            aimDirection = AimDirection.UpRight;
        }else if (angleDegree > 67f && angleDegree <= 112f)
        {
            aimDirection = AimDirection.Up;
        }else if (angleDegree > 112f && angleDegree <= 158f)
        {
            aimDirection = AimDirection.UpLeft;
        }else if ((angleDegree > 158f && angleDegree <= 180f) || (angleDegree > -180f && angleDegree <= -135f))
        {
            aimDirection = AimDirection.Left;
        }else if ((angleDegree > -45f && angleDegree <= 0f) || (angleDegree > 0 && angleDegree < 22f))
        {
            aimDirection = AimDirection.Right;
        }else if (angleDegree > -135f && angleDegree <= -45f)
        {
            aimDirection = AimDirection.Down;
        }else
        {
            aimDirection= AimDirection.Right;
        }

        return aimDirection;
    }

    public static float LinearToDecibels(int linear)
    {
        float linearScaleRange = 20f;
        return Mathf.Log10((float)linear / linearScaleRange) * 20f;
    }

    public static bool ValidateCheckEmptyString(Object thisObject , string fileName , string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fileName = "is empty and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckNullValue(Object thisObject, string fileName, UnityEngine.Object ObjectToCheck)
    {
        if (ObjectToCheck == null)
        {
            Debug.Log(fileName + " is null and must contain a value in object" + thisObject.name.ToString());
            return true;
        }
        return false ;
    }

    public static bool ValidateCheckEnumerableValue(Object thisObject , string fileName , IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fileName + " is null in object " + thisObject.name.ToString());
            return true;
        }

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fileName + " has null value in object " + thisObject.name.ToString());
                error = true;
            }else
            {
                count++;    
            }
        }

        if (count == 0)
        {
            Debug.Log(fileName + " has no value in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    public static bool ValidateCheckPostiveValue(Object thisObject, string fileName, int ValueToChek, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (ValueToChek < 0)
            {
                Debug.Log(fileName + " must contain a postive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (ValueToChek <= 0)
            {
                Debug.Log(fileName + " must contain a postive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    public static bool ValidateCheckPostiveValue(Object thisObject, string fileName, float ValueToChek, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (ValueToChek < 0)
            {
                Debug.Log(fileName + " must contain a postive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (ValueToChek <= 0)
            {
                Debug.Log(fileName + " must contain a postive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    public static bool ValidateChackPositveRange(Object thisObject, string fileNameMinimum, float valueToCheckMinimum,
        string fileNameMaximum, float valueToCheckMaximum, bool isZeroAllowed)
    {
        bool error = false;
        if (valueToCheckMinimum > valueToCheckMaximum)
        {
            Debug.Log(fileNameMinimum + " must be less than or equal to " + fileNameMaximum + " in object " + thisObject.ToString());
            error = true;
        }

        if (ValidateCheckPostiveValue(thisObject, fileNameMaximum, valueToCheckMaximum, isZeroAllowed)) error = true;
        if (ValidateCheckPostiveValue(thisObject, fileNameMinimum, valueToCheckMinimum, isZeroAllowed)) error = true;

        return error;
    }

    public static Vector3 GetSpawnPositionNearestToPlayer(Vector3 playerPosition)
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiateRoom.grid;

        Vector3 NearestSpawnPosition = new Vector3(1000f, 1000f, 0);

        foreach (Vector2Int spwanPositionGrid in currentRoom.spawnPositonArray)
        {
            Vector3 spawnPositionWorld = grid.CellToWorld((Vector3Int)spwanPositionGrid);

            if (Vector3.Distance(spawnPositionWorld, playerPosition) < Vector3.Distance(NearestSpawnPosition,playerPosition))
            {
                NearestSpawnPosition = spawnPositionWorld;
            }
        }
        return NearestSpawnPosition;
    }
}
