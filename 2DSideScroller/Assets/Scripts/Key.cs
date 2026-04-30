// Assets/Scripts/Items/Key.cs
using UnityEngine;

public sealed class Key : MonoBehaviour
{
    public enum KeyType
    {
        Regular = 0,
        Final = 1,
    }

    [SerializeField]
    private KeyType keyType = KeyType.Regular;

    [SerializeField]
    private int keyId = -1;

    public KeyType Type => keyType;
    public int KeyId => keyId;
    public bool IsPickedUp { get; private set; }

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
}
