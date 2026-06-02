using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnID;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);

        // Arrow showing which way the player will face
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.right * 2f);
    }
}
