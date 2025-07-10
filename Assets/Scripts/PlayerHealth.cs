using TMPro;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnLivesChanged))]
    public int lives;

    private int maxLives = 3;

    public TextMeshProUGUI livesText;

    public override void OnStartServer()
    {
        var dmg = GetComponent<ReceiveDamage>();
        if (dmg != null)
            maxLives = dmg.GetMaxHealth();

        lives = maxLives;
    }

    public override void OnStartClient()
    {
        UpdateUI(); // always sync the UI on spawn
    }

    [Server]
    public void LoseLife()
    {
        lives = Mathf.Max(0, lives - 1);
        Debug.Log($"{name} lost a life. Lives left: {lives}");

        if (lives <= 0)
        {
            lives = maxLives; // reset lives on respawn
        }
    }

    void OnLivesChanged(int oldLives, int newLives)
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (livesText != null)
        {
            livesText.text = $"{lives}";
        }
    }
}
