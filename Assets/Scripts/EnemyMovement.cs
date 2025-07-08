using UnityEngine;
using Mirror;
public class EnemyMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 1f;

    void Update()
    {
        if (isServer)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }
}
