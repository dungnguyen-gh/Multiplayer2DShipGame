using UnityEngine;
using Mirror;
public class PrefabReferences : MonoBehaviour
{
    public GameObject ship;
    public GameObject bullet;
    public GameObject enemy;
    public GameObject explosion;
    public GameObject RowUI;

    public GameObject gameManager;
    public GameObject enemySpawner;

    void Awake()
    {
        NetworkManager.singleton.spawnPrefabs.Add(ship);
        NetworkManager.singleton.spawnPrefabs.Add(bullet);
        NetworkManager.singleton.spawnPrefabs.Add(enemy);
        NetworkManager.singleton.spawnPrefabs.Add(explosion);
        NetworkManager.singleton.spawnPrefabs.Add(gameManager);
        NetworkManager.singleton.spawnPrefabs.Add(enemySpawner);
    }
}
