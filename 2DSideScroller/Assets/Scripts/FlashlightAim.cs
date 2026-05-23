using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightAim : MonoBehaviour
{
    [Header("Mash Feedback")]
    [SerializeField]
    private SpriteRenderer flashlightSprite;

    [SerializeField]
    private float pulseScale = 1.2f;

    [SerializeField]
    private float pulseSpeed = 12f;

    private Vector3 originalScale;
    private float pulseAmount = 0f;

    [SerializeField]
    private Light2D flashbulbvol;

    [SerializeField]
    private Light2D flashbulb;

    public bool isFacingRight = true;

    private List<IExorcisable> targetsInLight = new List<IExorcisable>();

    [Header("Battery")]
    [SerializeField]
    private float maxBattery = 5f;

    [SerializeField]
    private float batteryDrainRate = 1f;

    [Header("Mash Recovery")]
    [SerializeField]
    private float mashTimeRequired = 4f;

    [SerializeField]
    private float cooldownTime = 2f;

    [SerializeField]
    private int mashRequiredCount = 20;

    private bool isDrained = false;
    private bool isRecovering = false;
    private bool recoveryStarted = false;

    private float mashTimer = 0f;
    private int mashCount = 0;
    private float cooldownTimer = 0f;

    private float currentBattery;
    private bool isFlashlightOn = true;

    private PolygonCollider2D detectray;

    public enum BeamMode
    {
        Wide,
        Focused,
    }

    private BeamMode currentMode = BeamMode.Wide;

    void Awake()
    {
        detectray = GetComponent<PolygonCollider2D>();
    }

    void Start()
    {
        currentBattery = maxBattery;
        originalScale = flashlightSprite.transform.localScale;
    }

    void Update()
    {
        AimFlashlight();
        BatterySystem();

        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlashlightOn = !isFlashlightOn;
            UpdateFlashlightState();
        }

        if (Input.GetMouseButtonDown(1) && isFlashlightOn)
        {
            if (!isDrained && !isRecovering)
            {
                currentMode = currentMode == BeamMode.Wide ? BeamMode.Focused : BeamMode.Wide;
            }
        }

        UpdateBeamVisuals();
        ApplyEffectsToEnemies();
        RechargeMash();
        UpdatePulse();
    }

    public void SetFacing(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    void AimFlashlight()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        if (isFacingRight && direction.x < 0)
            direction = Vector2.right;

        if (!isFacingRight && direction.x > 0)
            direction = Vector2.left;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // =========================
    // COLLISION → INTERFACE
    // =========================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IExorcisable target = collision.GetComponent<IExorcisable>();

        if (target != null && !targetsInLight.Contains(target))
        {
            targetsInLight.Add(target);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IExorcisable target = collision.GetComponent<IExorcisable>();

        if (target != null)
        {
            targetsInLight.Remove(target);
            target.ClearLightEffects();
        }
    }

    // =========================
    // BEAM VISUALS
    // =========================
    void UpdateBeamVisuals()
    {
        float targetOuter = currentMode == BeamMode.Focused ? 25f : 45f;
        float targetInner = currentMode == BeamMode.Focused ? 10f : 25f;

        flashbulb.pointLightOuterAngle = Mathf.Lerp(
            flashbulb.pointLightOuterAngle,
            targetOuter,
            Time.deltaTime * 10f
        );

        flashbulb.pointLightInnerAngle = Mathf.Lerp(
            flashbulb.pointLightInnerAngle,
            targetInner,
            Time.deltaTime * 3f
        );

        flashbulb.intensity = Mathf.Lerp(
            flashbulb.intensity,
            currentMode == BeamMode.Focused ? 1.25f : 1f,
            Time.deltaTime * 0.1f
        );
    }

    // =========================
    // EFFECTS (INTERFACE BASED)
    // =========================
    void ApplyEffectsToEnemies()
    {
        foreach (IExorcisable target in targetsInLight)
        {
            if (target == null)
                continue;

            switch (currentMode)
            {
                case BeamMode.Wide:
                    target.ApplySlow(0.5f);
                    break;

                case BeamMode.Focused:
                    target.ApplyStun(0.7f);
                    break;
            }
        }

        targetsInLight.RemoveAll(t => t == null);
    }

    // =========================
    // BATTERY SYSTEM
    // =========================
    void BatterySystem()
    {
        if (currentMode == BeamMode.Focused && !isDrained)
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;

            if (currentBattery <= 0)
            {
                flashbulb.enabled = false;
                currentBattery = 0;
                isDrained = true;
                currentMode = BeamMode.Wide;

                UpdateBeamVisuals();
                UpdateFlashlightState();

                Debug.Log("Battery Drained!");
            }
        }
    }

    // =========================
    // MASH RECOVERY
    // =========================
    void RechargeMash()
    {
        if (!isDrained)
            return;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return;
        }

        if (!recoveryStarted)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                recoveryStarted = true;
                isRecovering = true;
                mashTimer = 0f;
                mashCount = 0;
            }
            return;
        }

        mashTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            mashCount++;

            // trigger pulse
            pulseAmount = 1f;
        }

        if (mashTimer < mashTimeRequired)
            return;

        float percent = Mathf.Clamp01((float)mashCount / mashRequiredCount);

        RecoverBattery(percent);

        recoveryStarted = false;
    }

    void RecoverBattery(float percent)
    {
        currentBattery = maxBattery * percent;

        Debug.Log($"Recovered {percent * 100f:F0}% battery");

        isRecovering = false;

        if (currentBattery > 0)
        {
            isDrained = false;
            UpdateFlashlightState();
        }
        else
        {
            FailRecovery();
        }
    }

    void FailRecovery()
    {
        Debug.Log("Recovery Failed - Cooldown!");

        isRecovering = false;
        recoveryStarted = false;
        cooldownTimer = cooldownTime;
    }

    // =========================
    // TOGGLE LIGHT
    // =========================
    void UpdateFlashlightState()
    {
        bool active = isFlashlightOn && !isDrained;

        flashbulb.enabled = active;
        flashbulbvol.enabled = active;
        detectray.enabled = active;

        if (!active)
        {
            foreach (IExorcisable target in targetsInLight)
            {
                if (target != null)
                    target.ClearLightEffects();
            }

            targetsInLight.Clear();
        }
    }

    void UpdatePulse()
    {
        if (flashlightSprite == null)
            return;

        if (pulseAmount > 0f)
        {
            pulseAmount -= Time.deltaTime * pulseSpeed;

            float scale = Mathf.Lerp(1f, pulseScale, Mathf.Sin(pulseAmount * Mathf.PI));

            flashlightSprite.transform.localScale = originalScale * scale;
        }
        else
        {
            flashlightSprite.transform.localScale = originalScale;
        }
    }
}
