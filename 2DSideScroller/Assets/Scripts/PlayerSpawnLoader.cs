using UnityEngine;

public class PlayerSpawnLoader : MonoBehaviour
{
    public GameObject SpawnPointHolder;
    void Start()
    {
        GameController.OnPlayerDeath += PlayerRespawn;
    }

    //private void Update()
    //{
    //    // Debug Test
    //    if (Input.GetKeyDown(KeyCode.R)) {
    //        PlayerRespawn();
    //    }
    //}

    private void PlayerRespawn() {
        Debug.Log("Player Respawn Triggered");
        SpawnPoint[] points = SpawnPointHolder.GetComponentsInChildren<SpawnPoint>();

        foreach (SpawnPoint point in points)
        {
            if (point.spawnID == SceneTransitionManager.Instance.SpawnPointID)
            {
                transform.position = point.transform.position;
                return;
            }
        }
    }
}
