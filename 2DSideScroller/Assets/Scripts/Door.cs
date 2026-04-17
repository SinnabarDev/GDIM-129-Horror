using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class Door : MonoBehaviour
{
    [SerializeField] private bool startsLocked = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Collider2D blockingCollider;

    public bool IsLocked { get; private set; }

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (blockingCollider == null)
        {
            blockingCollider = GetComponent<Collider2D>();
        }

        IsLocked = startsLocked;
        ApplyState();
    }

    public bool TryUnlock()
    {
        if (!IsLocked)
        {
            return false;
        }

        IsLocked = false;
        ApplyState();
        return true;
    }

    private void ApplyState()
    {
        if (spriteRenderer != null)
        {
            if (IsLocked && lockedSprite != null)
            {
                spriteRenderer.sprite = lockedSprite;
            }
            else if (!IsLocked && openedSprite != null)
            {
                spriteRenderer.sprite = openedSprite;
            }
        }

        if (blockingCollider != null)
        {
            blockingCollider.enabled = IsLocked;
        }
    }
}


