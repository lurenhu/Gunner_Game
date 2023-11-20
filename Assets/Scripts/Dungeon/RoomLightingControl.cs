using System.Collections;
using UnityEngine.Tilemaps;
using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(InstantiateRoom))]
public class RoomLightingControl : MonoBehaviour
{
    private InstantiateRoom instantiateRoom;

    private void Awake()
    {
        instantiateRoom = GetComponent<InstantiateRoom>();
    }

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
        if (roomChangedEventArgs.room == instantiateRoom.room && !instantiateRoom.room.isLit)
        {
            FadeInRoomLighting();

            instantiateRoom.ActivateEnvironmentObject();

            FadeInEnvironmentLighting();

            FadeInDoor();

            instantiateRoom.room.isLit = true;
        }
    }

    private void FadeInEnvironmentLighting()
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        Environment[] environments = GetComponentsInChildren<Environment>();

        foreach (Environment environment in environments)
        {
            if (environment.spriteRenderer != null)
            {
                environment.spriteRenderer.material = material;
            }
        }

        StartCoroutine(FadeInEnvironmentLightingRoutine(material, environments));
    }

    private IEnumerator FadeInEnvironmentLightingRoutine(Material material, Environment[] environments)
    {
        for (float i = 0; i <= 1; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        foreach (Environment environment in environments)
        {
            if (environment.spriteRenderer != null)
            {
                environment.spriteRenderer.material = GameResources.Instance.litMaterial;
            }
        }
    }

    private void FadeInRoomLighting()
    {
        StartCoroutine(FadeInRoomLightingRoutine(instantiateRoom));
    }

    private IEnumerator FadeInRoomLightingRoutine(InstantiateRoom instantiateRoom)
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        instantiateRoom.groundTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.frontTilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = material;
        instantiateRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = material;

        for (float i = 0; i <= 1; i+= Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        instantiateRoom.groundTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.frontTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.decoration1Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.decoration2Tilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
        instantiateRoom.minimapTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
    }


    private void FadeInDoor()
    {
        Door[] doorArray = GetComponentsInChildren<Door>();

        foreach (Door door in doorArray)
        {
            DoorLightingControl doorLightingControl = door.GetComponentInChildren<DoorLightingControl>();

            doorLightingControl.FadeInDoor(door);
        }
    }
}
