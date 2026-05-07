using System.Collections;
using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
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
    // MOVEMENT LOOP
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

    // =========================
    // ATTACK EVENT
    // =========================
    public void PlayAttack()
    {
        if (attack == null || isAttacking)
            return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // stop loops
        if (muttering != null && muttering.isPlaying)
            muttering.Stop();

        if (movement != null && movement.isPlaying)
            movement.Stop();

        // slight variation
        if (attack != null)
        {
            attack.pitch = Random.Range(0.95f, 1.05f);
            attack.PlayOneShot(attack.clip);
        }

        // optional: delay bat slightly for impact feel
        yield return new WaitForSeconds(0.05f);

        if (bat != null)
        {
            bat.pitch = Random.Range(0.95f, 1.1f);
            bat.PlayOneShot(bat.clip);
        }

        // wait for longest sound
        float waitTime = 0f;

        if (attack != null)
            waitTime = Mathf.Max(waitTime, attack.clip.length);

        if (bat != null)
            waitTime = Mathf.Max(waitTime, bat.clip.length);

        yield return new WaitForSeconds(waitTime);

        // resume muttering
        if (muttering != null && !muttering.isPlaying)
            muttering.Play();

        isAttacking = false;
    }
}
