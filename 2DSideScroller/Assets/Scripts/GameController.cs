using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public float playerMaxHealth = 100f;
    public float currentHealth;

    // Major Game Events
    public static event Action OnPauseGame; // Implemented✅

    //public static event Action OnQuitGame;
    //public static event Action OnRestartGame;
    public static event Action<int> OnPlayerHit;

    public static event Action OnPlayerDeath;

    private bool isGamePaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentHealth = playerMaxHealth;
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
            currentHealth = playerMaxHealth;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            OnPauseGame?.Invoke();
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void UpdatePlayerGetHit(int damage)
    {
        OnPlayerHit?.Invoke(damage);
        currentHealth -= damage;
    }
}
