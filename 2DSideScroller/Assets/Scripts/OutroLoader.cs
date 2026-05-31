using UnityEngine;
using UnityEngine.SceneManagement;

public class OutroTrigger : MonoBehaviour
{
    [SerializeField]
    private string outroSceneName = "Outro";

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            SceneManager.LoadScene(outroSceneName);
        }
    }
}
