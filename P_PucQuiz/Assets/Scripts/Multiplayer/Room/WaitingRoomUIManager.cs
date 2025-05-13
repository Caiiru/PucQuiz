using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomUIManager : MonoBehaviour
{
    public static WaitingRoomUIManager Instance { get; private set; }

    public Button StartGameButton;
    public TMPro.TextMeshProUGUI playerListText;
    public TMPro.TextMeshProUGUI roomCodeText;

    private void Awake()
    {
        if(Instance!=null) Destroy((this.gameObject));

        Instance = this;
    }

    private void Start()
    {
        if (RoomManager.Instance == null)
            return;
        UpdatePlayerList(RoomManager.Instance.GetPlayerNames());
        NetworkRunner networkRunner = RoomManager.Instance.GetNetworkRunner();

        roomCodeText.text = $"Room Code:{RoomManager.Instance.GetRoomCode()}";
        if (networkRunner.IsClient)
        {
            Destroy(StartGameButton);
        }
        


    }
    
    public void UpdatePlayerList(List<String> players){
        if (playerListText != null)
        {
            playerListText.text = "Jogadores:\n";
            foreach (string name in players)
            {
                playerListText.text += $"- {name}\n";
            }
        }
    }
}
