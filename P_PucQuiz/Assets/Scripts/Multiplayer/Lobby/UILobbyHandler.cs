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
        QuizLobby.Instance.OnJoinedLobbyUI+=UpdateLobbyUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateLobbyUI(object sender, EventArgs e)
    {
        var _lobby = QuizLobby.Instance.GetJoinedLobby();

 
        Hide();
        int playerCount = _lobby.Players.Count;
        _playerIndex = _playerIndex == -1 ? playerCount : _playerIndex;
        ClearLobby();
        //roomCodeText.text = $"Room Code: {QuizLobby.Instance.GetJoinedLobby().LobbyCode}";
        int _index = 0;

        foreach (var player in _lobby.Players)
        {
            if (_index != 0)
            {
                var lobbyPlayer = Instantiate(lobbyPlayerPrefab, container.gameObject.transform, true);
                LobbyPlayerUI lobbyPlayerUI = lobbyPlayer.GetComponent<LobbyPlayerUI>();
                lobbyPlayerUI.UpdatePlayer(player);
            }
            _index++;
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

    private void UpdateNamesPosition()
    {
        
    }
}
