using TMPro;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnLivesChanged))]
    public int lives = 3;

    public TextMeshProUGUI livesText;

    public override void OnStartLocalPlayer()
    {
        UpdateUI();
    }

    // Call this on the server when hit
    [Server]
    public void TakeDamage()
    {
        if (lives <= 0) return;

        lives--;

        if (lives <= 0)
        {
            // Handle player death
            Debug.Log($"{name} has died.");
            NetworkServer.Destroy(gameObject);
        }
    }

    void OnLivesChanged(int oldLives, int newLives)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {lives}";
        }
    }
}
