using UnityEngine;

public class PlayerSpawnLoader : MonoBehaviour
{
    void Start()
    {
        SpawnPoint[] points = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint point in points)
        {
            if (point.spawnID == SceneTransitionManager.SpawnPointID)
            {
                transform.position = point.transform.position;
                return;
            }
        }
    }
}
