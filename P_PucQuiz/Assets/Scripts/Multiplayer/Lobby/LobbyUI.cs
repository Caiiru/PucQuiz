using System;
using System.Collections;
using DG.Tweening;
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
    

    [Header("Menu Background")]
    [SerializeField] private GameObject _backgroundBlocks; 

    [SerializeField] GameObject particlesMenu;


    // PRIVATE
    GameManager gameManager;


    void OnEnable()
    {

        gameManager = GameManager.Instance;
        gameManager.OnUpdateUI += OnUpdateUIRequested;
        //gameManager.onPlayerJoined += OnPlayerJoined;
        gameManager.ConnectedPlayers.OnListChanged += OnLobbyUIListChanged;
        gameManager.OnQuizStarted += OnQuizStarted;

        DeveloperConsole.Console.AddCommand("updateUI", UpdateUICommand);
    }

    private void OnLobbyUIListChanged(NetworkListEvent<QuizPlayerData> changeEvent)
    {
        UpdateLobbyUI();
    }

    void OnDisable()
    {
        // gameManager.onPlayerJoined -= OnPlayerJoined;
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
        particlesMenu.SetActive(false);
        //_backgroundBlocks.SetActive(false);
        _backgroundBlocks.transform.DOScale(140f, 2f);
    }

    private void Show()
    {
        container.gameObject.SetActive(true); 
        particlesMenu.SetActive(true);
        _backgroundBlocks.SetActive(true);
        _backgroundBlocks.transform.DOScale(108f, 2f);
    }

    #region Events 
    private void OnQuizStarted(object sender, EventArgs e)
    {
        Hide();
    }

    private void OnUpdateUIRequested(object sender, EventArgs e)
    {
        UpdateLobbyUI();
    }
    #endregion


    void UpdateUICommand(string[] args)
    {
        UpdateLobbyUI();
    }
}
