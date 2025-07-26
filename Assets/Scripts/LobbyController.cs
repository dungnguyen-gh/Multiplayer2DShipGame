using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using Mirror.SimpleWeb;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput, addressInput, portInput;
    [SerializeField] private Button hostBtn, joinBtn;

    [SerializeField] private TextMeshProUGUI warningText;

    void Start()
    {
        hostBtn.onClick.AddListener(OnHost);
        joinBtn.onClick.AddListener(OnJoin);
    }

    void OnHost()
    {
        if (!ValidateInputs()) return;

        // Trim inputs to remove accidental spaces
        string playerName = nameInput.text.Trim();
        string address = addressInput.text.Trim();
        string portText = portInput.text.Trim();

        PlayerPrefs.SetString("playerName", playerName);
        Debug.Log($"LobbyController: Saved player name '{playerName}' to PlayerPrefs");

        if (!ushort.TryParse(portText, out ushort port))
        {
            SetWarningText("Invalid port number.");
            return;
        }

        NetworkManager.singleton.networkAddress = address;

        if (Transport.active is SimpleWebTransport webTransport)
        {
            webTransport.port = port;
            webTransport.clientUseWss = true; // Ensure secure WebSocket for WebGL
            Debug.Log($"[Host] Hosting on wss://{address}:{port}");
        }
        else
        {
            SetWarningText("Transport is not SimpleWebTransport.");
            return;
        }

        SetWarningText($"Hosting on wss://{address}:{port}");
        NetworkManager.singleton.StartHost();
    }
    void OnJoin()
    {
        if (!ValidateInputs()) return;

        if (NetworkClient.active)
        {
            SetWarningText("Client already started.");
            return;
        }

        // Trim inputs
        string playerName = nameInput.text.Trim();
        string address = addressInput.text.Trim();
        string portText = portInput.text.Trim();

        PlayerPrefs.SetString("playerName", playerName);
        Debug.Log($"LobbyController: Saved player name '{playerName}' to PlayerPrefs");

        if (!ushort.TryParse(portText, out ushort port))
        {
            SetWarningText("Invalid port number.");
            return;
        }

        NetworkManager.singleton.networkAddress = address;

        if (Transport.active is SimpleWebTransport webTransport)
        {
            webTransport.port = port;
            webTransport.clientUseWss = true; // Required for WebGL secure connection
            Debug.Log($"[Join] Connecting to wss://{address}:{port}");
        }
        else
        {
            SetWarningText("Transport is not SimpleWebTransport.");
            return;
        }

        SetWarningText($"Connecting to wss://{address}:{port}");
        NetworkManager.singleton.StartClient();
    }
    bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text) ||
            string.IsNullOrWhiteSpace(addressInput.text) ||
            string.IsNullOrWhiteSpace(portInput.text))
        {
            SetWarningText("Please enter name, address, and port.");
            return false;
        }

        return true;
    }
    void SetWarningText(string message)
    {
        warningText.text = message;
    }
    private void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
