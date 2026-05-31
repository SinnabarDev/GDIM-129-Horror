using UnityEngine;

public sealed class PlayerPickup : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField]
    private float keyPickupRange = 1.25f;

    [SerializeField]
    private LayerMask keyLayer;

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

    private Key nearbyKey;
    private Door nearbyDoor;

    private void Start()
    {
        HideAllKeyUi();
        SetActiveIfAssigned(doorUnlockedTextUi, false);
        SetActiveIfAssigned(keyInteraction, false);
        SetActiveIfAssigned(doorInteraction, false);
        SetActiveIfAssigned(doorNotUnlocked, false);
        RefreshProgressUi();
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

            Key key = hit.GetComponent<Key>() ?? hit.GetComponentInParent<Key>();
            if (key == null || key.IsPickedUp || KeyProgression.Instance.IsKeyCollected(key))
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

            Door door = hit.GetComponent<Door>() ?? hit.GetComponentInParent<Door>();
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
        if (KeyProgression.Instance.IsKeyCollected(key))
        {
            return false;
        }

        if (!key.TryPickUp())
        {
            return false;
        }

        if (!KeyProgression.Instance.RegisterCollectedKey(key))
        {
            return false;
        }

        if (key.Type == Key.KeyType.Regular)
        {
            ShowCollectedKeyUi(key.KeyId);

            if (KeyProgression.Instance.HasAllRegularKeys)
            {
                SetActiveIfAssigned(doorUnlockedTextUi, true);
                SetActiveIfAssigned(doorNotUnlocked, false);
                Debug.Log("All 3 regular keys collected. The first door can now be unlocked.");
            }

            Debug.Log(
                $"Collected regular key ID {key.KeyId}. Progress: {KeyProgression.Instance.CollectedRegularKeyCount}/3"
            );
            return true;
        }

        if (key.Type == Key.KeyType.Final)
        {
            SetActiveIfAssigned(doorNotUnlocked, false);
            Debug.Log("Collected the final key. The final door can now be unlocked.");
            return true;
        }

        return false;
    }

    private bool CanUnlockDoor(Door door)
    {
        return door.Requirement switch
        {
            Door.UnlockRequirement.ThreeRegularKeys => KeyProgression.Instance.HasAllRegularKeys,
            Door.UnlockRequirement.FinalKey => KeyProgression.Instance.HasFinalKey,
            _ => false,
        };
    }

    private void RefreshProgressUi()
    {
        for (int i = 0; i < collectedKeyUi.Length; i++)
        {
            SetActiveIfAssigned(
                collectedKeyUi[i],
                KeyProgression.Instance.IsRegularKeyCollected(i)
            );
        }

        SetActiveIfAssigned(doorUnlockedTextUi, KeyProgression.Instance.HasAllRegularKeys);
    }

    private void ShowCollectedKeyUi(int keyId)
    {
        if (keyId < 0 || keyId >= collectedKeyUi.Length)
        {
            Debug.LogWarning($"No UI slot exists for keyId {keyId}.", this);
            return;
        }

        SetActiveIfAssigned(collectedKeyUi[keyId], true);
    }

    private void HideAllKeyUi()
    {
        foreach (GameObject uiObject in collectedKeyUi)
        {
            SetActiveIfAssigned(uiObject, false);
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
