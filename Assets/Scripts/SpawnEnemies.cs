using Mirror;
using UnityEngine;

public class SpawnEnemies : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1.5f;
    public override void OnStartServer()
    {
        InvokeRepeating("Spawn", 1f, spawnInterval);
    }
    void Spawn()
    {
        Vector2 spawnPosition = new Vector2(Random.Range(-5.0f, 5.0f), transform.position.y);
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        NetworkServer.Spawn(enemy);
        Destroy(enemy, 10);
    }
}
