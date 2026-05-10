using TMPro;
using UnityEngine;

public class EnemyExorcistTyping : MonoBehaviour
{
    [SerializeField]
    private GameObject textBoxUI;

    [SerializeField]
    private TextMeshProUGUI wordText;

    [SerializeField]
    private MonoBehaviour enemyBehaviour;

    private IExorcisable enemy;

    [SerializeField]
    private string enemyWord = "SHADE";

    public string GetWord() => enemyWord;

    private int currentIndex;

    void Awake()
    {
        enemy = enemyBehaviour as IExorcisable;

        if (enemy == null)
        {
            Debug.LogError("Assigned enemy does not implement IExorcisable!");
        }
    }

    void Update()
    {
        if (enemy == null)
            return;

        if (!enemy.IsStunned())
        {
            textBoxUI.SetActive(true);
            wordText.text = "";
            return;
        }

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
