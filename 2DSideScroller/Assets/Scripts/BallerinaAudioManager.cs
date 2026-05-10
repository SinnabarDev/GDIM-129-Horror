using UnityEngine;

public class BallerinaAudioManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource creepyMusic;
    [SerializeField] private AudioSource mutter;
    [SerializeField] private AudioSource attack;
    [SerializeField] private AudioSource shuffle;

    private bool isWaiting;

    private void Start()
    {
        if (creepyMusic != null)
        {
            creepyMusic.loop = true;
            creepyMusic.Stop();
        }
    }

    private void Update()
    {
        HandleMusic();
    }

    // =========================
    // LOOP MUSIC STATE
    // =========================
    private void HandleMusic()
    {
        if (creepyMusic == null)
            return;

        if (!isWaiting)
        {
            if (!creepyMusic.isPlaying)
                creepyMusic.Play();
        }
        else
        {
            if (creepyMusic.isPlaying)
                creepyMusic.Pause();
        }
    }

    // =========================
    // CALLED BY NPC STATE
    // =========================
    public void SetWaiting(bool waiting)
    {
        isWaiting = waiting;
    }

    // =========================
    // TELEPORT ONE SHOT
    // =========================
    public void PlayTeleport()
    {
        if (mutter == null) return;

        mutter.pitch = Random.Range(0.9f, 1.1f);
        mutter.PlayOneShot(mutter.clip);
    }

    // =========================
    // ANIMATION EVENT
    // ATTACK SWING
    // =========================
    public void PlayAttack()
    {
        if (attack == null) return;

        attack.pitch = Random.Range(0.95f, 1.05f);
        attack.PlayOneShot(attack.clip);
    }

    // =========================
    // ANIMATION EVENT
    // SHUFFLE IMPACT
    // =========================
    public void PlayShuffle()
    {
        if (shuffle == null) return;

        shuffle.pitch = Random.Range(0.95f, 1.1f);
        shuffle.PlayOneShot(shuffle.clip);
    }
}