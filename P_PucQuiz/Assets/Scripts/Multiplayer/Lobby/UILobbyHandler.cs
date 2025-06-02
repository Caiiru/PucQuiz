using System;
using Multiplayer.Lobby;
using TMPro;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UIElements;

public class UILobbyHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab; 
    [SerializeField] private Transform container;

    [SerializeField] private int _playerIndex = -1;


    void Start()
    {
        QuizLobby.Instance.onUpdateLobbyUI += UpdateLobbyUI;
        GameManager.Instance.OnQuizStarted += QuizStarted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 

    private void UpdateLobbyUI(object sender, EventArgs e)
    {
        Hide(); 
        ClearLobby();
        int index = 0;
        return;
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (index != -1)
            {
                var lobbyPlayer = Instantiate(lobbyPlayerPrefab, container.gameObject.transform, true);
                LobbyPlayerUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerUI>();
                //lobbyPlayerUI.UpdatePlayer(player);
                lobbyPlayerUI.UpdatePlayerName(player.PlayerObject.GetComponent<QuizPlayer>().playerName.Value.ToString());
            }
            index++;
        }
        Show();
    }
    private void QuizStarted(object sender, EventArgs e) {
        Hide();
    }

    private void ClearLobby()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    private void Hide()
    {
        container.gameObject.SetActive(false);
    }

    private void Show()
    {
        container.gameObject.SetActive(true);
    }

}
