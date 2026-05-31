using System.Collections.Generic;
using UnityEngine;

public sealed class KeyProgression : MonoBehaviour
{
    private static KeyProgression instance;

    [SerializeField]
    private int requiredRegularKeyCount = 3;

    private readonly HashSet<int> collectedRegularKeyIds = new();
    private bool hasFinalKey;

    public static KeyProgression Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject progressObject = new("KeyProgression");
                instance = progressObject.AddComponent<KeyProgression>();
            }

            return instance;
        }
    }

    public int CollectedRegularKeyCount => collectedRegularKeyIds.Count;
    public bool HasFinalKey => hasFinalKey;
    public bool HasAllRegularKeys => CollectedRegularKeyCount >= requiredRegularKeyCount;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsRegularKeyCollected(int keyId)
    {
        return collectedRegularKeyIds.Contains(keyId);
    }

    public bool IsKeyCollected(Key key)
    {
        if (key == null)
        {
            return false;
        }

        return key.Type switch
        {
            Key.KeyType.Regular => IsRegularKeyCollected(key.KeyId),
            Key.KeyType.Final => hasFinalKey,
            _ => false,
        };
    }

    public bool RegisterCollectedKey(Key key)
    {
        if (key == null)
        {
            return false;
        }

        return key.Type switch
        {
            Key.KeyType.Regular => collectedRegularKeyIds.Add(key.KeyId),
            Key.KeyType.Final => RegisterFinalKey(),
            _ => false,
        };
    }

    private bool RegisterFinalKey()
    {
        Debug.Log("RegisterFinalKey called");

        if (hasFinalKey)
            return false;

        hasFinalKey = true;

        Debug.Log("hasFinalKey = true");

        return true;
    }
}
