using System;
using System.Collections;
using Multiplayer.Lobby;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class QuizPlayer : NetworkBehaviour
{

    [Header("Player Name")]

    public NetworkVariable<FixedString32Bytes> playerName = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public string PlayerName => playerName.Value.ToString();

    [Space]
    [Header("Points")]

    public NetworkVariable<int> points = new(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> alreadyAnswered = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //UI
    LobbyPlayerUI _lobbyPlayerUI;

    //Manager
    GameManager _gameManager;

    public void Start()
    {
        //DeveloperConsole.Console.AddCommand("playerAddPoints", AddPointsCommand);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _gameManager = FindAnyObjectByType<GameManager>();



        if (IsOwner)
        {
            playerName.Value = GameManager.Instance.LocalPlayerName;
            Debug.Log($"Sending to host...{OwnerClientId} - {playerName.Value}");

        }
    }

    IEnumerator WaitToUpdate()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Wait to ui update ended");
    }

    [Rpc(SendTo.Server)]
    void joinedToServerRpc(ulong clientId, string n)
    {

        Debug.Log($"Received from player {clientId}, his name: {n}");
    }
 
 


}
