using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class FlashlightAim : MonoBehaviour
{
    [SerializeField] private Light2D flashbulb;
    public bool isFacingRight = true;

    private List<Enemy> enemiesInLight = new List<Enemy>();

    [Header("Battery")]
    [SerializeField] private float maxBattery = 5f;
    [SerializeField] private float batteryDrainRate = 1f; // 1 second = 1 battery

    [Header("Mash Recovery")]
    [SerializeField] private float mashTimeRequired = 5f;
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private int mashRequiredCount = 20;

    private bool isDrained = false;
    private bool isRecovering = false;
    private bool recoveryStarted = false;
    private float mashTimer = 0f;
    private int mashCount = 0;
    private float cooldownTimer = 0f;

private float currentBattery;
    public enum BeamMode
    {
        Wide,     // slow
        Focused   // stun
    }
    private BeamMode currentMode = BeamMode.Wide;

    void Start()
    {
        currentBattery = maxBattery;
    }

    void Update()
    {
        AimFlashlight();
        BatterySystem();

        if (Input.GetMouseButtonDown(1)) // toggle on click
        {   
            UnityEngine.Debug.Log("Right Mouse Button Pressed");
            if (!isDrained && !isRecovering)
            {
            currentMode = currentMode == BeamMode.Wide 
                ? BeamMode.Focused 
                : BeamMode.Wide;

            }    
        }
        UpdateBeamVisuals();
        //ApplyEffectsToEnemies();
        RechargeMash();
    }

    void AimFlashlight()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 direction = mousePos - transform.position;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        //angle = Mathf.Clamp(angle, -180f, 0f);

        //transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector2 direction = (mousePos - transform.position).normalized;

        // 🚫 Block backwards aiming
        if (isFacingRight && direction.x < 0)
            direction.x = 0;

        if (!isFacingRight && direction.x > 0)
            direction.x = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetFacing(bool facingRight)
    {
        isFacingRight = facingRight;
    }

private void OnTriggerEnter2D(Collider2D collision)
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

private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.CompareTag("Enemy"))
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemiesInLight.Remove(enemy);
            //enemy.ClearLightEffects();
        }
    }
}
void UpdateBeamVisuals()
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

    flashbulb.pointLightOuterAngle = Mathf.Lerp(flashbulb.pointLightOuterAngle, targetOuter, Time.deltaTime * 10f);
    flashbulb.pointLightInnerAngle = Mathf.Lerp(flashbulb.pointLightInnerAngle, targetInner, Time.deltaTime * 3f);
    flashbulb.intensity = Mathf.Lerp(flashbulb.intensity, currentMode == BeamMode.Focused ? 1.25f : 1f, Time.deltaTime * 0.1f);
}

void ApplyEffectsToEnemies()
{
    foreach (Enemy enemy in enemiesInLight)
    {
        if (currentMode == BeamMode.Wide)
        {
            //enemy.ApplySlow();
        }
        else
        {
            //enemy.ApplyStun();
        }
    }
}

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
            Debug.Log("Battery Drained!");

        }
    }
}

void RechargeMash()
{
    if (!isDrained) return;

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
void RechargeBattery()
{
    UnityEngine.Debug.Log("Battery Recharged!");

    isDrained = false;
    isRecovering = false;

    currentBattery = maxBattery;
    flashbulb.enabled = true;
}

void FailRecovery()
{
    UnityEngine.Debug.Log("Recovery Failed - Cooldown!");

    isRecovering = false;
    recoveryStarted = false;
    cooldownTimer = cooldownTime;
}
}