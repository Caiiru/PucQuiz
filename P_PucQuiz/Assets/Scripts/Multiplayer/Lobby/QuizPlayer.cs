using Multiplayer.Lobby;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine; 

public class QuizPlayer : NetworkBehaviour
{

    [Header("Player Name")]
    public NetworkVariable<FixedString32Bytes> playerName = new("",NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

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
        Debug.Log("Network Spawn");
        _gameManager = FindAnyObjectByType<GameManager>();



        if (IsOwner)
        {
            playerName.Value = QuizLobby.Instance.GetPlayerName();
            Debug.Log($"eu sou o jogador {OwnerClientId}, player quiz, name: {playerName.Value}");
        } 
        


    }
    public void AddPointsCommand(string[] args)
    {
        if (!IsOwner) return;
        points.Value = points.Value + int.Parse(args[0]);
        DEV.Instance.DevPrint($"my poits: {points.Value}");


        this.playerName.Value = QuizLobby.Instance.GetPlayerName();
        this.points.Value = 0;

        DEV.Instance.DevPrint($"Player Spawn:{playerName.Value} ");
        Debug.Log("Joined");
    }

    public void AddPoints(int amount)
    {
        if (!IsServer)
        {
            return;
        }
        points.Value += amount;

        Debug.Log($"Server: Player {playerName} ganhou {amount} pontos. Novos pontos: {points.Value}");

    }
 
 
}
