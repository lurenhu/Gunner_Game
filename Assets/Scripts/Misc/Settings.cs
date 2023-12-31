using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Settings
{
    #region UNITS
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion

    #region DUNGEON BUILD SETTING
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region ROOM SETTINGS
    public const float fadeInTime = 0.5f;
    public const int maxChildCorridors = 3;
    public const float doorUnlockDelay = 1f;

    #endregion

    #region ANIMATOR PARAMETERS
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static float baseSpeedForPlayerAnimations = 8f;

    public static float baseSpeedForEnemyAnimation = 3f;

    public static int open = Animator.StringToHash("open");

    public static int destroy = Animator.StringToHash("destroy");
    public static String stateDestroy = "Destroyed";
    #endregion

    #region GAMEOBJECT TAG
    public static string playerTag = "Player";
    public static string playerWeapon = "playerWeapon";
    #endregion

    #region FIRING CONTROL
    public const float useAimAngleDistance = 3.5f;
    #endregion

    #region ASTAR FINDING PROGRAME
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    #endregion

    #region ENEMY PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI PARAMETERS
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;
    #endregion

    #region CONTACT DAMAGE PARAMETERS
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion
}
