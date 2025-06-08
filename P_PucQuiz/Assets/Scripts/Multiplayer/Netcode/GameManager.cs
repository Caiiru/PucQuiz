using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeveloperConsole;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameState
{
    WaitingToStart,
    DisplayingQuestion,
    CollectingAnswers,
    ShowingResults,
    RoundOver,
    GameOver
}

public class GameManager : NetworkBehaviour
{
    [Header("Lobby Settings")]
    public string JoinCode;
    public string LocalPlayerName;
    public bool usingRelay = false;

    [Header("Game Config")]
    private float timeToShowQuestion = 99f;
    private float timePerQuestion = 15f;
    private float timeToShowResults = 5f;



    // --- Variáveis de Rede ---
    #region Network Settings
    [Header("Network Settings")]
    public NetworkVariable<GameState> CurrentGameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    public NetworkVariable<Question> CurrentQuestionData = new NetworkVariable<Question>();
    public NetworkVariable<float> Timer = new NetworkVariable<float>(0f);
    public NetworkVariable<int> CurrentQuestionNumber = new NetworkVariable<int>(0); // Número da pergunta atual na rodada  


    public NetworkList<QuizPlayerData> ConnectedPlayers = new();
    NetworkManager _networkManager;

    #endregion
     

    public List<QuizPlayer> players = new();


    //EVENTS

    #region Events Call
    public EventHandler OnUpdateUI;
    public EventHandler OnJoiningGame;
    public EventHandler OnQuizStarted;
    #endregion


