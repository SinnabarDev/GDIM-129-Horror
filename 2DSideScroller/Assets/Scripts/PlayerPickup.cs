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

    [Header("UI")]
    [SerializeField] private GameObject[] collectedKeyUi;
    [SerializeField] private GameObject doorUnlockedTextUi;

    private readonly HashSet<int> collectedKeyIds = new();

    public int CollectedKeyCount => collectedKeyIds.Count;
    public bool HasCollectedAllKeys => CollectedKeyCount >= goalKeyCount;

    private void Start()
    {
        HideAllKeyUi();
        HideDoorUnlockedText();
    }

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

            if (collectedKeyIds.Contains(key.KeyId))
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

        int keyId = nearestKey.KeyId;

        if (!nearestKey.TryPickUp())
        {
            return false;
        }

        if (!collectedKeyIds.Add(keyId))
        {
            return false;
        }

        ShowCollectedKeyUi(keyId);

        if (HasCollectedAllKeys)
        {
            ShowDoorUnlockedText();
            Debug.Log("All 3 keys collected. The door can now be unlocked.");
        }

        Debug.Log($"Collected key ID {keyId}. Progress: {CollectedKeyCount}/{goalKeyCount}");
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

    private void ShowCollectedKeyUi(int keyId)
    {
        if (collectedKeyUi == null)
        {
            return;
        }

        if (keyId < 0 || keyId >= collectedKeyUi.Length)
        {
            Debug.LogWarning($"No UI slot exists for keyId {keyId}.");
            return;
        }

        GameObject keyUi = collectedKeyUi[keyId];
        if (keyUi != null)
        {
            keyUi.SetActive(true);
        }
    }

    private void HideAllKeyUi()
    {
        if (collectedKeyUi == null)
        {
            return;
        }

        foreach (GameObject uiObject in collectedKeyUi)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(false);
            }
        }
    }

    private void ShowDoorUnlockedText()
    {
        if (doorUnlockedTextUi != null)
        {
            doorUnlockedTextUi.SetActive(true);
        }
    }

    private void HideDoorUnlockedText()
    {
        if (doorUnlockedTextUi != null)
        {
            doorUnlockedTextUi.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, keyPickupRange);
        Gizmos.DrawWireSphere(transform.position, doorInteractRange);
    }
}