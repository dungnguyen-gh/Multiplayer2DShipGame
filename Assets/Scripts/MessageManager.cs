using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private Button sendButton;
    [SerializeField] private ScrollRect messageScrollRect;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        sendButton.onClick.AddListener(OnSendButtonClicked);

        messageInput.onSubmit.AddListener(OnInputSubmit);
    }

    private void OnSendButtonClicked()
    {
        SendMessage();
    }
    private void OnInputSubmit(string text)
    {
        SendMessage();
    }
    private void SendMessage()
    {
        string message = messageInput.text.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            Debug.Log("MessageManager: Attempting to send: " + message);
            if (NetworkClient.localPlayer == null)
            {
                Debug.LogError("NetworkClient.localPlayer is null");
                return;
            }
            MessageSender localPlayer = NetworkClient.localPlayer.GetComponent<MessageSender>();
            if (localPlayer == null)
            {
                Debug.LogError("MessageSender component not found on local player");
                return;
            }
            localPlayer.SendMessage(message);
            messageInput.text = "";
            messageInput.ActivateInputField();
        }
    }
    public void AddMessage(string playerName, string message)
    {
        Debug.Log("MessageManager: Adding message from " + playerName + ": " + message);
        if (contentParent == null) { Debug.LogError("contentParent is null"); return; }
        if (messagePrefab == null) { Debug.LogError("messagePrefab is null"); return; }
        GameObject newMessage = Instantiate(messagePrefab, contentParent);
        MessageUI messageUI = newMessage.GetComponent<MessageUI>();
        if (messageUI != null)
        {
            Debug.Log("MessageManager: Setting message UI for " + playerName + ": " + message);
            messageUI.SetMessage(playerName, message);
        }
        else
        {
            Debug.LogError("MessageUI invalid");
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        
        if (messageScrollRect != null)
        {
            Debug.Log("MessageManager: Scrolling to bottom");
            messageScrollRect.verticalNormalizedPosition = 0f;
        }
        else
        {
            Debug.LogError("Message Rect invalid");
        }
    }

    public bool isInputFocus()
    {
        return messageInput.isFocused;
    }
}
