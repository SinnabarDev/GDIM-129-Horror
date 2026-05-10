using System.Collections;
using UnityEngine;

public class BallerinaAudioManager : MonoBehaviour
{
    public enum AudioState
    {
        Dancing,
        Stepping,
        Teleporting,
        Attacking,
        Idle
    }

    [Header("Dance / Movement")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource shuffle;

    [Header("Teleport")]
    [SerializeField] private AudioSource muttering;

    [Header("Attack")]
    [SerializeField] private AudioSource attack;

    private AudioState currentState;

    // =========================
    // PUBLIC STATE CONTROL
    // =========================
    public void SetState(AudioState newState)
    {
        if (currentState == newState) return;

        StopAllLoops();

        currentState = newState;

        switch (currentState)
        {
            case AudioState.Dancing:
                PlayDanceLoop();
                break;

            case AudioState.Stepping:
                PlayStepLoop();
                break;

            case AudioState.Teleporting:
                PlayTeleport();
                break;

            case AudioState.Attacking:
                PlayAttack();
                break;
        }
    }

    // =========================
    // DANCE (idle loop)
    // =========================
    private void PlayDanceLoop()
    {
        if (!music.isPlaying)
            music.Play();

        if (!shuffle.isPlaying)
            shuffle.Play();
    }

    // =========================
    // STEPPING (slight variation but same feel)
    // =========================
    private void PlayStepLoop()
    {
        if (!music.isPlaying)
            music.Play();

        if (!shuffle.isPlaying)
            shuffle.Play();
    }

    // =========================
    // TELEPORT (horror cue)
    // =========================
    private void PlayTeleport()
    {
        shuffle.Stop();
        music.Stop();

        muttering.Play();
    }

    // =========================
    // ATTACK (impact + rhythm break)
    // =========================
    private void PlayAttack()
    {
        attack.Play();

        // keep subtle shuffle under attack for tension
        if (!shuffle.isPlaying)
            shuffle.Play();
    }

    // =========================
    // STOP EVERYTHING
    // =========================
    private void StopAllLoops()
    {
        music.Stop();
        shuffle.Stop();
        muttering.Stop();
        attack.Stop();
    }
}