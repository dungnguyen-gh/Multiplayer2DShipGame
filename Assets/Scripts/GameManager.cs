using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    // server-only storage of scores
    private Dictionary<string, int> scores = new Dictionary<string, int>();

    public override void OnStartServer()
    {
        Debug.Log("GameManager: OnStartServer called");
        instance = this;
        if (scores == null)
        {
            Debug.LogError("GameManager: scores dictionary is null on server");
        }
    }

    // called whenever a new players name becomes known on the server
    [Server]
    public void AddPlayer(string playerName)
    {
        Debug.Log($"GameManager.AddPlayer called with {playerName}");
        if (!scores.ContainsKey(playerName))
        {
            scores[playerName] = 0;
            Debug.Log($"GameManager: Added player {playerName} with score 0");
            SendScoreBoardToAllClients();
        }
        else
        {
            Debug.LogWarning($"GameManager: Player {playerName} already exists");
        }
    }
    [Server]
    private void SendScoreBoardToAllClients()
    {
        List<PlayerScore> playerScores = new List<PlayerScore>();
        foreach (var kvp in scores)
        {
            playerScores.Add(new PlayerScore { name = kvp.Key, score = kvp.Value });
        }
        Debug.Log($"GameManager: Sending scoreboard with {playerScores.Count} players to client");
        RpcUpdateScoreboard(playerScores.ToArray());
    }

    [ClientRpc]
    private void RpcUpdateScoreboard(PlayerScore[] scoreboard)
    {
        Debug.Log($"GameManager: RpcUpdateScoreboard called with {scoreboard.Length} players");
        if (ScoreBoardUIManager.instance != null)
        {
            ScoreBoardUIManager.instance.UpdateScoreboard(scoreboard);
        }
        else
        {
            Debug.LogError("GameManager: ScoreBoardUIManager.instance is null in RpcUpdateScoreboard");
        }
    }
    // called whenever someone scores a kill
    [Server]
    public void RegisterKill(string playerName)
    {
        if (!scores.ContainsKey(playerName))
        {
            Debug.LogWarning($"Player {playerName} not found in scores");
            return;
        }
        scores[playerName]++;
        Debug.Log($"Player {playerName} scored a kill, new score: {scores[playerName]}");
        RpcUpdateScore(playerName, scores[playerName]);
    }

    // reset score on death
    [Server]
    public void ResetPlayerScore(string playerName)
    {
        if (!scores.ContainsKey(playerName))
        {
            return;
        }
        scores[playerName] = 0;
        RpcUpdateScore(playerName, 0);
    }

    // RPC to all clients - add a new row
    [ClientRpc]
    void RpcAddPlayer(string playerName)
    {
        Debug.Log($"GameManager: Client received RpcAddPlayer for {playerName}");
        if (ScoreBoardUIManager.instance != null)
        {
            ScoreBoardUIManager.instance.AddPlayer(playerName);
        }
        else
        {
            Debug.LogError("GameManager: ScoreBoardUIManager.instance is null on client");
        }
    }

    // RPC to all clients - update one rows score
    [ClientRpc]
    void RpcUpdateScore(string playerName, int newScore)
    {
        if (ScoreBoardUIManager.instance != null)
        {
            Debug.Log($"RpcUpdateScore called for {playerName} with score {newScore}");
            ScoreBoardUIManager.instance.UpdateScore(playerName, newScore);
        }
        else
        {
            Debug.LogWarning("ScoreBoardUIManager.instance is null");
        }
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
        if (ScoreBoardUIManager.instance != null)
        {
            ScoreBoardUIManager.instance.RemovePlayer(playerName);
        }
    }

    // Called by the server to send the current scoreboard to a specific client
    [TargetRpc]
    public void TargetSendFullBoard(NetworkConnection target, PlayerScore[] allScores)
    {
        Debug.Log("TargetSendFullBoard called for client");
        StartCoroutine(WaitAndSendFullBoard(allScores));
    }

    // Coroutine to wait until UI is ready and display the scoreboard
    private IEnumerator WaitAndSendFullBoard(PlayerScore[] allScores)
    {
        Debug.Log("Waiting for ScoreBoardUIManager...");
        yield return new WaitUntil(() => ScoreBoardUIManager.instance != null);
        Debug.Log($"ScoreBoardUIManager ready, sending scoreboard with {allScores.Length} players");

        ScoreBoardUIManager.instance.ClearAllRows();

        foreach (var s in allScores)
        {
            Debug.Log($"GameManager: Adding player {s.name} with score {s.score} to client scoreboard");
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
        Debug.Log($"GameManager: Sending scoreboard with {list.Count} players to client");
        TargetSendFullBoard(conn, list.ToArray());
    }
}

[System.Serializable]
public struct PlayerScore
{
    public string name;
    public int score;
}
