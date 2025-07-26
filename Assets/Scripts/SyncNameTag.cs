using Mirror;
using TMPro;
using UnityEngine;

public class SyncNameTag : NetworkBehaviour
{
    // in hook, only on the server
    [SyncVar(hook = nameof(OnNameChanged))] 
    public string playerName = "Name";

    public TextMeshProUGUI nameText;

    public override void OnStartLocalPlayer()
    {
        Debug.Log("SyncNameTag: OnStartLocalPlayer called for player ship");
        string n = PlayerPrefs.GetString("playerName", "Player");
        Debug.Log($"SyncNameTag: Setting name from PlayerPrefs: {n}");
        CmdSetName(n);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (nameText != null && !string.IsNullOrEmpty(playerName))
        {
            nameText.text = playerName;
        }
        if (isLocalPlayer)
        {
            CmdRequestScoreBoard();
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("SyncNameTag: OnStartServer called for player object");
    }

    [Command]
    void CmdSetName(string newName)
    {
        Debug.Log($"SyncNameTag: Server received CmdSetName with name: {newName} for {gameObject.name}");
        playerName = newName;
        Debug.Log($"SyncNameTag: playerName set to {newName} on server for {gameObject.name}");
        if (GameManager.instance != null)
        {
            Debug.Log("SyncNameTag: Manually calling GameManager.AddPlayer from CmdSetName");
            GameManager.instance.AddPlayer(newName);
        }
        else
        {
            Debug.LogError("SyncNameTag: GameManager.instance is null in CmdSetName");
        }
    }

    [Command]
    void CmdRequestScoreBoard()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SendScoreBoardTo(connectionToClient);
        }
        else
        {
            Debug.LogError("SyncNameTag: GameManager.instance is null in CmdRequestScoreBoard");
        }
    }

    // whenever the server's copy of playerName changes, OnNameChanged runs
    void OnNameChanged(string oldName, string newName)
    {
        Debug.Log($"SyncNameTag: OnNameChanged triggered for {gameObject.name}, oldName: {oldName}, newName: {newName}, isServer: {isServer}");

        if (nameText != null)
        {
            nameText.text = newName;
        }

        if (isServer)
        {
            if (GameManager.instance != null)
            {
                Debug.Log("SyncNameTag: Calling GameManager.AddPlayer from hook");
                GameManager.instance.AddPlayer(newName);
            }
            else
            {
                Debug.LogError("SyncNameTag: GameManager.instance is null on server in OnNameChanged");
            }
        }
    }
    public override void OnStopServer()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.RemovePlayer(playerName);
        }
    }
}
