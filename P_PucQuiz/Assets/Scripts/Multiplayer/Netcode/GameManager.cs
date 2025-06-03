using System;
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

    [Header("Game Config")]
    [SerializeField] private float timeToShowQuestion = 99f;
    [SerializeField] private float timePerQuestion = 15f;
    [SerializeField] private float timeToShowResults = 5f;

    // --- Variáveis de Rede ---
    [Header("Network Settings")]
    public NetworkVariable<GameState> CurrentGameState = new NetworkVariable<GameState>(GameState.WaitingToStart);
    public NetworkVariable<Question> CurrentQuestionData = new NetworkVariable<Question>();
    public NetworkVariable<float> Timer = new NetworkVariable<float>(0f);
    public NetworkVariable<int> CurrentQuestionNumber = new NetworkVariable<int>(0); // Número da pergunta atual na rodada  


    public NetworkList<QuizPlayerData> playersConnected = new();
    NetworkManager _networkManager;





    //EVENTS

    public EventHandler onPlayerJoined;

    public EventHandler onJoiningGame;
    public EventHandler OnQuizStarted;



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
        CurrentQuestionData.OnValueChanged += OnQuestionChanged;
        Timer.OnValueChanged += OnTimerChanged;
        playersConnected.OnListChanged += OnPlayersConnectedListChanged;

        /*
        var _player = Instantiate(playerPrefab);
        DEV.Instance.DevPrint($"Player: {_player.GetComponent<QuizPlayer>().playerName.Value} joined");
        if (IsOwner)
        {
            if (player == null)
            {
                player = _player.GetComponent<QuizPlayer>();
            }
        }
        if (IsServer)
        {
            if (IsOwner)
            {
                Destroy(_player);
            }
            else
            {
                players.Add(player.GetComponent<QuizPlayer>());
            }

            CurrentGameState.Value = GameState.WaitingToStart;
            Timer.Value = 0;

        }
        */

        if (IsServer)
        {
            CurrentGameState.Value = GameState.WaitingToStart;
            Timer.Value = 0;
        }
        if (IsOwner)
        {
            Debug.Log("Im owner inside gameManagers");
        }
        HandleGameStateChange(GameState.WaitingToStart, CurrentGameState.Value);
        HandleQuestionChange(default, CurrentQuestionData.Value);
        HandleTimerChange(0, Timer.Value);



        //DEBUG 


    }

    private void OnPlayersConnectedListChanged(NetworkListEvent<QuizPlayerData> changeEvent)
    { 
        
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        CurrentGameState.OnValueChanged -= OnGameStateChanged;
        CurrentQuestionData.OnValueChanged -= OnQuestionChanged;
        Timer.OnValueChanged -= OnTimerChanged;

        if (Instance == this) Instance = null;

    }

    public void Update()
    {
        //ONLY SERVER
        if (!IsServer) return;


        Timer.Value = Timer.Value > 0 ? Timer.Value -= Time.deltaTime : 0;

        CheckGameState(CurrentGameState.Value);
    }

    public void CheckGameState(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.WaitingToStart:

                break;
            case GameState.DisplayingQuestion:
                if (Timer.Value <= 0)
                {
                    CurrentGameState.Value = GameState.CollectingAnswers;
                    Timer.Value = timePerQuestion;

                }

                break;
            case GameState.CollectingAnswers:


                if (Timer.Value <= 0 || AllPlayersAnswered())
                {
                    Debug.Log("All players answered or time pass");
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

                CurrentQuestionNumber.Value++;
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

    public void OnQuestionChanged(Question previousQuestion, Question newQuestion)
    {
        HandleQuestionChange(previousQuestion, newQuestion);
    }
    private void HandleQuestionChange(Question previousQuestion, Question newQuestion)
    {
        //DEV.Instance.DevPrint($"New Question Loaded: {newQuestion.QuestionText}");
        //TODO -> UPDATE UI
    }

    public void OnTimerChanged(float previousValue, float newValue)
    {
        HandleTimerChange(previousValue, newValue);
    }
    private void HandleTimerChange(float previousValue, float newValue)
    {
        //UI COM TEMPO 
    }


    private bool AllPlayersAnswered()
    {
        if (!IsServer)
        {
            return false;
        }
        int playerCount = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.GetComponent<QuizPlayer>() != null && client.PlayerObject.GetComponent<QuizPlayer>().alreadyAnswered.Value)
            {

                playerCount++;
            }
        }

        return playerCount >= NetworkManager.Singleton.ConnectedClientsList.Count;
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
        await InitializeRelay();

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalManager.AproveConnection;
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType: "wss"));
        _networkManager.GetComponent<UnityTransport>().UseWebSockets = true;
        return NetworkManager.Singleton.StartHost() ? JoinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string _joinCode, string _playerName)
    {
        LocalPlayerName = _playerName;
        JoinCode = _joinCode;
        await InitializeRelay();
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(_playerName);
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: _joinCode);
        // Configure transport
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "wss"));


        return !string.IsNullOrEmpty(_joinCode) && NetworkManager.Singleton.StartClient();
    }
    public async Task<bool> InitializeRelay()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        return true;
    }

    internal void AddConnectedPlayer(ulong clientNetworkId, string playerName)
    {
        if (!IsServer) return;


        foreach (var player in playersConnected)
        {
            if (player.ClientId == clientNetworkId)
            {
                Debug.LogWarning($"Player {clientNetworkId} already in list");
                return;
            }

        }

        playersConnected.Add(new QuizPlayerData()
        {
            ClientId = clientNetworkId,
            PlayerName = playerName
        });
    }
}


public struct QuizPlayerData : INetworkSerializable, System.IEquatable<QuizPlayerData>
{
    public ulong ClientId;
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
    #endregion
}