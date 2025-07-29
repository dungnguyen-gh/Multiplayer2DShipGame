using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI messageText;

    public void SetMessage(string playerName, string message)
    {
        if (nameText != null) nameText.text = playerName;
        if (messageText != null) messageText.text = message;
    }
}
