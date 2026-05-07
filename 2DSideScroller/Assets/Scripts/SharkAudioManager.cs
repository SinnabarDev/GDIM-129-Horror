using System.Collections;
using UnityEngine;

public class SharkAudioManager : MonoBehaviour
{
    [Header("Looping Audio")]
    [SerializeField]
    private AudioSource muttering;

    [SerializeField]
    private AudioSource movement;

    [Header("Attack Audio")]
    [SerializeField]
    private AudioSource bite;

    [SerializeField]
    private AudioSource jump;

    private Rigidbody2D rb;

    private bool isBusy;

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

    // =================================
    // MOVEMENT LOOP
    // =================================
    private void HandleMovementLoop()
    {
        if (movement == null || rb == null)
            return;

        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.05f;

        if (moving && !isBusy)
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

    // =================================
    // BITE ATTACK
    // Animation Event
    // =================================
    public void PlayBiteAttack()
    {
        if (bite == null)
            return;

        StartCoroutine(BiteRoutine());
    }

    private IEnumerator BiteRoutine()
    {
        isBusy = true;

        StopLoops();

        bite.pitch = Random.Range(0.95f, 1.05f);
        bite.PlayOneShot(bite.clip);

        yield return new WaitForSeconds(bite.clip.length);

        ResumeMuttering();

        isBusy = false;
    }

    // =================================
    // JUMP ATTACK
    // Animation Event
    // =================================
    public void PlayJumpAttack()
    {
        if (jump == null)
            return;

        StartCoroutine(JumpRoutine());
    }

    private IEnumerator JumpRoutine()
    {
        isBusy = true;

        StopLoops();

        jump.pitch = Random.Range(0.95f, 1.1f);
        jump.PlayOneShot(jump.clip);

        yield return new WaitForSeconds(jump.clip.length);

        ResumeMuttering();

        isBusy = false;
    }

    // =================================
    // HELPERS
    // =================================
    private void StopLoops()
    {
        if (muttering != null && muttering.isPlaying)
            muttering.Stop();

        if (movement != null && movement.isPlaying)
            movement.Stop();
    }

    private void ResumeMuttering()
    {
        if (muttering != null && !muttering.isPlaying)
            muttering.Play();
    }
}
