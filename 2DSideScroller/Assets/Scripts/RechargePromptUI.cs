using TMPro;
using UnityEngine;

public class RechargePromptUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FlashlightAim flashlightAim;

    [SerializeField]
    private RectTransform promptTransform;

    [SerializeField]
    private TMP_Text promptText;

    [Header("Pulse")]
    [SerializeField]
    private float pulseScale = 1.25f;

    [SerializeField]
    private float pulseSpeed = 10f;

    private Vector3 originalScale;
    private float pulseTimer;

    private void Awake()
    {
        if (flashlightAim == null)
        {
            flashlightAim = FindObjectOfType<FlashlightAim>();
        }

        if (promptTransform == null)
        {
            promptTransform = transform as RectTransform;
        }

        originalScale = promptTransform.localScale;
    }

    private void Update()
    {
        if (flashlightAim == null || promptText == null)
            return;

        bool showPrompt = flashlightAim.IsBatteryDrained;

        // Show/Hide only the text
        promptText.gameObject.SetActive(showPrompt);

        if (!showPrompt)
        {
            promptTransform.localScale = originalScale;
            return;
        }

        UpdatePulse();
    }

    private void UpdatePulse()
    {
        pulseTimer += Time.deltaTime * pulseSpeed;

        float scale = 1f + (Mathf.Sin(pulseTimer) * 0.5f + 0.5f) * (pulseScale - 1f);

        promptTransform.localScale = originalScale * scale;
    }
}
