using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // =========================
    // STATE
    // =========================
    private enum State
    {
        Patrol,
        Chase
    }

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    private float debugTimer;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 2f;

    [Header("Patrol")]
    [SerializeField] private List<Transform> waypoints;

    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("LOS Surprise Aggro")]
    [Range(0f, 1f)]
    [SerializeField] private float surpriseChance = 0.3f;

    [Header("Lose Interest")]
    [SerializeField] private float loseInterestTime = 3f;

    [Header("Visuals")]
    private SpriteRenderer sr;

    // =========================
    // FLASHLIGHT EFFECTS
    // =========================
    private float moveDebuff = 1f;

    [SerializeField] private float stunDuration = 2f;
    private float stunTimer = 0f;
    public bool isStunned = false;

    // =========================
    // INTERNAL STATE
    // =========================
    private State state = State.Patrol;

    private Rigidbody2D rb;
    private int waypointIndex = 0;
    private float lastSeenTime;
    private Vector2 velocity;
[Header("Typing Weakness")]

private int savedProgress = 0;
private Vector2 savedVelocity;
private bool isDisabled = false;

public int GetSavedProgress() => savedProgress;
public void SetSavedProgress(int value)
{
    savedProgress = value;
}
public bool IsStunned() => isStunned;


    // =========================
    // INIT
    // =========================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.freezeRotation = true;
    }

    // =========================
    // PHYSICS
    // =========================
    private void FixedUpdate()
    {
        if (player == null) return;

        // =========================
        // STUN
        // =========================
        if (isStunned)
        {
            stunTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (stunTimer <= 0f)
            {
                isStunned = false;
            }

            return;
        }
        if (isDisabled)
{
    rb.linearVelocity = Vector2.zero;
    return;
}

        // =========================
        // RECOVER SLOW
        // =========================
        moveDebuff = Mathf.Lerp(moveDebuff, 1f, Time.fixedDeltaTime * 1.2f);

        // =========================
        // DETECTION
        // =========================
        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= detectionRange;
        bool hasLOS = HasLineOfSight();

        // =========================
        // STATE LOGIC
        // =========================
if (HideSpotLogic.isPlayerHiding)
{
    state = State.Patrol;
}
        if (inRange && !HideSpotLogic.isPlayerHiding)
        {
            state = State.Chase;
            lastSeenTime = Time.time;
        }

        if (state == State.Patrol && inRange && hasLOS)
        {
            if (Random.value < surpriseChance)
            {
                state = State.Chase;
                lastSeenTime = Time.time;
            }
        }

        if (state == State.Chase && !inRange)
        {
            if (Time.time - lastSeenTime > loseInterestTime)
            {
                state = State.Patrol;
            }
        }

        // =========================
        // TARGET + SPEED
        // =========================
        Vector2 target;
        float speed;

        if (state == State.Patrol)
        {
            if (waypoints.Count == 0) return;

            // PLATFORMER PATROL (X ONLY)
            target = new Vector2(
                waypoints[waypointIndex].position.x,
                transform.position.y
            );

            speed = patrolSpeed;

            if (Mathf.Abs(transform.position.x - target.x) < 0.2f)
            {
                waypointIndex = (waypointIndex + 1) % waypoints.Count;
            }
        }
        else
        {
            target = player.position;
            speed = chaseSpeed;
        }

        // =========================
        // MOVEMENT
        // =========================
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        float finalSpeed = speed * moveDebuff;
        velocity = direction * finalSpeed;

        // PLATFORMER MOVE X ONLY
        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        // =========================
        // FLIP
        // =========================
        FaceDirection(direction.x);

        // =========================
        // DEBUG
        // =========================
        debugTimer -= Time.fixedDeltaTime;

        if (showDebug && debugTimer <= 0f)
        {
            debugTimer = 0.25f;

            Debug.Log(
                $"[{gameObject.name}] " +
                $"State:{state} | " +
                $"Waypoint:{waypointIndex} | " +
                $"Slow:{moveDebuff:F2} | " +
                $"Stunned:{isStunned}"
            );
        }
    }

    // =========================
    // FLIP
    // =========================
    private void FaceDirection(float xDir)
    {
        if (xDir > 0.01f)
            sr.flipX = false;
        else if (xDir < -0.01f)
            sr.flipX = true;
    }

    // =========================
    // LOS CHECK
    // =========================
    private bool HasLineOfSight()
    {
        Vector2 origin = transform.position;
        Vector2 dir = (player.position - transform.position).normalized;
        float dist = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, obstacleMask);

        return hit.collider == null || hit.collider.transform == player;
    }

    // =========================
    // FLASHLIGHT EFFECTS
    // =========================
    public void ApplySlow(float targetSlow = 0.5f)
    {
        moveDebuff = Mathf.Lerp(moveDebuff, targetSlow, Time.deltaTime * 5f);
    }

    public void ApplyStun(float amount)
    {
        ApplySlow(amount);
        TriggerStun();
    }

    private void TriggerStun()
    {
        isStunned = true;
        stunTimer = stunDuration;
    }

    public void ClearLightEffects()
    {
        moveDebuff = 1f;
    }

public void TriggerDisable()
{
    if (!isDisabled)
        StartCoroutine(DisableRoutine());
}
IEnumerator DisableRoutine()
{
    isDisabled = true;

    // freeze movement
    rb.linearVelocity = Vector2.zero;
    rb.angularVelocity = 0f;

    // switch to KINEMATIC (safe)
    rb.bodyType = RigidbodyType2D.Kinematic;

    sr.enabled = false;

    Collider2D col = GetComponent<Collider2D>();
    if (col != null) col.enabled = false;

    yield return new WaitForSeconds(Random.Range(10f, 15f));

    // restore physics
    rb.bodyType = RigidbodyType2D.Dynamic;

    sr.enabled = true;

    if (col != null) col.enabled = true;

    savedProgress = 0;
    isDisabled = false;
}
}