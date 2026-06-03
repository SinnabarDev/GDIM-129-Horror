using TMPro;
using UnityEngine;

public class HideSpotLogic : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float hideDuration = 5f;

    [SerializeField]
    private float spawnOffset = 1.5f;

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

    [SerializeField]
    private GameObject flashlightObject;

    [SerializeField]
    private GameObject visionObject;

    public static bool isPlayerHiding = false;

    private static HideSpotLogic activeHideSpot;

    private bool playerInRange;
    private float currentHideTimer;

    private SpriteRenderer[] playerSpriteRenderers;
    private Collider2D[] playerColliders;
    private Rigidbody2D playerRb;
    private Vector3 spawnPosition;

    private void Start()
    {
        if (player != null)
        {
            playerSpriteRenderers = player.GetComponentsInChildren<SpriteRenderer>(true);
            playerColliders = player.GetComponentsInChildren<Collider2D>(true);
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        if (interactUI != null)
            interactUI.SetActive(false);

        if (timerUI != null)
            timerUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null)
            return;

        if (isPlayerHiding)
        {
            if (activeHideSpot == this)
                HandleHidingTimer();

            return;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            EnterHideSpot();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactUI != null)
            interactUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void EnterHideSpot()
    {
        activeHideSpot = this;
        isPlayerHiding = true;
        spawnPosition = player.position;

        if (interactUI != null)
            interactUI.SetActive(false);

        if (timerUI != null)
            timerUI.SetActive(true);

        foreach (SpriteRenderer sr in playerSpriteRenderers)
            sr.enabled = false;

        foreach (Collider2D col in playerColliders)
            col.enabled = false;

        if (playerControlScript != null)
            playerControlScript.enabled = false;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
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

        if (timerText != null)
            timerText.text = "TIME LEFT: " + Mathf.Ceil(currentHideTimer);

        if (currentHideTimer <= 0)
        {
            ExitHideSpot();
        }
    }

    private void ExitHideSpot()
    {
        activeHideSpot = null;
        isPlayerHiding = false;

        if (timerUI != null)
            timerUI.SetActive(false);

        int randomDirection = Random.Range(0, 2) == 0 ? -1 : 1;

        //Vector3 spawnPosition = transform.position + new Vector3(randomDirection * spawnOffset, 0f, 0f);

        player.position = spawnPosition;

        if (playerRb != null)
        {
            playerRb.simulated = true;
            playerRb.bodyType = RigidbodyType2D.Dynamic;
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        foreach (SpriteRenderer sr in playerSpriteRenderers)
            sr.enabled = true;

        foreach (Collider2D col in playerColliders)
            col.enabled = true;

        if (playerControlScript != null)
            playerControlScript.enabled = true;

        if (flashlightObject != null)
            flashlightObject.SetActive(true);

        if (visionObject != null)
            visionObject.SetActive(true);
    }
}
