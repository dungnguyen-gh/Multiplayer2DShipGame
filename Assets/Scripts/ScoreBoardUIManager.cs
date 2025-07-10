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
        instance = this;
    }
    public void ClearAllRows()
    {
        foreach (var row in playerRows.Values)
        {
            Destroy(row.gameObject);
        }
        playerRows.Clear();
    }
    public void AddPlayer(string name)
    {
        if (playerRows.ContainsKey(name)) return;

        var rowGO = Instantiate(scoreRowPrefab, contentParent);
        var rowUI = rowGO.GetComponent<ScoreRowUI>();
        rowUI.SetValues(name, 0);
        playerRows[name] = rowUI;
        SortRows();
    }

    public void UpdateScore(string name, int newScore)
    {
        if (!playerRows.TryGetValue(name, out var row)) return;
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
