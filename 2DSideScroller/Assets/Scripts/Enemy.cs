using UnityEngine;

public class Enemy : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Inspector-exposed fields
    // -------------------------------------------------------------------------

    /*[Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Wander Timing")]
    [Tooltip("Minimum seconds before picking a new state.")]
    [SerializeField] private float minWaitTime = 1f;

    [Tooltip("Maximum seconds before picking a new state.")]
    [SerializeField] private float maxWaitTime = 5f;

    // -------------------------------------------------------------------------
    // Private state
    // -------------------------------------------------------------------------

    // The three possible behaviours the enemy can be in at any moment.
    private enum WanderState { Idle, MoveLeft, MoveRight }

    private WanderState currentState = WanderState.Idle;

    // Counts UP each frame. When it reaches, a new
    // state is chosen and the counters are reset.
    private float stateElapsed = 0f;

    // The randomly-chosen duration (in seconds) the enemy stays in the
    // current state before switching. Re-rolled every time a new state begins.
    private float stateTimer = 0f;*/

    // Cached component references
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        //ChooseNewState();
    }

    /// <summary>
    /// Using FixedUpdate ensures that physics-based movement stays smooth and
    /// interacts properly with colliders (walls, platforms, other).
    /// </summary>
    private void FixedUpdate()
    {
        /*// --- Timer logic ---------------------------------------------------
        stateElapsed += Time.fixedDeltaTime;

        if (stateElapsed >= stateTimer)
        {
            ChooseNewState();
        }

        // --- Movement logic ------------------------------------------------
        switch (currentState)
        {
            case WanderState.MoveLeft:
                rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
                FaceDirection(left: true);
                break;

            case WanderState.MoveRight:
                rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
                FaceDirection(left: false);
                break;

            case WanderState.Idle:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                break;
        }*/
    }

    /// Randomly selects one of the three states, then rolls a new wait duration
    /// between minWaitTime and MaxWaitTime. The elapsed counter is reset to 0 so the
    /// new state gets its full time slice.
    /*private void ChooseNewState()
    {
        int roll = Random.Range(0, 3);
        // 0 = Idle, 1 = MoveLeft, 2 = MoveRight

        currentState = roll switch
        {
            0 => WanderState.Idle,
            1 => WanderState.MoveLeft,
            _ => WanderState.MoveRight,
        };

        // Roll a new duration for the freshly-chosen state.
        stateTimer = Random.Range(minWaitTime, maxWaitTime);

        // Reset elapsed time for a new state. 
        stateElapsed = 0f;
    }
    */
    /// Flips the SpriteRenderer so the enemy sprite always faces
    /// the direction it is walking.
    private void FaceDirection(bool left)
    {
        sr.flipX = left;
    }
}
