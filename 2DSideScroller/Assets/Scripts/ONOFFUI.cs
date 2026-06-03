using TMPro;
using UnityEngine;

public class ONOFFUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FlashlightAim flashlightAim;

    [SerializeField]
    private TMP_Text onText;

    [SerializeField]
    private TMP_Text offText;

    [Header("Alpha Settings")]
    [SerializeField, Range(0f, 1f)]
    private float activeAlpha = 1f;

    [SerializeField, Range(0f, 1f)]
    private float inactiveAlpha = 0.2f;

    private bool lastState;

    private void Awake()
    {
        if (flashlightAim == null)
        {
            flashlightAim = FindObjectOfType<FlashlightAim>();
        }
    }

    private void Start()
    {
        UpdateStatus(true);
    }

    private void Update()
    {
        UpdateStatus(false);
    }

    private void UpdateStatus(bool force)
    {
        if (flashlightAim == null)
            return;

        bool isOn = flashlightAim.IsFlashlightOn;

        if (!force && isOn == lastState)
            return;

        SetAlpha(onText, isOn ? activeAlpha : inactiveAlpha);
        SetAlpha(offText, isOn ? inactiveAlpha : activeAlpha);

        lastState = isOn;
    }

    private void SetAlpha(TMP_Text text, float alpha)
    {
        if (text == null)
            return;

        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