    #region Singleton
    public static GameManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

    }
    #endregion

    void Start()
    {
        _networkManager = FindAnyObjectByType<NetworkManager>();
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CurrentGameState.OnValueChanged += OnGameStateChanged;



        if (IsServer)
        {
            CurrentGameState.Value = GameState.WaitingToStart;
            Timer.Value = 0;
            HandleGameStateChange(GameState.WaitingToStart, CurrentGameState.Value);
        }
        if (!IsServer)
        {
            SendPlayerInfoToServerRpc(AuthenticationService.Instance.PlayerId, LocalPlayerName);
        }

        //onPlayerJoined?.Invoke(this, null);

        //DEBUG 


    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        CurrentGameState.OnValueChanged -= OnGameStateChanged;

        if (Instance == this) Instance = null;

    }

    public void Update()
    {
        //ONLY SERVER
        if (!IsServer) return;


        Timer.Value = Timer.Value > 0 ? Timer.Value -= Time.deltaTime : 0;

        if (CurrentGameState.Value == GameState.WaitingToStart) return;
        CheckGameState(CurrentGameState.Value);
    }

    public void CheckGameState(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.DisplayingQuestion:
                if (Timer.Value <= 0)
                {
                    CurrentGameState.Value = GameState.CollectingAnswers;
                    Timer.Value = timePerQuestion;

                }

                break;
            case GameState.CollectingAnswers:


                if (Timer.Value <= 0)
                {
                    CurrentGameState.Value = GameState.ShowingResults;
                    Timer.Value = timeToShowResults;
                }
                break;

            case GameState.ShowingResults:

                if (Timer.Value > 0) break;

                //if (CurrentQuestionNumber.Value == _allQuestions.Count)
                //{
                //   CurrentGameState.Value = GameState.GameOver;
                //break;
                //}

                //CurrentQuestionNumber.Value++;
                //CurrentQuestionData.Value = _allQuestions[CurrentQuestionNumber.Value];

                CurrentGameState.Value = GameState.DisplayingQuestion;
                Timer.Value = timeToShowQuestion;


                break;

            case GameState.GameOver:
                //tela de vitoria
                break;
        }
    }
    public void OnGameStateChanged(GameState previousState, GameState newState)
    {
        HandleGameStateChange(previousState, newState);
    }

    private void HandleGameStateChange(GameState previousState, GameState newState)
    {
        DEV.Instance.DevPrint($"Gamestate was changed from {previousState} to {newState}");
    }



    [Rpc(SendTo.Everyone)]
    public void StartQuizRpc()
    {
        //Event_PucQuiz.scene_actualy = "Quiz";
        //LayoutManager.instance.ChangeMenu("Quiz","Quiz"); 
        LayoutManager.instance.StartQuiz();
        OnQuizStarted?.Invoke(this, null);
        Timer.Value = timeToShowQuestion;
        CurrentGameState.Value = GameState.DisplayingQuestion;
    }


    #region Lobby Stuff
    public async Task<string> StartHostWithRelay(int maxConnections = 5, string _playerName = "null")
    {
        LocalPlayerName = _playerName;
        await InitializeAuth();

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType: "wss"));
        JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


        Debug.Log($"My AuthID: {AuthenticationService.Instance.PlayerId}"); 
        _networkManager.GetComponent<UnityTransport>().UseWebSockets = true;
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalManager.AproveConnection;
        return NetworkManager.Singleton.StartServer() ? JoinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string _joinCode, string _playerName)
    {
        LocalPlayerName = _playerName;
        JoinCode = _joinCode;
        await InitializeAuth();

        var connectionPayload = new ConnectionApprovalManager.ConnectionPayload()
        {
            PlayerAuthID = AuthenticationService.Instance.PlayerId,
            PlayerName = _playerName
        };

        NetworkManager.Singleton.NetworkConfig.ConnectionData = ConnectionApprovalManager.SerializeConnectionPayload(payload: connectionPayload);
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: _joinCode);
        // Configure transport
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "wss"));


        Debug.Log($"My AuthID: {AuthenticationService.Instance.PlayerId}");
        return !string.IsNullOrEmpty(_joinCode) && NetworkManager.Singleton.StartClient();
    }
    public async Task<bool> InitializeAuth()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        return true;
    }
    public NetworkList<QuizPlayerData> GetConnectedPlayers()
    {
        CheckList();
        return ConnectedPlayers;
    }
    #endregion

    #region Server Functions


    internal void AddConnectedPlayer(string clientNetworkId, string playerName)
    {
        if (!IsServer) return;


        foreach (var player in ConnectedPlayers)
        {
            if (player.ClientId == clientNetworkId)
            {
                return;
            }

        }

        ConnectedPlayers.Add(new QuizPlayerData()
        {
            ClientId = clientNetworkId,
            PlayerName = playerName,
            
        });
        UpdateLobbyRPC();

    }
    internal void RemoveConnectedPlayerByID(string clientNetworkId)
    {
        //if (!IsServer) return; 
        foreach (var player in ConnectedPlayers)
        {
            if (player.ClientId == clientNetworkId)
            {
                ConnectedPlayers.Remove(player);
                UpdateLobbyRPC();
                return;
            }

        }
    }
    internal void RemoveConnectedPlayerByName(string playerName)
    {
        foreach (var player in ConnectedPlayers)
        {
            if (player.PlayerName == playerName)
            {
                ConnectedPlayers.Remove(player);
                UpdateLobbyRPC();
                return;
            }

        }

    }

    public void SetPlayerScore(string clientID, int newScore)
    {
        if (!IsServer) return; 
        
        for(int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if(player.ClientId.Value == clientID)
            {
                player.Score.Value = newScore;
                return;
            }
        }
    }

    public void GivePointsToEveryoneCommand(string[] args)
    {
        foreach (var player in players)
        {
            SetPlayerScore(player.ClientId.Value.ToString(), int.Parse(args[0]));

            DEV.Instance.DevPrint($"{player.PlayerName.Value} has {player.Score.Value} points");
        }
    }

    

    public QuizPlayer[] GetTop5Playes()
    {
        

        List<QuizPlayer> topPlayers = players.OrderByDescending(player => player).Take(5).ToList();



        return topPlayers.ToArray();
    }


    [ServerRpc(RequireOwnership = false)]
    void SendPlayerInfoToServerRpc(string clientId, string playerName)
    {
        Debug.Log($"Adding {clientId}->{playerName} to connectedPlayers ");
        AddConnectedPlayer(clientId, playerName);
    }
    [Rpc(SendTo.Everyone)]
    void UpdateLobbyRPC()
    {
        OnUpdateUI?.Invoke(this, null);
    }

    internal void CheckList()
    {
        //if (!IsServer) return;

        ArrayList _playersList = new();
        foreach (var player in ConnectedPlayers)
        {
            if (_playersList.Contains(player.ClientId))
            {
                RemoveConnectedPlayerByID(player.ClientId.ToString());
            }
            _playersList.Add(player.ClientId);
        }

    }

    public void AddQuizPlayer(string playerId, QuizPlayer quizPlayer)
    {
        throw new NotImplementedException();
    }
    public void AddPlayer(QuizPlayer quizPlayer)
    {
        players.Add(quizPlayer);
    }


    #endregion

}



#region Quiz Player Data
[Serializable]
public struct QuizPlayerData : INetworkSerializable, System.IEquatable<QuizPlayerData>
{
    public FixedString32Bytes ClientId;
    public FixedString32Bytes PlayerName; 
    public bool Equals(QuizPlayerData other)
    {
        return ClientId == other.ClientId && PlayerName == other.PlayerName;
    }
 

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName); 
    }
    public override int GetHashCode()
    {
        return ClientId.GetHashCode() ^ PlayerName.GetHashCode();
    }
}

#endregion