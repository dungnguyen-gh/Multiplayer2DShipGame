using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;
public class BackToLobby : MonoBehaviour
{
    [SerializeField] private Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        // stop the host
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        // stop the client
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }

        SceneManager.LoadScene("Lobby");
    }
}
