using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Button hostBtn, joinBtn;
    void Start()
    {
        hostBtn.onClick.AddListener(OnHost);
        joinBtn.onClick.AddListener(OnJoin);
    }

    void OnHost()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text)) return;
        PlayerPrefs.SetString("playerName", nameInput.text);
        NetworkManager.singleton.StartHost();
    }
    void OnJoin()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text)) return;
        PlayerPrefs.SetString("playerName", nameInput.text);
        NetworkManager.singleton.networkAddress = "localhost";
        NetworkManager.singleton.StartClient();
    }
    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
