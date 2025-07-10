using Mirror;
using TMPro;
using UnityEngine;

public class SyncNameTag : NetworkBehaviour
{
    // in hook, only on the server
    [SyncVar(hook = nameof(OnNameChanged))] 
    public string playerName = "";

    public TextMeshProUGUI nameText;

    public override void OnStartLocalPlayer()
    {
        string n = PlayerPrefs.GetString("playerName", "Player");
        CmdSetName(n);
    }

    [Command]
    void CmdSetName(string n)
    {
        playerName = n;
    }

    // whenever the server's copy of playerName changes, OnNameChanged runs
    void OnNameChanged(string oldName, string newName)
    {
        nameText.text = newName;
        if (isServer)
        {
            GameManager.instance.AddPlayer(newName);
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            nameText.text = playerName;
        }
    }
    public override void OnStopServer()
    {
        //base.OnStopServer();

        // this player is destroyed on server side
        GameManager.instance.RemovePlayer(playerName); 
    }
}
