// Assets/Scripts/Player/PlayerItemCollector2D.cs
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPickup : MonoBehaviour
{
    [Header("Collection")]
    [SerializeField] private float pickupRange = 1.25f;
    [SerializeField] private LayerMask keyLayer;
    [SerializeField] private int goalKeyCount = 3;

    private readonly HashSet<int> collectedKeyIds = new();

    public int CollectedKeyCount => collectedKeyIds.Count;
    public bool HasCollectedAllKeys => CollectedKeyCount >= goalKeyCount;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryCollectNearestKey();
        }
    }

    private void TryCollectNearestKey()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRange, keyLayer);

        if (hits == null || hits.Length == 0)
        {
            return;
        }

        Collider2D nearestKey = null;
        float nearestDistanceSqr = float.MaxValue;
        Vector2 playerPosition = transform.position;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            int keyId = hit.gameObject.GetInstanceID();
            if (collectedKeyIds.Contains(keyId))
            {
                continue;
            }

            Vector2 closestPoint = hit.ClosestPoint(playerPosition);
            float distanceSqr = (closestPoint - playerPosition).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestKey = hit;
            }
        }

        if (nearestKey == null)
        {
            return;
        }

        CollectKey(nearestKey.gameObject);
    }

    private void CollectKey(GameObject keyObject)
    {
        int keyId = keyObject.GetInstanceID();

        if (!collectedKeyIds.Add(keyId))
        {
            return;
        }

        keyObject.SetActive(false);

        Debug.Log($"Collected key {CollectedKeyCount}/{goalKeyCount}");

        if (HasCollectedAllKeys)
        {
            Debug.Log("All distinct keys collected. Goal achieved.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}