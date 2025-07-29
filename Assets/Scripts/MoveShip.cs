using UnityEngine;
using Mirror;

public class MoveShip : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;

    private Camera mainCamera;

    private void Start()
    {
        if (isLocalPlayer)
        {
            mainCamera = Camera.main;
        }
    }
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (MessageManager.instance.isInputFocus()) return; 
        
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate((Vector3)input * speed * Time.deltaTime);

        ClampPositionToScreen();
        
        
    }
    private void ClampPositionToScreen()
    {
        // get players current position
        Vector3 pos = transform.position;

        // convert viewport to world space
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        // padding to make sure the ship not hit the border
        float padding = 1.5f;

        pos.x = Mathf.Clamp(pos.x, bottomLeft.x + padding, topRight.x - padding);
        pos.y = Mathf.Clamp(pos.y, bottomLeft.y + padding, topRight.y - padding);

        transform.position = pos;
    }
}