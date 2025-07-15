using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    [SerializeField] private TextMeshProUGUI respawnText;

    private void Awake()
    {
        instance = this;
    }
    public void ShowMessage(string message)
    {
        if (respawnText != null)
        {
            respawnText.text = message;
            respawnText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }

}
