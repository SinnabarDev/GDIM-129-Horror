using UnityEngine;
using UnityEngine.SceneManagement;

public class StairTransition : MonoBehaviour
{
    public string sceneToLoad;
    public string targetSpawnID;

    private bool playerInRange;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SceneTransitionManager.SpawnPointID = targetSpawnID;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
