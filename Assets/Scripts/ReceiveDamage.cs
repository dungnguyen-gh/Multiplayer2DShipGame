using Mirror;
using UnityEngine;

public class ReceiveDamage : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SyncVar] private int currentHealth;
    [SerializeField] private string enemyTag;
    [SerializeField] private bool destroyOnDeath;
    private Vector2 initialPosition;
    private string lastShooter = "";

    private void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return; // Prevent clients from handling collision

        if (collision.CompareTag(enemyTag))
        {
            BulletMovement bullet = collision.GetComponent<BulletMovement>();
            if (bullet != null)
            {
                lastShooter = bullet.shooterName;
            }

            TakeDamage(1);
            NetworkServer.Destroy(collision.gameObject);
        }
    }
    void TakeDamage(int amount)
    {
        if (isServer)
        {
            currentHealth -= amount;
            Debug.Log($"{gameObject.name} took {amount} damage, remaining: {currentHealth}");

            if (currentHealth <= 0)
            {
                if (!string.IsNullOrEmpty(lastShooter))
                {
                    GameManager.instance.RegisterKill(lastShooter);
                }

                if (destroyOnDeath)
                {
                    Destroy(gameObject);
                }
                else
                {
                    currentHealth = maxHealth;
                    RpcRespawn();
                }

                SyncNameTag syncNameTag = GetComponent<SyncNameTag>();
                if (syncNameTag != null)
                {
                    GameManager.instance.ResetPlayerScore(syncNameTag.playerName);
                }
            }
        }
    }
    [ClientRpc]
    void RpcRespawn()
    {
        transform.position = initialPosition;
    }

    int Factorial(int n)
    {
        if (n <= 1)
        {
            return 1;
        }
        else
        {
            return n * Factorial(n - 1);
        }
    }
}
