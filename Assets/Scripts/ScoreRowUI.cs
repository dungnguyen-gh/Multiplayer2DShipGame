using UnityEngine;
using TMPro;

public class ScoreRowUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void SetValues(string playerName, int score)
    {
        if (nameText == null || scoreText == null)
        {
            Debug.LogError("nameText or scoreText is not assigned in ScoreRowUI");
            return;
        }
        nameText.text = playerName;
        scoreText.text = score.ToString();
        Debug.Log($"SetValues: {playerName} with score {score}");
    }

    public void UpdateScore(int score)
    {
        if (scoreText == null)
        {
            Debug.LogError("scoreText is not assigned in ScoreRowUI");
            return;
        }
        scoreText.text = score.ToString();
        Debug.Log($"UpdateScore: {score}");
    }
}
