using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardUIManager : MonoBehaviour
{
    public static ScoreBoardUIManager instance;
    public GameObject scoreRowPrefab;
    public Transform contentParent;

    Dictionary<string, ScoreRowUI> playerRows = new();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        Debug.Log("ScoreBoardUIManager: Awake called");
    }
    public void UpdateScoreboard(PlayerScore[] scoreboard)
    {
        Debug.Log($"ScoreBoardUIManager: Received scoreboard with {scoreboard.Length} players");
        ClearAllRows(); // Remove existing rows

        foreach (var player in scoreboard)
        {
            AddPlayer(player.name, player.score);
        }
    }
    public void ClearAllRows()
    {
        foreach (var row in playerRows.Values)
        {
            Destroy(row.gameObject);
        }
        playerRows.Clear();
    }
    private void AddPlayer(string playerName, int score)
    {
        Debug.Log($"ScoreBoardUIManager: Adding player {playerName} with score {score}");
        GameObject row = Instantiate(scoreRowPrefab, contentParent);
        ScoreRowUI rowUI = row.GetComponent<ScoreRowUI>();
        if (rowUI != null)
        {
            rowUI.SetValues(playerName, score);
            playerRows[playerName] = rowUI;
            Debug.Log($"ScoreBoardUIManager: Added ScoreRowUI for {playerName} with score {score}");
        }
        else
        {
            Debug.LogError("ScoreBoardUIManager: ScoreRowUI component missing on prefab");
            Destroy(row);
        }
    }
    public void AddPlayer(string name)
    {
        Debug.Log($"ScoreBoardUIManager: Adding player {name}");
        if (playerRows.ContainsKey(name)) return;

        if (scoreRowPrefab == null || contentParent == null)
        {
            Debug.LogError("Cannot instantiate row: prefab or parent is null");
            return;
        }

        var rowGO = Instantiate(scoreRowPrefab, contentParent);
        var rowUI = rowGO.GetComponent<ScoreRowUI>();
        if (rowUI == null)
        {
            Debug.LogError("ScoreRowUI component not found on prefab");
            Destroy(rowGO);
            return;
        }
        rowUI.SetValues(name, 0);
        playerRows[name] = rowUI;
        SortRows();
    }

    public void UpdateScore(string name, int newScore)
    {
        if (!playerRows.TryGetValue(name, out var row))
        {
            Debug.LogWarning($"Player {name} not found in scoreboard to update score");
            return;
        }
        row.UpdateScore(newScore);
        SortRows();
    }
    public void RemovePlayer(string name)
    {
        if (!playerRows.TryGetValue(name, out var ui)) return;
        Destroy(ui.gameObject);
        playerRows.Remove(name);
        SortRows();
    }
    void SortRows()
    {
        var entries = new List<KeyValuePair<string, ScoreRowUI>>(playerRows);
        // descending by the numeric text
        entries.Sort((a, b) =>
            int.Parse(b.Value.scoreText.text)
            .CompareTo(int.Parse(a.Value.scoreText.text))
        );

        for (int i = 0; i < entries.Count; i++)
            entries[i].Value.transform.SetSiblingIndex(i);
    }
}
