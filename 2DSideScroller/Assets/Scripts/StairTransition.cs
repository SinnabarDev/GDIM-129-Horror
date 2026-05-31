using UnityEngine;
using UnityEngine.SceneManagement;

public class StairTransition : MonoBehaviour
{
    [Header("Scene Transition")]
    [SerializeField]
    private string sceneToLoad;

    [SerializeField]
    private string targetSpawnID;

    private bool playerInRange;
    private bool isTransitioning;

    private void Update()
    {
        if (!playerInRange || isTransitioning)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            isTransitioning = true;

            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.SpawnPointID = targetSpawnID;
            }
            else
            {
                Debug.LogWarning("SceneTransitionManager not found!");
            }

            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
