using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightAim : MonoBehaviour
{
    [SerializeField]
    private Light2D flashbulbvol; //this is the spot light source for flashlight

    [SerializeField]
    private Light2D flashbulb; //this is the spot light source for flashlight
    public bool isFacingRight = true; //used as to define conditions for blocking backwards aiming

    private List<Enemy> enemiesInLight = new List<Enemy>();

    [Header("Battery")]
    [SerializeField]
    private float maxBattery = 5f; //battery size

    [SerializeField]
    private float batteryDrainRate = 1f; // 1 second = 1 battery

    [Header("Mash Recovery")]
    [SerializeField]
    private float mashTimeRequired = 4f; //time window to mash

    [SerializeField]
    private float cooldownTime = 2f; //time before you can try again

    [SerializeField]
    private int mashRequiredCount = 20; //number of mashers for successful recovery

    private bool isDrained = false; //empty battery state
    private bool isRecovering = false; //currently in recovery state
    private bool recoveryStarted = false; //started recovery process (first mash)?
    private float mashTimer = 0f; //time since recovery started
    private int mashCount = 0; //number of mashes counted
    private float cooldownTimer = 0f; //time for cooldown

    private float currentBattery; //current battery level
    private bool isFlashlightOn = true; //toggle flashlight on/off
    private PolygonCollider2D detectray;

    public enum BeamMode
    {
        Wide, // normal state to slow
        Focused, // expending state to stun
    }

    private BeamMode currentMode = BeamMode.Wide;

    void Awake()
    {
        detectray = GetComponent<PolygonCollider2D>();
    }

    void Start()
    {
        currentBattery = maxBattery; //intialize battery at full
    }

    void Update()
    {
        AimFlashlight();
        BatterySystem();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isFlashlightOn = !isFlashlightOn;
            UpdateFlashlightState();
        }
        if (Input.GetMouseButtonDown(1) && isFlashlightOn) // toggle on click to focus beam
        {
            if (!isDrained && !isRecovering)
            {
                currentMode = currentMode == BeamMode.Wide ? BeamMode.Focused : BeamMode.Wide;
            }
        }
        UpdateBeamVisuals();
        ApplyEffectsToEnemies();
        RechargeMash();
    }

    public void SetFacing(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    void AimFlashlight()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 direction = mousePos - transform.position;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        //angle = Mathf.Clamp(angle, -180f, 0f);

        //transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector2 direction = (mousePos - transform.position).normalized;

        // Block backwards aiming
        if (isFacingRight && direction.x < 0)
            direction = Vector2.right;

        if (!isFacingRight && direction.x > 0)
            direction = Vector2.left;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision) //adding enemys in the light to the list for effects application
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null && !enemiesInLight.Contains(enemy))
            {
                enemiesInLight.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //removing enemys from the list when they exit the light
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemiesInLight.Remove(enemy);
                enemy.ClearLightEffects();
            }
        }
    }

    void UpdateBeamVisuals() //visual transition between wide and focused beam math
    {
        //if (currentMode == BeamMode.Focused)
        //{
        //    flashbulb.pointLightOuterAngle = 25f;
        //    flashbulb.pointLightInnerAngle = 10f;
        //    flashbulb.intensity = 1.25f; // optional: feels stronger
        //}
        //else
        //{
        //    flashbulb.pointLightOuterAngle = 45f;
        //    flashbulb.pointLightInnerAngle = 25f;
        //    flashbulb.intensity = 1f;//
        //}
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

    void ApplyEffectsToEnemies()
    {
        foreach (Enemy enemy in enemiesInLight)
        {
            if (enemy == null)
                continue;

            switch (currentMode)
            {
                case BeamMode.Wide:
                    enemy.ApplySlow(0.5f);
                    break;

                case BeamMode.Focused:
                    enemy.ApplyStun(0.7f);
                    break;
            }
        }
        //enemiesInLight.RemoveAll(e => e == null);
    }

    void BatterySystem() //drain battery when in focused mode, trigger drain state and visuals
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

    void RechargeMash() //mash to recharge system, trigger on empty battery, start timer and count mashes, if successful recharge if not start cooldown
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
            if (Input.GetKeyDown(KeyCode.F))
            {
                recoveryStarted = true;
                isRecovering = true;
                mashTimer = 0f;
                mashCount = 0;
            }

            return;
        }

        mashTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.F))
        {
            mashCount++;
            UnityEngine.Debug.Log("Mash Count: " + mashCount);
        }

        if (mashTimer < mashTimeRequired)
            return;

        if (mashCount >= mashRequiredCount)
        {
            RechargeBattery();
        }
        else
        {
            FailRecovery();
        }

        recoveryStarted = false;
    }

    void RechargeBattery() //successful recharge, reset states and visuals
    {
        UnityEngine.Debug.Log("Battery Recharged!");

        isDrained = false;
        isRecovering = false;

        currentBattery = maxBattery;
        UpdateFlashlightState();
    }

    void FailRecovery() //failed recharge, trigger cooldown and reset recovery started?
    {
        UnityEngine.Debug.Log("Recovery Failed - Cooldown!");

        isRecovering = false;
        recoveryStarted = false;
        cooldownTimer = cooldownTime;
    }

    void UpdateFlashlightState()
    {
        bool active = isFlashlightOn && !isDrained;

        flashbulb.enabled = active;
        flashbulbvol.enabled = active;
        detectray.enabled = active;

        if (!active)
        {
            // clear all enemies when light turns off
            foreach (Enemy enemy in enemiesInLight)
            {
                if (enemy == null)
                    continue;

                enemy.ClearLightEffects();
            }

            enemiesInLight.Clear();
        }
    }
}
