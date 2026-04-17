using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPickup : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] private float keyPickupRange = 1.25f;
    [SerializeField] private LayerMask keyLayer;
    [SerializeField] private int goalKeyCount = 3;

    [Header("Doors")]
    [SerializeField] private float doorInteractRange = 1.5f;
    [SerializeField] private LayerMask doorLayer;

    private readonly HashSet<int> collectedKeyIds = new();

    public int CollectedKeyCount => collectedKeyIds.Count;
    public bool HasCollectedAllKeys => CollectedKeyCount >= goalKeyCount;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }
    }

    private void HandleLeftClick()
    {
        if (TryCollectNearestKey())
        {
            return;
        }

        if (HasCollectedAllKeys)
        {
            TryUnlockNearestDoor();
        }
    }

    private bool TryCollectNearestKey()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, keyPickupRange, keyLayer);

        Key nearestKey = null;
        float nearestDistanceSqr = float.MaxValue;
        Vector2 playerPosition = transform.position;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            Key key = hit.GetComponent<Key>();
            if (key == null || key.IsPickedUp)
            {
                continue;
            }

            int keyObjectId = key.gameObject.GetInstanceID();
            if (collectedKeyIds.Contains(keyObjectId))
            {
                continue;
            }

            Vector2 closestPoint = hit.ClosestPoint(playerPosition);
            float distanceSqr = (closestPoint - playerPosition).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestKey = key;
            }
        }

        if (nearestKey == null)
        {
            return false;
        }

        int collectedId = nearestKey.gameObject.GetInstanceID();

        if (!nearestKey.TryPickUp())
        {
            return false;
        }

        collectedKeyIds.Add(collectedId);

        Debug.Log($"Collected key {CollectedKeyCount}/{goalKeyCount}");

        if (HasCollectedAllKeys)
        {
            Debug.Log("All 3 keys collected. The door can now be unlocked.");
        }

        return true;
    }

    private bool TryUnlockNearestDoor()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, doorInteractRange, doorLayer);

        Door nearestDoor = null;
        float nearestDistanceSqr = float.MaxValue;
        Vector2 playerPosition = transform.position;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            Door door = hit.GetComponent<Door>();
            if (door == null || !door.IsLocked)
            {
                continue;
            }

            Vector2 closestPoint = hit.ClosestPoint(playerPosition);
            float distanceSqr = (closestPoint - playerPosition).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestDoor = door;
            }
        }

        if (nearestDoor == null)
        {
            return false;
        }

        bool unlocked = nearestDoor.TryUnlock();

        if (unlocked)
        {
            Debug.Log("Door unlocked.");
        }

        return unlocked;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, keyPickupRange);
        Gizmos.DrawWireSphere(transform.position, doorInteractRange);
    }
}