using UnityEngine;

public class DamageBox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField]
    private int damage = 1;

    [Header("Knockback")]
    [SerializeField]
    private float horizontalKnockback = 14f;

    [SerializeField]
    private float verticalKnockback = 3f;

    [SerializeField]
    private float knockbackDuration = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player Being Hit!");

        GameController.Instance.UpdatePlayerGetHit(damage);

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            float direction = other.transform.position.x > transform.position.x ? 1f : -1f;

            Vector2 knockbackForce = new Vector2(
                direction * horizontalKnockback,
                verticalKnockback
            );

            playerMovement.ApplyKnockback(knockbackForce, knockbackDuration);
        }
    }
}
