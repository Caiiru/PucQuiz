using System;
using System.Collections;
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
        //QuizLobby.Instance.onUpdateLobbyUI += UpdateLobbyUI;
        //GameManager.Instance.onPlayerJoined += UpdateLobbyUI;
        GameManager.Instance.playersConnected.OnListChanged += UpdateLobbyUI;
        GameManager.Instance.OnQuizStarted += QuizStarted;

        DeveloperConsole.Console.AddCommand("updateUI", UpdateUICommand);

    }

    private void UpdateLobbyUI(NetworkListEvent<QuizPlayerData> changeEvent)
    { 
        Hide();
        ClearLobby();

        Debug.Log($"Player count: {GameManager.Instance.playersConnected.Count}");
        foreach (var playerName in GameManager.Instance.playersConnected)
        {
            var lobbyPlayer = Instantiate(lobbyPlayerPrefab, container.gameObject.transform, true);
            LobbyPlayerUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerUI>();
            lobbyPlayerUI.UpdatePlayerName(playerName.PlayerName.ToString());
        } 
        Show();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateUICommand(string[] args)
    {
        StartCoroutine("UpdatePlayerNames");
    }

    void UpdateLobbyUI(object sender, EventArgs e)
    {
        Debug.Log("UpdateLobbyEvent");
        StartCoroutine("UpdatePlayerNames");
    }

    private IEnumerator UpdatePlayerNames()
    {
        yield return new WaitForSeconds(0.5f); 

        Hide();
        ClearLobby();
        /*
        Debug.Log("Update PLayer Names");
        foreach (var playerName in GameManager.Instance.networkPlayers.Values)
        {

            var lobbyPlayer = Instantiate(lobbyPlayerPrefab, container.gameObject.transform, true);
            LobbyPlayerUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerUI>(); 
            lobbyPlayerUI.UpdatePlayerName(playerName);
        } */
        Show();
    }
    private void QuizStarted(object sender, EventArgs e)
    {
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
