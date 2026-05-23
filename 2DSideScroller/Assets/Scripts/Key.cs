using UnityEngine;

public sealed class Key : MonoBehaviour
{
    public enum KeyType
    {
        Regular = 0,
        Final = 1
    }

    [SerializeField] private KeyType keyType = KeyType.Regular;
    [SerializeField] private int keyId = -1;

    public KeyType Type => keyType;
    public int KeyId => keyId;
    public bool IsPickedUp { get; private set; }

    private void Start()
    {
        if (KeyProgression.Instance.IsKeyCollected(this))
        {
            Destroy(gameObject);
        }
    }

    public bool TryPickUp()
    {
        if (IsPickedUp)
        {
            return false;
        }

        IsPickedUp = true;
        Destroy(gameObject);
        return true;
    }

    private void OnValidate()
    {
        if (keyType == KeyType.Regular && keyId < 0)
        {
            Debug.LogWarning($"{name}: Regular keys should use a keyId of 0, 1, or 2.", this);
        }
    }
}
