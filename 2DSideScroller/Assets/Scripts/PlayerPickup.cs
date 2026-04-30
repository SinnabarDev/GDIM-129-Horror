using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPickup : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField]
    private float keyPickupRange = 1.25f;

    [SerializeField]
    private LayerMask keyLayer;

    [SerializeField]
    private int goalKeyCount = 3;

    [Header("Doors")]
    [SerializeField]
    private float doorInteractRange = 1.5f;

    [SerializeField]
    private LayerMask doorLayer;

    [Header("Collection UI")]
    [SerializeField]
    private GameObject[] collectedKeyUi;

    [SerializeField]
    private GameObject doorUnlockedTextUi;

    [Header("Interaction Prompts")]
    [SerializeField]
    private GameObject keyInteraction;

    [SerializeField]
    private GameObject doorInteraction;

    [SerializeField]
    private GameObject doorNotUnlocked;

    private readonly HashSet<int> collectedRegularKeyIds = new();

    private Key nearbyKey;
    private Door nearbyDoor;
    private bool hasFinalKey;

    public int CollectedKeyCount => collectedRegularKeyIds.Count;
    public bool HasCollectedAllKeys => CollectedKeyCount >= goalKeyCount;
    public bool HasFinalKey => hasFinalKey;

    private void Start()
    {
        HideAllKeyUi();
        SetActiveIfAssigned(doorUnlockedTextUi, false);
        SetActiveIfAssigned(keyInteraction, false);
        SetActiveIfAssigned(doorInteraction, false);
        SetActiveIfAssigned(doorNotUnlocked, false);
    }

    private void Update()
    {
        RefreshNearbyInteractables();
        UpdateInteractionPrompts();

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteractInput();
        }
    }

    private void HandleInteractInput()
    {
        if (nearbyKey != null)
        {
            if (TryCollectKey(nearbyKey))
            {
                RefreshNearbyInteractables();
                UpdateInteractionPrompts();
            }

            return;
        }

        if (nearbyDoor == null)
        {
            return;
        }

        if (!CanUnlockDoor(nearbyDoor))
        {
            SetActiveIfAssigned(doorNotUnlocked, true);
            return;
        }

        if (nearbyDoor.TryUnlock())
        {
            SetActiveIfAssigned(doorNotUnlocked, false);
            RefreshNearbyInteractables();
            UpdateInteractionPrompts();
            Debug.Log("Door unlocked.");
        }
    }

    private void RefreshNearbyInteractables()
    {
        nearbyKey = FindNearestKeyInRange();
        nearbyDoor = FindNearestLockedDoorInRange();
    }

    private void UpdateInteractionPrompts()
    {
        SetActiveIfAssigned(keyInteraction, nearbyKey != null);
        SetActiveIfAssigned(doorInteraction, nearbyDoor != null);

        if (nearbyDoor == null || CanUnlockDoor(nearbyDoor))
        {
            SetActiveIfAssigned(doorNotUnlocked, false);
        }
    }

    private Key FindNearestKeyInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            keyPickupRange,
            keyLayer
        );

        Key nearest = null;
        float nearestDistanceSqr = float.MaxValue;
        Vector2 playerPosition = transform.position;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            Key key = hit.GetComponent<Key>();
            if (key == null || key.IsPickedUp || IsKeyAlreadyCollected(key))
            {
                continue;
            }

            Vector2 closestPoint = hit.ClosestPoint(playerPosition);
            float distanceSqr = (closestPoint - playerPosition).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearest = key;
            }
        }

        return nearest;
    }

    private Door FindNearestLockedDoorInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            doorInteractRange,
            doorLayer
        );

        Door nearest = null;
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
                nearest = door;
            }
        }

        return nearest;
    }

    private bool TryCollectKey(Key key)
    {
        if (IsKeyAlreadyCollected(key))
        {
            return false;
        }

        Key.KeyType keyType = key.Type;
        int keyId = key.KeyId;

        if (!key.TryPickUp())
        {
            return false;
        }

        switch (keyType)
        {
            case Key.KeyType.Regular:
                if (!collectedRegularKeyIds.Add(keyId))
                {
                    return false;
                }

                ShowCollectedKeyUi(keyId);

                if (HasCollectedAllKeys)
                {
                    SetActiveIfAssigned(doorUnlockedTextUi, true);
                    SetActiveIfAssigned(doorNotUnlocked, false);
                    Debug.Log("All 3 regular keys collected. The first door can now be unlocked.");
                }

                Debug.Log(
                    $"Collected regular key ID {keyId}. Progress: {CollectedKeyCount}/{goalKeyCount}"
                );
                return true;

            case Key.KeyType.Final:
                hasFinalKey = true;
                SetActiveIfAssigned(doorNotUnlocked, false);
                Debug.Log("Collected the final key. The final door can now be unlocked.");
                return true;

            default:
                return false;
        }
    }

    private bool IsKeyAlreadyCollected(Key key)
    {
        return key.Type switch
        {
            Key.KeyType.Regular => collectedRegularKeyIds.Contains(key.KeyId),
            Key.KeyType.Final => hasFinalKey,
            _ => false,
        };
    }

    private bool CanUnlockDoor(Door door)
    {
        return door.Requirement switch
        {
            Door.UnlockRequirement.ThreeRegularKeys => HasCollectedAllKeys,
            Door.UnlockRequirement.FinalKey => HasFinalKey,
            _ => false,
        };
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

    private static void SetActiveIfAssigned(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, keyPickupRange);
        Gizmos.DrawWireSphere(transform.position, doorInteractRange);
    }
}
