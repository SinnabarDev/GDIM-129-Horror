// Assets/Scripts/UI/FlashlightBatteryUI.cs
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightBatteryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FlashlightAim flashlightAim;

    [SerializeField]
    private Image batteryImage;

    [Header("Sprites (Index 0 = Empty, 5 = Full)")]
    [SerializeField]
    private Sprite[] batteryLevelSprites = new Sprite[6];

    private FieldInfo currentBatteryField;
    private int lastShownLevel = -1;

    private void Awake()
    {
        if (flashlightAim == null)
        {
            flashlightAim = FindObjectOfType<FlashlightAim>();
        }

        if (flashlightAim != null)
        {
            currentBatteryField = typeof(FlashlightAim).GetField(
                "currentBattery",
                BindingFlags.Instance | BindingFlags.NonPublic
            );
        }
    }

    private void Start()
    {
        RefreshBatterySprite(force: true);
    }

    private void Update()
    {
        RefreshBatterySprite(force: false);
    }

    private void RefreshBatterySprite(bool force)
    {
        if (flashlightAim == null || batteryImage == null || currentBatteryField == null)
        {
            return;
        }

        if (batteryLevelSprites == null || batteryLevelSprites.Length != 6)
        {
            Debug.LogWarning(
                "FlashlightBatteryUI needs exactly 6 sprites assigned (0 to 5).",
                this
            );
            return;
        }

        object value = currentBatteryField.GetValue(flashlightAim);
        if (value == null)
        {
            return;
        }

        float currentBattery = (float)value;
        int batteryLevel = Mathf.Clamp(Mathf.CeilToInt(currentBattery), 0, 5);

        if (!force && batteryLevel == lastShownLevel)
        {
            return;
        }

        Sprite targetSprite = batteryLevelSprites[batteryLevel];
        if (targetSprite == null)
        {
            Debug.LogWarning($"Missing battery sprite at index {batteryLevel}.", this);
            return;
        }

        batteryImage.sprite = targetSprite;
        lastShownLevel = batteryLevel;
    }
}
