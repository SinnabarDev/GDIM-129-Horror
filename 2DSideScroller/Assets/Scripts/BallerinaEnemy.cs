using System.Collections;
using UnityEngine;

public class StalkerNPC : MonoBehaviour, IExorcisable
{
    public enum NPCState
    {
        Moving,
        Waiting,
    }

    [Header("Audio")]
    [SerializeField]
    private BallerinaAudioManager audioManager;

    // =========================
    // FLASHLIGHT / STUN SYSTEM
    // =========================
    [Header("Light Effects")]
    private float moveDebuff = 1f;

    [SerializeField]
    private bool isSpawnable = true;

    [SerializeField]
    private float stunDuration = 2f;

    public bool isStunned = false;
    private float stunTimer;

    // =========================
    // WAYPOINT SYSTEM
    // =========================
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float checkDist = 1f;

    [Header("State Timing")]
    public float minMoveTime = 2f;
    public float maxMoveTime = 5f;

    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    // =========================
    // DETECTION
    // =========================
    [Header("Detection")]
    public Transform player;
    public float detectionRange = 8f;
    public LayerMask obstacleMask;
    private bool playerMoving;

    // =========================
    // TELEPORT
    // =========================
    [Header("Teleport")]
    public float teleportOffset = 2f;

    [SerializeField]
    private Rigidbody2D playerRb;
    public float teleportCooldown = 3f;
    private float teleportTimer;

    // =========================
    // COMBAT
    // =========================
    [Header("Combat")]
    public float attackCooldown = 2f;
    public float postAttackPause = 0.5f;

    [SerializeField]
    private float attackRange = 3f;
    private float attackTimer;
    private float postAttackTimer;
    private bool isAttacking;

    // =========================
    // STATE
    // =========================
    private NPCState currentState;
    private int currentWaypoint;

    private bool isDisabled;

    // =========================
    // COMPONENTS
    // =========================
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // =========================
    // PROGRESS
    // =========================
    private int savedProgress = 0;

    public int GetSavedProgress() => savedProgress;

    public void SetSavedProgress(int value) => savedProgress = value;

    public bool IsStunned() => isStunned;

    // =========================
    // INIT
    // =========================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Start()
    {
        StartCoroutine(StateLoop());
    }

    // =========================
    // UPDATE
    // =========================
    void Update()
    {
        if (player == null || isDisabled)
        {
            animator.SetBool("isWalking", false);
            return;
        }
        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (!isAttacking && attackTimer <= 0f && distToPlayer <= attackRange)
        {
            Attack();
            return;
        }

        HandleTimers();

        HandleStun();
        HandleAttackLock();

        if (isDisabled || isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }
        if (currentState == NPCState.Waiting)
        {
            audioManager.SetWaiting(true);
            animator.SetBool("isWalking", false);
        }

        CheckPlayerMovement();

        if (currentState == NPCState.Moving)
        {
            MoveToWaypoint();
            audioManager.SetWaiting(false);
        }
        else if (
            currentState == NPCState.Waiting
            && CanSeePlayer()
            && playerMoving
            && teleportTimer <= 0f
        )
        {
            TeleportNearPlayer();
            teleportTimer = teleportCooldown;
        }
    }

    private void FaceDirection(float xDir)
    {
        if (xDir > 0.01f)
            sr.flipX = false;
        else if (xDir < -0.01f)
            sr.flipX = true;
    }

    // =========================
    // STATE MACHINE
    // =========================
    IEnumerator StateLoop()
    {
        while (true)
        {
            currentState = NPCState.Moving;
            yield return new WaitForSeconds(Random.Range(minMoveTime, maxMoveTime));

            currentState = NPCState.Waiting;
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }

    // =========================
    // MOVEMENT
    // =========================
    void MoveToWaypoint()
    {
        if (waypoints.Length == 0)
            return;

        Transform target = waypoints[currentWaypoint];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * moveDebuff * Time.deltaTime
        );
        Vector2 direction = target.position - transform.position;
        FaceDirection(direction.x);
        animator.SetBool("isWalking", true);

        if (Vector2.Distance(transform.position, target.position) <= checkDist)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void CheckPlayerMovement()
    {
        playerMoving = playerRb.linearVelocity.sqrMagnitude > 0.50f;
    }

    // =========================
    // DETECTION
    // =========================
    bool CanSeePlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > detectionRange)
            return false;

        Vector2 dir = (player.position - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRange, obstacleMask);

        return hit.collider == null;
    }

    // =========================
    // TELEPORT
    // =========================
    void TeleportNearPlayer()
    {
        audioManager.PlayTeleport();
        float side = Mathf.Sign(player.position.x - transform.position.x);
        transform.position = new Vector3(
            player.position.x + (side * teleportOffset),
            transform.position.y,
            transform.position.z
        );
        FaceDirection(player.position.x - transform.position.x);
    }

    // =========================
    // COMBAT
    // =========================
    private void Attack()
    {
        if (isAttacking || isDisabled || isStunned)
            return;

        isAttacking = true;
        attackTimer = attackCooldown;

        rb.linearVelocity = Vector2.zero;

        if (animator)
            animator.SetTrigger("Attack");
    }

    public void EndAttack()
    {
        isAttacking = false;
        postAttackTimer = postAttackPause;
    }

    void HandleAttackLock()
    {
        if (postAttackTimer > 0f)
        {
            postAttackTimer -= Time.deltaTime;
            animator.SetBool("isWalking", false);
        }
    }

    void HandleTimers()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
        if (teleportTimer > 0f)
            teleportTimer -= Time.deltaTime;
    }

    // =========================
    // STUN
    // =========================
    void HandleStun()
    {
        if (!isStunned)
            return;

        stunTimer -= Time.deltaTime;

        animator.SetBool("isWalking", false);
        ;
        rb.linearVelocity = Vector2.zero;
        if (stunTimer <= 0f)
            isStunned = false;
    }

    public void ApplySlow(float amount)
    {
        moveDebuff = Mathf.Lerp(moveDebuff, amount, Time.deltaTime * 5f);
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

    // =========================
    // DISABLE SYSTEM
    // =========================
    public void TriggerDisable()
    {
        if (!isDisabled)
            StartCoroutine(DisableRoutine());
    }

    IEnumerator DisableRoutine()
    {
        isDisabled = true;

        isAttacking = false;
        isStunned = false;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.bodyType = RigidbodyType2D.Kinematic;

        // Disable animator
        if (animator != null)
            animator.enabled = false;

        // Disable audio
        AudioSource[] audios = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource a in audios)
        {
            a.Stop();
            a.enabled = false;
        }

        // Disable all sprites
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in sprites)
            s.enabled = false;

        // Disable all colliders
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in cols)
            c.enabled = false;

        yield return new WaitForSeconds(Random.Range(10f, 15f));

        if (isSpawnable)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;

            // Re-enable animator
            if (animator != null)
                animator.enabled = true;

            // Re-enable audio
            foreach (AudioSource a in audios)
                a.enabled = true;

            foreach (SpriteRenderer s in sprites)
                s.enabled = true;

            foreach (Collider2D c in cols)
                c.enabled = true;

            savedProgress = 0;
            isDisabled = false;
        }
    }

    // =========================
    // HELPERS
    // =========================
}
