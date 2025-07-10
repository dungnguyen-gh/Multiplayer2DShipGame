using UnityEngine;
using Mirror;
using System.Collections;

public class ScoreBoardRequest : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        // this runs on the client for the newly added player
        //CmdRequestScoreBoard();

        StartCoroutine(WaitUntilReady());
    }

    IEnumerator WaitUntilReady()
    {
        yield return new WaitUntil(() => PlayerPrefs.HasKey("playerName") && ScoreBoardUIManager.instance != null);
        yield return null;
        CmdRequestScoreBoard();
    }

    [Command]
    void CmdRequestScoreBoard()
    {
        GameManager.instance.SendScoreBoardTo(connectionToClient);
    }
}
