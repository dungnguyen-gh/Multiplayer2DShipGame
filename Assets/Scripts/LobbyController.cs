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

        PlayerPrefs.SetString("playerName", nameInput.text);
        string address = addressInput.text;
        ushort port;

        if (!ushort.TryParse(portInput.text, out port))
        {
            SetWarningText("Invalid port number.");
            return;
        }

        NetworkManager.singleton.networkAddress = address;

        if (Transport.active is SimpleWebTransport webTransport)
        {
            webTransport.port = port;
            Debug.Log($"[Host] Using SimpleWebTransport on {address}:{port}");
        }
        else
        {
            SetWarningText("Transport is not SimpleWebTransport.");
            return;
        }

        warningText.text = $"Hosting on {address}:{port}";
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

        PlayerPrefs.SetString("playerName", nameInput.text);
        string address = addressInput.text;
        ushort port;

        if (!ushort.TryParse(portInput.text, out port))
        {
            SetWarningText("Invalid port number.");
            return;
        }

        NetworkManager.singleton.networkAddress = address;

        if (Transport.active is SimpleWebTransport webTransport)
        {
            webTransport.port = port;
            Debug.Log($"[Join] Connecting to {address}:{port} via SimpleWebTransport");
        }
        else
        {
            SetWarningText("Transport is not SimpleWebTransport.");
            return;
        }

        warningText.text = $"Connecting to {address}:{port}";

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
