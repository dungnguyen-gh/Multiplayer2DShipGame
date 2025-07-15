using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;

public class ReceiveDamage : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SyncVar] private int currentHealth;
    [SerializeField] private string enemyTag;
    [SerializeField] private bool destroyOnDeath;


    private Vector2 initialPosition;
    private string lastShooter = "";
    private bool isInvincible = false;

    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private MoveShip controller;

    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private Material blinkMaterial;
    private Material originalMaterial;

    private void Start()
    {
        currentHealth = maxHealth;
        initialPosition = transform.position;

        if (sprite != null)
        {
            originalMaterial = sprite.material;
        }
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

            if (!isInvincible)
            {
                TakeDamage(1);

                NetworkServer.Destroy(collision.gameObject);
            }
        }
    }
    void TakeDamage(int amount)
    {
        if (!isServer) { return; }
        else
        {
            currentHealth -= amount;
            Debug.Log($"{gameObject.name} took {amount} damage, remaining: {currentHealth}");

            // notify PlayerHealth to lose a life
            var hp = GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.LoseLife();
            }

            if (currentHealth <= 0)
            {
                
                if (destroyOnDeath)
                {
                    SpawnExplosion();

                    if (!string.IsNullOrEmpty(lastShooter))
                    {
                        GameManager.instance.RegisterKill(lastShooter);
                    }
                    Destroy(gameObject);
                }
                else
                {
                    SpawnExplosion();
                    currentHealth = maxHealth;

 
                    StartCoroutine(DelayedRespawn());

                    var syncNameTag = GetComponent<SyncNameTag>();
                    if (syncNameTag != null)
                    {
                        GameManager.instance.ResetPlayerScore(syncNameTag.playerName);
                    }
                }
            }
        }
    }
    
    void SpawnExplosion()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(explosion);
        }
    }

    IEnumerator DelayedRespawn()
    {
        RpcDisableVisual();

        for (int i = 3; i >= 1; i--)
        {
            RpcUpdateRespawnText(connectionToClient, $"Respawning in {i}...");
            yield return new WaitForSeconds(1f);
        }

        RpcUpdateRespawnText(connectionToClient, "");
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        transform.position = initialPosition;

        RpcEnableVisual();

        var hp = GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.UpdateUI();
        }

        StartCoroutine(InvincibilityBlink());
    }
    IEnumerator InvincibilityBlink()
    {
        isInvincible = true;
        RpcSetMaterial(true);
        yield return new WaitForSeconds(3f);
        RpcSetMaterial(false);
        isInvincible = false;
    }
    [ClientRpc]
    void RpcSetMaterial(bool isBlinking)
    {
        if (sprite != null && blinkMaterial != null && originalMaterial != null)
            sprite.material =  isBlinking ? blinkMaterial : originalMaterial;
    }

    [ClientRpc]
    void RpcDisableVisual()
    {
        ToggleRpcVisual(false);
    }
    [ClientRpc]
    void RpcEnableVisual()
    {
        ToggleRpcVisual(true);
    }
    private void ToggleRpcVisual(bool isVisible)
    {
        if (sprite != null) sprite.enabled = isVisible;
        if (controller != null) controller.enabled = isVisible;
        if (col != null) col.enabled = isVisible;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isVisible);
        }
    }

    [TargetRpc]
    void RpcUpdateRespawnText(NetworkConnection target, string message)
    {
        HUDManager.instance?.ShowMessage(message);
    }
    
    [Server]
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
