using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Image healthFill;
    public Slider healthBar;

    [Header("Health Variables")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float smoothSpeed = 5f;

    [Tooltip("Starts pulsing when health is below this ratio (e.g., 0.3 = 30%)")]
    [Range(0f, 1f)]
    public float pulseThreshold = 0.5f;
    public float basePulseSpeed = 7f;

    private float targetFillProportion;
    private Vector3 originalScale;

    void Start()
    {
        GameController.OnPlayerHit += UpdateHealthUI;

        currentHealth = maxHealth;

        targetFillProportion = 1f;

        if (healthBar != null)
        {
            originalScale = healthBar.transform.localScale;
        }
    }

    void Update()
    {
        targetFillProportion = currentHealth / maxHealth;

        if (healthBar != null)
        {
            healthBar.value = Mathf.Lerp(
                healthBar.value,
                targetFillProportion,
                Time.deltaTime * smoothSpeed
            );
        }

        HandleNervousPulse();
    }

    private void HandleNervousPulse()
    {
        if (healthBar == null)
            return;

        if (targetFillProportion <= pulseThreshold && targetFillProportion > 0)
        {
            // Gets closer to 1 as health gets closer to 0.
            float intensity = 1f - (targetFillProportion / pulseThreshold);

            float currentPulseSpeed = basePulseSpeed + (intensity * 10f);

            float throb = Mathf.Sin(Time.time * currentPulseSpeed) * 0.05f * intensity;

            healthBar.transform.localScale = originalScale + new Vector3(throb, throb, 0f);
        }
        else
        {
            // If health is safe, smoothly return to normal size
            healthBar.transform.localScale = Vector3.Lerp(
                healthBar.transform.localScale,
                originalScale,
                Time.deltaTime * 5f
            );
        }
    }

    private void UpdateHealthUI(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Player Get Hit! Damage: {damage}, Current Health: {currentHealth}");
    }
}
