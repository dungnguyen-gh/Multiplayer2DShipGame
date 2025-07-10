using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public struct PlayerScore
    {
        public string name;
        public int score;
    }

    // server-only storage of scores
    private readonly Dictionary<string, int> scores = new();

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

    // reset score on death
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
    
    

    [Server]
    public void RemovePlayer(string playerName)
    {
        if (scores.Remove(playerName))
        {
            RpcRemovePlayer(playerName);
        }
    }

    [ClientRpc]
    void RpcRemovePlayer(string playerName) 
    { 
        ScoreBoardUIManager.instance.RemovePlayer(playerName);
    }

    // Called by the server to send the current scoreboard to a specific client
    [TargetRpc]
    public void TargetSendFullBoard(NetworkConnection target, PlayerScore[] allScores)
    {
        StartCoroutine(WaitAndSendFullBoard(allScores));
        Debug.Log("TargetSendFullBoard called on client.");
    }

    // Coroutine to wait until UI is ready and display the scoreboard
    private IEnumerator WaitAndSendFullBoard(PlayerScore[] allScores)
    {
        Debug.Log("Waiting for ScoreBoardUIManager...");
        yield return new WaitUntil(() => ScoreBoardUIManager.instance != null);
        Debug.Log("ScoreBoardUIManager ready, sending scoreboard.");

        ScoreBoardUIManager.instance.ClearAllRows();

        foreach (var s in allScores)
        {
            Debug.Log($"Adding player: {s.name} with score: {s.score}");
            ScoreBoardUIManager.instance.AddPlayer(s.name);
            ScoreBoardUIManager.instance.UpdateScore(s.name, s.score);
        }
    }

    // Called by the ScoreBoardRequest script (from a client)
    [Server]
    public void SendScoreBoardTo(NetworkConnection conn)
    {
        List<PlayerScore> list = new();
        foreach (var kv in scores)
        {
            list.Add(new PlayerScore { name = kv.Key, score = kv.Value });
        }

        TargetSendFullBoard(conn, list.ToArray());
    }
}
