using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject playAgainButton;

    private void Start()
    {
        if (playAgainButton != null)
        {
            playAgainButton.SetActive(false);
        }

        StartCoroutine(ShowButtonAfterDelay());
    }

    private IEnumerator ShowButtonAfterDelay()
    {
        yield return new WaitForSeconds(12f);

        if (playAgainButton != null)
        {
            playAgainButton.SetActive(true);
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Intro");
    }
}
