using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemashineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;
    [SerializeField] private Transform cursors;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemashineTargetGroup();
    }

    private void SetCinemashineTargetGroup()
    {
        CinemachineTargetGroup.Target cinemashineGroupTarget_Player = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 2.5f,
            target = GameManager.Instance.GetPlayer().transform
        };

        CinemachineTargetGroup.Target cinemashineGroupTarget_Cursor = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 1f,
            target = cursors
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemashineGroupTarget_Player, cinemashineGroupTarget_Cursor };


        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update()
    {
        cursors.position = HelperUtility.GetMouseWorldPosition();
    }
}
