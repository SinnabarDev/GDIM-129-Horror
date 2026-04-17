using UnityEngine;

public sealed class Key : MonoBehaviour
{
    [SerializeField] private int keyId;

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