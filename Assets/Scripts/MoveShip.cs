using UnityEngine;
using Mirror;

public class MoveShip : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    private void Update()
    {
        if (!isLocalPlayer) return;

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate((Vector3)input * speed * Time.deltaTime);

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}