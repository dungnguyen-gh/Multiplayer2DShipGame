using Mirror;
using UnityEngine;

public class ShootBullets : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    void Update()
    {
        if (!isLocalPlayer) return;
        if (MessageManager.instance.isInputFocus()) return; 
        if (Input.GetKeyDown(KeyCode.Space)) CmdShoot();
    }
    [Command]
    void CmdShoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // set shooter
        BulletMovement bulletMovement = bullet.GetComponent<BulletMovement>();
        bulletMovement.direction = Vector2.up;
        bulletMovement.shooterName = gameObject.GetComponent<SyncNameTag>().playerName;

        NetworkServer.Spawn(bullet);
        Destroy(bullet, 3f);
    }
}
