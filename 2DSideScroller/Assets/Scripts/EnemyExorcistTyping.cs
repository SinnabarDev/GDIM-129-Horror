using TMPro;
using UnityEngine;

public class EnemyExorcistTyping : MonoBehaviour
{
    [SerializeField]
    private GameObject textBoxUI;

    [SerializeField]
    private TextMeshProUGUI wordText;

    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private string enemyWord = "SHADE";

    public string GetWord() => enemyWord;

    private int currentIndex;

    void Update()
    {
        if (!enemy.IsStunned())
        {
            // NOT stunned → show empty box or hide text
            textBoxUI.SetActive(true);
            wordText.text = "";

            return;
        }

        // STUNNED → show word + typing
        textBoxUI.SetActive(true);

        HandleTyping();
        Debug.Log(enemy.IsStunned());
    }

    void HandleTyping()
    {
        foreach (char c in Input.inputString)
        {
            if (!char.IsLetter(c))
                continue;

            char input = char.ToUpper(c);

            string word = GetWord();
            int progress = enemy.GetSavedProgress();

            // Prevent reading past word end
            if (progress >= word.Length)
                return;

            if (input == word[progress])
            {
                progress++;
                enemy.SetSavedProgress(progress);

                if (progress >= word.Length)
                {
                    enemy.TriggerDisable();
                    return;
                }
            }
            else
            {
                enemy.SetSavedProgress(0);
            }
        }

        UpdateText();
        Debug.Log("Progress: " + enemy.GetSavedProgress());
        Debug.Log("Input String: " + Input.inputString);
    }

    void CheckInput(char input)
    {
        string word = GetWord();

        if (input == word[currentIndex])
        {
            currentIndex++;

            enemy.SetSavedProgress(currentIndex);

            if (currentIndex >= word.Length)
            {
                enemy.TriggerDisable();
            }
        }
        else
        {
            currentIndex = 0;
            enemy.SetSavedProgress(0);
        }
    }

    void UpdateText()
    {
        string word = GetWord();
        int index = enemy.GetSavedProgress();

        string done = word.Substring(0, index);
        string remain = word.Substring(index);

        wordText.text = "<color=red>" + done + "</color>" + remain;
        wordText.gameObject.SetActive(true);
    }
}
