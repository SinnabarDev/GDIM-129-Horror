using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    //[HideInInspector]
    public string SpawnPointID;

    private void Awake()
    {
        // Destroy duplicate managers
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set singleton instance
        Instance = this;

        // Keep this object when changing scenes
        DontDestroyOnLoad(gameObject);
    }

    public void SetSpawnPoint(string spawnID)
    {
        SpawnPointID = spawnID;
    }
}
