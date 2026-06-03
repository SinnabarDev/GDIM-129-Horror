using UnityEngine;
using UnityEngine.UI;

public class FlashlightBatteryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private FlashlightAim flashlightAim;

    [SerializeField]
    private Image batteryImage;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] batteryLevelSprites = new Sprite[6];

    private int lastShownLevel = -1;

    private void Awake()
    {
        if (flashlightAim == null)
        {
            flashlightAim = FindObjectOfType<FlashlightAim>();
        }
    }

    private void Start()
    {
        RefreshBatterySprite(true);
    }

    private void Update()
    {
        RefreshBatterySprite(false);
    }

    private void RefreshBatterySprite(bool force)
    {
        if (flashlightAim == null)
            return;

        if (batteryImage == null)
            return;

        if (batteryLevelSprites == null || batteryLevelSprites.Length != 6)
            return;

        float batteryValue = flashlightAim.DisplayBattery;

        float percent = batteryValue / 5f;

        int batteryLevel = Mathf.Clamp(Mathf.FloorToInt(percent * 6f), 0, 5);

        if (!force && batteryLevel == lastShownLevel)
            return;

        batteryImage.sprite = batteryLevelSprites[batteryLevel];
        lastShownLevel = batteryLevel;
    }
}
