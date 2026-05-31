using UnityEngine;

public class PlayerSpawnLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPointHolder;

    private void Start()
    {
        GameController.OnPlayerDeath += PlayerRespawn;

        MoveToSpawnPoint();
    }

    private void OnDestroy()
    {
        GameController.OnPlayerDeath -= PlayerRespawn;
    }

    private void PlayerRespawn()
    {
        Debug.Log("Player Respawn Triggered");
        MoveToSpawnPoint();
    }

    private void MoveToSpawnPoint()
    {
        if (spawnPointHolder == null)
        {
            Debug.LogError("Spawn Point Holder is not assigned.");
            return;
        }

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError("SceneTransitionManager not found.");
            return;
        }

        string targetID = SceneTransitionManager.Instance.SpawnPointID;

        if (string.IsNullOrEmpty(targetID))
        {
            Debug.Log("No SpawnPointID set. Using prefab position.");
            return;
        }

        SpawnPoint[] points = spawnPointHolder.GetComponentsInChildren<SpawnPoint>();

        foreach (SpawnPoint point in points)
        {
            if (point.spawnID == targetID)
            {
                transform.position = point.transform.position;
                Debug.Log($"Moved player to spawn point: {point.spawnID}");
                return;
            }
        }

        Debug.LogWarning($"No SpawnPoint found with ID '{targetID}'");
    }
}
