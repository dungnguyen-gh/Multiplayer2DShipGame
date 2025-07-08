using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    // server - only storage of scores
    readonly Dictionary<string, int> scores = new Dictionary<string, int>();

    private void Awake()
    {
        instance = this;
    }

    // called whenever a new players name becomes known on the server
    [Server]
    public void AddPlayer(string playerName)
    {
        if (scores.ContainsKey(playerName)) return;
        scores[playerName] = 0;
        RpcAddPlayer(playerName);
    }

    // called whenever someone scores a kill
    [Server]
    public void RegisterKill(string playerName)
    {
        if (!scores.ContainsKey(playerName)) return;
        scores[playerName]++;
        RpcUpdateScore(playerName, scores[playerName]);
    }

    // optional: reset score on death
    [Server]
    public void ResetPlayerScore(string playerName)
    {
        if (!scores.ContainsKey(playerName)) return;
        scores[playerName] = 0;
        RpcUpdateScore(playerName, 0);
    }

    // RPC to all clients - add a new row
    [ClientRpc]
    void RpcAddPlayer(string playerName)
    {
        ScoreBoardUIManager.instance.AddPlayer(playerName);
    }

    // RPC to all clients - update one rows score
    [ClientRpc]
    void RpcUpdateScore(string playerName, int newScore)
    {
        ScoreBoardUIManager.instance.UpdateScore(playerName, newScore);
    }

    [TargetRpc]
    public void TargetSendFullBoard(NetworkConnection target)
    {
        foreach (var kv in scores)
        {
            ScoreBoardUIManager.instance.AddPlayer(kv.Key);
            ScoreBoardUIManager.instance.UpdateScore(kv.Key, kv.Value);
        }
    }
}
