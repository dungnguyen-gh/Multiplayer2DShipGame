using UnityEngine;
using TMPro;

public class ScoreRowUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void SetValues(string playerName, int score)
    {
        nameText.text = playerName;
        scoreText.text = score.ToString();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
