using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
        public Image healthFill;

    [Header("Health Variables")]
        public int maxHealth = 100;
        public int currentHealth;
        public float smoothSpeed = 5f;
        [Tooltip("Starts pulsing when health is below this ratio (e.g., 0.3 = 30%)")]
        [Range(0f, 1f)]
        public float pulseThreshold = 0.3f;
        public float basePulseSpeed = 5f;

        private float targetFillProportion;
        private Vector3 originalScale;

    void Start()
    {
        GameController.OnPlayerHit += UpdateHealthUI;

        currentHealth = maxHealth;

        targetFillProportion = 1f;

        if (healthFill != null)
        {
            originalScale = healthFill.transform.localScale;
        }
    }

    void Update() {
        targetFillProportion = currentHealth / maxHealth;

        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFillProportion, Time.deltaTime * smoothSpeed);

        HandleNervousPulse();
    }

    private void HandleNervousPulse()
    {
        if (healthFill == null) return;

        if (targetFillProportion <= pulseThreshold && targetFillProportion > 0)
        {
            // Gets closer to 1 as health gets closer to 0.
            float intensity = 1f - (targetFillProportion / pulseThreshold);

            float currentPulseSpeed = basePulseSpeed + (intensity * 10f);

            float throb = Mathf.Sin(Time.time * currentPulseSpeed) * 0.05f * intensity;

            healthFill.transform.localScale = originalScale + new Vector3(throb, throb, 0f);
        }
        else
        {
            // If health is safe, smoothly return to normal size
            healthFill.transform.localScale = Vector3.Lerp(healthFill.transform.localScale, originalScale, Time.deltaTime * 5f);
        }
    }

    private void UpdateHealthUI(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
