using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private GameObject hitBox;

    [SerializeField]
    private Transform hitBoxTransform;

    private float originalX;
    private int facingDir = 1;

    private void Awake()
    {
        if (hitBoxTransform != null)
        {
            originalX = hitBoxTransform.localPosition.x;
        }
    }

    private void Start()
    {
        UpdateHitboxPosition();
        hitBox.SetActive(false);
    }

    public void SetFacingDirection(int direction)
    {
        facingDir = direction > 0 ? 1 : -1;
        UpdateHitboxPosition();
    }

    private void UpdateHitboxPosition()
    {
        if (hitBoxTransform == null)
            return;

        Vector3 pos = hitBoxTransform.localPosition;
        pos.x = Mathf.Abs(originalX) * facingDir;
        hitBoxTransform.localPosition = pos;
    }

    public void EnableHitBox()
    {
        hitBox.SetActive(true);
    }

    public void DisableHitBox()
    {
        hitBox.SetActive(false);
    }
}
