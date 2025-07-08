using UnityEngine;
using Mirror;

public class ScoreBoardRequest : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        // this runs on the client for the newly added player
        CmdRequestScoreBoard();
    }

    [Command]
    void CmdRequestScoreBoard(NetworkConnectionToClient sender = null)
    {
        GameManager.instance.TargetSendFullBoard(sender);
    }
}
