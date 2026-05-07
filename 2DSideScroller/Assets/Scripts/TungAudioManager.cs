using System.Collections;
using UnityEngine;

public class TungAudioManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioSource muttering;

    [SerializeField]
    private AudioSource movement;

    [SerializeField]
    private AudioSource attack;

    [SerializeField]
    private AudioSource bat;

    private Rigidbody2D rb;
    private bool isAttacking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Start baseline muttering
        if (muttering != null)
        {
            muttering.loop = true;
            muttering.Play();
        }
    }

    private void Update()
    {
        HandleMovementLoop();
    }

    // =========================
    // MOVEMENT LOOP (ONLY IF NOT ATTACKING)
    // =========================
    private void HandleMovementLoop()
    {
        if (movement == null || rb == null)
            return;

        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.05f;

        if (moving && !isAttacking)
        {
            if (!movement.isPlaying)
                movement.Play();
        }
        else
        {
            if (movement.isPlaying)
                movement.Stop();
        }
    }

    // ======================================================
    // ANIMATION EVENT: START OF ATTACK (STATE CONTROL)
    // ======================================================
    public void StartAttack()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        // stop all looping audio
        if (muttering != null && muttering.isPlaying)
            muttering.Stop();

        if (movement != null && movement.isPlaying)
            movement.Stop();
    }

    // ======================================================
    // ANIMATION EVENT: ATTACK SOUND (WINDUP / VOICE)
    // ======================================================
    public void PlayAttack()
    {
        if (attack != null)
        {
            attack.pitch = Random.Range(0.95f, 1.05f);
            attack.PlayOneShot(attack.clip);
        }
    }

    // ======================================================
    // ANIMATION EVENT: BAT IMPACT SOUND
    // ======================================================
    public void PlayBat()
    {
        if (bat != null)
        {
            bat.pitch = Random.Range(0.95f, 1.1f);
            bat.PlayOneShot(bat.clip);
        }
    }

    // ======================================================
    // ANIMATION EVENT: END OF ATTACK
    // ======================================================
    public void StopAttack()
    {
        isAttacking = false;

        // resume muttering loop
        if (muttering != null && !muttering.isPlaying)
            muttering.Play();
    }
}
