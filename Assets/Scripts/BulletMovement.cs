using UnityEngine;
using Mirror;

public class BulletMovement : NetworkBehaviour
{
    [SyncVar] public Vector2 direction = Vector2.up;
    [SyncVar] public string shooterName;
    [SerializeField] float speed = 8f;

    void Update()
    {
        if (!isServer) { return; }

        transform.Translate(direction * speed * Time.deltaTime);
    }
}
