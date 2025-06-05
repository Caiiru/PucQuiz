using System;
using System.Collections;
using Multiplayer.Lobby;
using TMPro;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private Transform container;

    GameManager gameManager;


    void OnEnable()
    {

        gameManager = GameManager.Instance;
        gameManager.onJoiningGame += OnJoiningGame;
        gameManager.onPlayerJoined += OnPlayerJoined;
        //gameManager.playersConnected.OnListChanged += OnLobbyUIListChanged;
    }
    void OnDisable()
    {
        gameManager.onPlayerJoined -= OnPlayerJoined;
    }


    private void UpdateLobbyUI()
    {
        Hide();
        ClearLobby();
        foreach (var playerName in gameManager.GetConnectedPlayers())
        {
            var lobbyPlayer = Instantiate(lobbyPlayerPrefab, container.gameObject.transform, true);
            LobbyPlayerUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerUI>();
            lobbyPlayerUI.UpdatePlayerName(playerName.PlayerName.ToString());
        }
        Show();
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

    #region Events
    private void OnLobbyUIListChanged(NetworkListEvent<QuizPlayerData> changeEvent)
    {
        UpdateLobbyUI();
    }
    private void OnPlayerJoined(object sender, EventArgs e)
    {
        UpdateLobbyUI();
    }
    private void OnQuizStarted(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnJoiningGame(object sender, EventArgs e)
    {
        gameManager.onPlayerJoined += OnPlayerJoined;
        gameManager.OnQuizStarted += OnQuizStarted;
    }
    #endregion

}
