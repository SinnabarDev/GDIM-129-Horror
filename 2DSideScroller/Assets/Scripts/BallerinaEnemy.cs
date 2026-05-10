using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallerinaEnemy : MonoBehaviour
{
    private enum State
    {
        Idle,
        Teleporting
    }

    [Header("Waypoints")]
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachThreshold = 0.2f;
    [SerializeField] private float stepDelay = 0.4f;

    private int waypointIndex;

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Teleport Rule")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float teleportCooldown = 5f;
    [SerializeField] private float teleportOffset = 1.5f;

    private float teleportTimer;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 20f;
    [SerializeField] private Animator animator;

    private float attackTimer;
    private bool isAttacking;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private State state = State.Idle;
    private bool waiting;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        attackTimer -= Time.fixedDeltaTime;
        teleportTimer -= Time.fixedDeltaTime;

        EvaluateTeleportCondition();

        if (state == State.Teleporting || isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        MoveTowardWaypoint();
    }

    // =========================
    // DIRECT WAYPOINT MOVEMENT
    // =========================
    private void MoveTowardWaypoint()
    {
        if (waypoints.Count == 0) return;

        if (waiting) return;

        Transform wp = waypoints[waypointIndex];

        Vector2 toWaypoint = (Vector2)wp.position - (Vector2)transform.position;
        float dist = toWaypoint.magnitude;

        Vector2 dir = toWaypoint.normalized;

        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

        animator.SetBool("isWalking", true);

        Face(dir.x);

        // ARRIVAL
        if (dist < reachThreshold)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);

            StartCoroutine(WaitThenAdvance());
        }
    }

    private IEnumerator WaitThenAdvance()
    {
        waiting = true;

        yield return new WaitForSeconds(stepDelay);

        waypointIndex = (waypointIndex + 1) % waypoints.Count;

        waiting = false;
    }

    // =========================
    // TELEPORT CONDITION
    // =========================
    private void EvaluateTeleportCondition()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > detectionRange) return;
        if (teleportTimer > 0f) return;
        if (waiting) return;

        bool hasLOS = HasLineOfSight();
        bool playerMoving = PlayerIsMoving();

        if (hasLOS && playerMoving)
        {
            StartCoroutine(TeleportToPlayer());
        }
    }

    // =========================
    // TELEPORT
    // =========================
    private IEnumerator TeleportToPlayer()
    {
        state = State.Teleporting;
        teleportTimer = teleportCooldown;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        sr.enabled = false;

        yield return new WaitForSeconds(0.25f);

        Vector2 dir = (player.position - transform.position).normalized;
        Vector2 tp = (Vector2)player.position - dir * teleportOffset;

        transform.position = tp;

        sr.enabled = true;

        yield return new WaitForSeconds(0.4f);

        state = State.Idle;
    }

    // =========================
    // ATTACK
    // =========================
    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        rb.linearVelocity = Vector2.zero;
    }

    public void EndAttack()
    {
        isAttacking = false;
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
    // PLAYER MOVEMENT CHECK
    // =========================
    private bool PlayerIsMoving()
    {
        Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
        return prb != null && prb.linearVelocity.magnitude > 0.1f;
    }

    // =========================
    // FACING
    // =========================
    private void Face(float x)
    {
        if (x > 0.01f) sr.flipX = false;
        else if (x < -0.01f) sr.flipX = true;
    }
}