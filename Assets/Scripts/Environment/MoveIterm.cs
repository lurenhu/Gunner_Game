using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(BoxCollider2D))]
public class MoveIterm : MonoBehaviour
{
    [SerializeField] private SoundEffectSO moveSoundEffect;
    [HideInInspector] public BoxCollider2D boxCollider2D;
    Rigidbody2D rb;
    InstantiateRoom instantiateRoom;
    Vector3 previousPosition;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        instantiateRoom = GetComponentInParent<InstantiateRoom>();

        instantiateRoom.moveableItermsList.Add(this);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        UpdateObstacles();
    }

    private void UpdateObstacles()
    {
        ConfineItemToRoomBounds();

        instantiateRoom.UpdateMoveableObstacles();

        previousPosition = transform.position;

        if (Mathf.Abs(rb.velocity.x) > 0.001f || Mathf.Abs(rb.velocity.y) > 0.001f)
        {
            if (moveSoundEffect != null && Time.frameCount % 10 == 0)
            {
                SoundEffectManager.Instance.PlaySoundEffect(moveSoundEffect);
            }
        }
    }

    private void ConfineItemToRoomBounds()
    {
        Bounds itemBounds = boxCollider2D.bounds;
        Bounds roomBounds = instantiateRoom.RoomColliderBounds;

        if (itemBounds.min.x <= roomBounds.min.x ||
            itemBounds.max.x <= roomBounds.max.x ||
            roomBounds.min.y <= roomBounds.min.y ||
            roomBounds.max.y <= roomBounds.max.y)
        {
            transform.position = previousPosition;
        }
    }
}
