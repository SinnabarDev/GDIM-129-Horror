using TMPro;
using UnityEngine;

public class HideSpotLogic : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float interactionDistance = 2f;

    [SerializeField]
    private float hideDuration = 5f;

    [SerializeField]
    private float spawnOffset = 1.5f; // Distance to spawn left or right

    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private GameObject interactUI;

    [SerializeField]
    private GameObject timerUI;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private MonoBehaviour playerControlScript;
    public static bool isPlayerHiding = false;

    private float currentHideTimer = 0f;
    private SpriteRenderer[] playerSpriteRenderers;
    private Collider2D[] playerColliders;
    private Rigidbody2D playerRb;

    [SerializeField]
    private GameObject flashlightObject;

    [SerializeField]
    private GameObject visionObject;

    void Start()
    {
        if (player != null)
        {
            playerSpriteRenderers = player.GetComponentsInChildren<SpriteRenderer>(true);
            playerColliders = player.GetComponentsInChildren<Collider2D>(true);
            playerRb = player.GetComponent<Rigidbody2D>();
        }
        interactUI.SetActive(false);
        timerUI.SetActive(false);
    }

    void Update()
    {
        if (player == null)
            return;

        if (isPlayerHiding)
        {
            HandleHidingTimer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionDistance)
        {
            interactUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                EnterHideSpot();
            }
        }
        else
        {
            interactUI.SetActive(false);
        }
    }

    private void EnterHideSpot()
    {
        isPlayerHiding = true;

        interactUI.SetActive(false);
        timerUI.SetActive(true);

        if (playerSpriteRenderers != null)
        {
            foreach (SpriteRenderer sr in playerSpriteRenderers)
            {
                sr.enabled = false;
            }
        }

        // Disable colliders
        if (playerColliders != null)
        {
            foreach (Collider2D col in playerColliders)
            {
                col.enabled = false;
            }
        }

        // Disable movement / controls
        if (playerControlScript != null)
        {
            playerControlScript.enabled = false;
        }
        // Disable Rigidbody safely
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;

            // Prevent falling
            playerRb.bodyType = RigidbodyType2D.Kinematic;
            playerRb.simulated = false;
        }
        if (flashlightObject != null)
            flashlightObject.SetActive(false);

        if (visionObject != null)
            visionObject.SetActive(false);

        currentHideTimer = hideDuration;
    }

    private void HandleHidingTimer()
    {
        currentHideTimer -= Time.deltaTime;

        // Update the UI text to show whole seconds (Mathf.Ceil rounds up to the nearest whole number)
        timerText.text = "TIME LEFT: " + Mathf.Ceil(currentHideTimer).ToString();

        if (currentHideTimer <= 0)
        {
            ExitHideSpot();
        }
    }

    private void ExitHideSpot()
    {
        isPlayerHiding = false;
        timerUI.SetActive(false);

        // Randomly pick -1 (left) or 1 (right)
        int randomDirection = Random.Range(0, 2) == 0 ? -1 : 1;

        // Calculate the new position based on the closet's position plus the offset
        Vector3 spawnPosition =
            transform.position + new Vector3(randomDirection * spawnOffset, 0f, 0f);

        // Move the player and reactivate
        player.position = spawnPosition;
        // Restore Rigidbody FIRST
        if (playerRb != null)
        {
            playerRb.simulated = true;
            playerRb.bodyType = RigidbodyType2D.Dynamic;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        if (playerSpriteRenderers != null)
        {
            foreach (SpriteRenderer sr in playerSpriteRenderers)
            {
                sr.enabled = true;
            }
        }
        if (playerColliders != null)
        {
            foreach (Collider2D col in playerColliders)
            {
                col.enabled = true;
            }
        }

        // Enable controls
        if (playerControlScript != null)
        {
            playerControlScript.enabled = true;
        }
        if (flashlightObject != null)
            flashlightObject.SetActive(true);

        if (visionObject != null)
            visionObject.SetActive(true);
    }
}
