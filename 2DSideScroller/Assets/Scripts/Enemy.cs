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
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;

    [Header("Patrol")]
    [SerializeField] private List<Transform> waypoints;

    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 6f;
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
    private float slowBlend = 1f;
    [SerializeField] private float stunDuration = 3f;
    private bool isStunned = false;

    // =========================
    // INTERNAL STATE
    // =========================
    private State state = State.Patrol;

    private Rigidbody2D rb;

    private int waypointIndex = 0;
    private float lastSeenTime;

    private Vector2 velocity;

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
        // STUN STATE (HARD LOCK)
        // =========================
        if (isStunned)
        {
            stunDuration -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (stunDuration <= 0f)
            {
                isStunned = false;
            }

            return;
        }
       

        // =========================
        // RECOVERY (only when NOT stunned)
        // =========================
        slowBlend = Mathf.Lerp(slowBlend, 1f, Time.fixedDeltaTime * 1.2f);

        // =========================
        // PERCEPTION
        // =========================
        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= detectionRange;
        bool hasLOS = HasLineOfSight();

        // =========================
        // STATE LOGIC
        // =========================
        if (inRange)
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

            target = waypoints[waypointIndex].position;
            speed = patrolSpeed;

            Vector2 pos = transform.position;

            if (Mathf.Abs(pos.x - target.x) < 0.3f)
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

        float finalSpeed = speed * slowBlend;
        velocity = direction * finalSpeed;

        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

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
                $"Slow:{slowBlend:F2} | " +
                $"Stunned:{isStunned} | " +
                $"FinalSpeed:{finalSpeed:F2}"
            );
        }

        // =========================
        // FLIP
        // =========================
        FaceDirection(direction.x);
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
    // LOS
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
        slowBlend = Mathf.Lerp(slowBlend, targetSlow, Time.deltaTime * 5f);
    }

    // 🔥 NEW: STACK SYSTEM
    public void ApplyStun(float amount)
    {
        ApplySlow(amount);
        TriggerStun();
    }

    private void TriggerStun()
    {
        isStunned = true;
        stunDuration = 2f;
    }

    public void ClearLightEffects()
    {
        slowBlend = 1f;
    }
}