using UnityEngine;
using System.Collections.Generic;
using PlayFlow;
using Newtonsoft.Json; // Add this import

public class PlayerStateUpdateExample : MonoBehaviour
{
    private PlayFlowLobbyManager lobbyManager;
    private float updateCounter = 0f;

    private void Start()
    {
        lobbyManager = FindObjectOfType<PlayFlowLobbyManager>();
        if (lobbyManager == null)
        {
            Debug.LogError("PlayFlowLobbyManager not found in the scene!");
        }
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var playerState = new Dictionary<string, object>
            {
                ["position"] = new Dictionary<string, float>
                {
                    ["x"] = transform.position.x,
                    ["y"] = transform.position.z
                },
                ["health"] = 100,
                ["score"] = updateCounter
            };

            // Use JsonConvert instead of JsonUtility
            Debug.Log($"Sending state update: {JsonConvert.SerializeObject(new { state = playerState })}");
            
            updateCounter += 10;

            bool success = await lobbyManager.SendPlayerStateUpdateAsync(playerState);
            if (success)
            {
                Debug.Log($"Successfully sent player state update for player {lobbyManager.GetPlayerId()}!");
                
                var lobby = lobbyManager.GetCurrentLobby();
                // Use JsonConvert for lobby serialization
                Debug.Log($"Current lobby: {JsonConvert.SerializeObject(lobby)}");

                if (lobby?.lobbyStateRealTime != null && 
                    lobby.lobbyStateRealTime.ContainsKey(lobbyManager.GetPlayerId()))
                {
                    // Use JsonConvert for state serialization
                    Debug.Log($"Current lobby state: {JsonConvert.SerializeObject(lobby.lobbyStateRealTime[lobbyManager.GetPlayerId()])}");
                }
                else
                {
                    Debug.LogWarning("State update succeeded but state not found in lobby!");
                }
            }
            else
            {
                Debug.LogWarning("Failed to send player state update!");
            }
        }
    }
}