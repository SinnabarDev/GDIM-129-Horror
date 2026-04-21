using UnityEngine;
using TMPro;

public class EnemyExorcistTyping : MonoBehaviour
{
[SerializeField] private GameObject textBoxUI;
[SerializeField] private TextMeshProUGUI wordText;
[SerializeField] private Enemy enemy;

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
}

void HandleTyping()
{
    foreach (char c in Input.inputString)
    {
        if (!char.IsLetter(c)) continue;

        char input = char.ToUpper(c);

        string word = enemy.GetWord();

        if (input == word[enemy.GetSavedProgress()])
        {
            enemy.SetSavedProgress(enemy.GetSavedProgress() + 1);

            if (enemy.GetSavedProgress() >= word.Length)
            {
                enemy.TriggerDisable();
            }
        }
        else
            {
                enemy.SetSavedProgress(0);
            }
    }

    UpdateText();
}

    void CheckInput(char input)
    {
        string word = enemy.GetWord();

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
        string word = enemy.GetWord();
        int index = enemy.GetSavedProgress();

        string done = word.Substring(0, index);
        string remain = word.Substring(index);

        wordText.text = "<color=RED>" + done + "</color>" + remain;
        wordText.gameObject.SetActive(true);
    }
}
