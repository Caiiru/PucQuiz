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

    private void UpdateLobbyUI(object sender, QuizLobby.UpdateLobbyUIArgs e)
    {
        var lobby = QuizLobby.Instance.GetJoinedLobby();
        Hide();
        int playerCount = lobby.Players.Count;
        _playerIndex = _playerIndex == -1 ? playerCount : _playerIndex;
        ClearLobby();
        int index = 0;

        foreach (var player in lobby.Players)
        {
            if (index != 0)
            {
                var lobbyPlayer = Instantiate(lobbyPlayerPrefab, container.gameObject.transform, true);
                LobbyPlayerUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerUI>();
                lobbyPlayerUI.UpdatePlayer(player);
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
