using System.Collections;
using UnityEngine;

public class SharkSpecial : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Rigidbody2D rb;

    [Header("Attack")]
    [SerializeField]
    private float attackRange = 14f;

    [SerializeField]
    private float SPCooldown = 15f;

    [Header("Jump Arc")]
    [SerializeField]
    private float jumpDuration = 0.42f; // frame 4 -> frame 9 timing

    [SerializeField]
    private float jumpHeight = 4f;

    [SerializeField]
    private float randomLandingX = 2.25f;

    [SerializeField]
    private float landingYOffset = -1.5f;

    private float SPTimer = 0f;
    private bool isSP;
    private Vector3 savedTarget;
    private Coroutine jumpRoutine;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        SPTimer -= Time.fixedDeltaTime;

        // lock shark during special
        if (isSP)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= attackRange && SPTimer <= 0f)
        {
            Jump();
        }
    }

    // =========================
    // START SPECIAL
    // =========================
    private void Jump()
    {
        isSP = true;
        SPTimer = SPCooldown;

        // save player position now (fair dodge window)
        savedTarget = player.position;

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    // ==================================================
    // ANIMATION EVENT ON FRAME 4 (takeoff frame)
    // ==================================================
    public void StartArcJump()
    {
        if (jumpRoutine != null)
            StopCoroutine(jumpRoutine);

        jumpRoutine = StartCoroutine(ParabolicJump());
    }

    // =========================
    // ARC MOVEMENT
    // =========================
    private IEnumerator ParabolicJump()
    {
        Vector3 start = transform.position;

        Vector3 end =
            savedTarget
            + new Vector3(Random.Range(-randomLandingX, randomLandingX), landingYOffset, 0f);

        float timer = 0f;

        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;

            float t = timer / jumpDuration;
            t = Mathf.Clamp01(t);

            // horizontal move
            Vector3 pos = Vector3.Lerp(start, end, t);

            // parabola height
            float arc = 4f * jumpHeight * t * (1f - t);
            pos.y += arc;

            rb.MovePosition(pos);

            yield return null;
        }

        rb.MovePosition(end);
    }

    // ==================================================
    // ANIMATION EVENT ON FRAME 9 (landing frame)
    // ==================================================
    public void LandJump()
    {
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(FailsafeEndSpecial());
    }

    private IEnumerator FailsafeEndSpecial()
    {
        yield return new WaitForSeconds(1f);
        isSP = false;
    }

    // ==================================================
    // END OF DIVE ANIMATION EVENT
    // ==================================================
    public void EndSpecial()
    {
        isSP = false;
    }
}
