using UnityEngine;
using Mirror;

public class MessageSender : NetworkBehaviour
{
    [SerializeField] private SyncNameTag syncNameTag;

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public void SendMessage(string message)
    {
        if (!string.IsNullOrEmpty(message) && isLocalPlayer)
        {
            if (syncNameTag != null)
            {
                Debug.Log("Client: Calling CmdSendMessage with: " + message);
                CmdSendMessage(message);
            }
            else
            {
                Debug.LogError("syncNameTag is null");
            }
        }
    }

    [Command]
    void CmdSendMessage(string message)
    {
        Debug.Log("Server: CmdSendMessage received: " + message);
        if (syncNameTag != null)
        {
            Debug.Log("Server: Calling RpcReceiveMessage with name: " + syncNameTag.playerName + " and message: " + message);
            RpcReceiveMessage(syncNameTag.playerName, message);
        }
        else
        {
            Debug.LogError("Server: syncNameTag is null");
        }
    }

    [ClientRpc]
    void RpcReceiveMessage(string senderName, string message)
    {
        // all clients receive the message
        Debug.Log("Client: RpcReceiveMessage from " + senderName + ": " + message);
        if (MessageManager.instance != null)
        {
            Debug.Log("Client: Calling AddMessage with sender: " + senderName + " and message: " + message);
            MessageManager.instance.AddMessage(senderName, message);
        }
        else
        {
            Debug.LogError("MessageManager.instance is null");
        }
    }
}
