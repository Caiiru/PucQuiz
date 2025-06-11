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
using Unity.Services.Lobbies.Models;
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
    [Header("Local Player Settings")]
    public string LocalPlayerName;
    public QuizPlayer LocalPlayer;


    [Header("Game Config")]
    private float timeToShowQuestion = 99f;
    private float timePerQuestion = 15f;
    private float timeToShowResults = 5f;



    // --- Variáveis de Rede ---
    #region Network Settings
    [Header("Network Settings")]
    public GameState CurrentGameState;
    public NetworkVariable<float> Timer = new NetworkVariable<float>(0f);

    public NetworkVariable<Question> CurrentQuestionData = new NetworkVariable<Question>();
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
    public EventHandler OnGameStateChanged;
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



        if (IsServer)
        {
            CurrentGameState = GameState.WaitingToStart;
            Timer.Value = 0;

        }
        if (!IsServer)
        {
            SendPlayerInfoToServerRpc(AuthenticationService.Instance.PlayerId, LobbyManager.Instance.LocalPlayerName);
        }
        CurrentGameState = GameState.WaitingToStart;

    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();


        if (Instance == this) Instance = null;

    }

    public void Update()
    {
        //ONLY SERVER
        if (!IsServer) return;


        Timer.Value = Timer.Value > 0 ? Timer.Value -= Time.deltaTime : 0;

        if (CurrentGameState == GameState.WaitingToStart) return;
        CheckGameState(CurrentGameState);
    }

    public void CheckGameState(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.DisplayingQuestion:
                if (Timer.Value <= 0)
                {
                    ChangeCurrentGameStateRPC(GameState.CollectingAnswers, timePerQuestion);
                     

                }

                break;
            case GameState.CollectingAnswers:
                if (Timer.Value <= 0)
                {
                    ChangeCurrentGameStateRPC(GameState.ShowingResults, timeToShowResults); 
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

                //CurrentGameState.Value = GameState.DisplayingQuestion;
                //Timer.Value = timeToShowQuestion;


                break;

            case GameState.GameOver:
                //tela de vitoria
                break;
        }
    }




    [Rpc(SendTo.Everyone)]
    public void StartQuizRpc()
    {
        LayoutManager.instance.StartQuiz();
        OnQuizStarted?.Invoke(this, null);
        Timer.Value = 3.5f;

    }
    [Rpc(SendTo.Everyone)]
    public void ChangeCurrentGameStateRPC(GameState newGameState, float _timer)
    {
        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(this, null);
        if (IsServer)
            Timer.Value = _timer;

        Debug.Log("New State:" + newGameState);
    }

    public NetworkList<QuizPlayerData> GetConnectedPlayers()
    {
        CheckList();
        return ConnectedPlayers;
    }

    #region Server Functions


    ///RECEIVE Player data on call to join
    ///Populate a list with connected players - ID: Authentication ID 
    ///Use this function to quickly receive player name and update lobby ui
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

    /// <summary>
    /// Receive player id and set a new score for this player.
    /// </summary>
    /// <param name="clientID">Authentication id of the player, this is save on Quiz Player class as ClientID </param>
    /// <param name="newScore">Players new score</param>
    public void SetPlayerScore(string clientID, int newScore)
    {
        //if (!IsServer) return; 

        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player.ClientId.Value == clientID)
            {
                player.Score.Value = newScore;
                return;
            }
        }
    }




    public QuizPlayer[] GetTop5Playes()
    {


        List<QuizPlayer> topPlayers = players.OrderByDescending(player => player).Take(players.Count > 5 ? 5 : players.Count).ToList();



        return topPlayers.ToArray();
    }

    public QuizPlayer GetPlayerByID(string playerID)
    {
        foreach (var player in players)
        {
            if (player.ClientId.Value == playerID)
            {
                return player;
            }
        }
        return null;
    }

    /// <summary>
    /// Add Player connected with name and authID to use on lobby
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="playerName"></param>
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

    public void AddPlayer(QuizPlayer quizPlayer)
    {
        //DEV.Instance.DevPrint($"Player add - {quizPlayer.PlayerName.Value.ToString()}");
        players.Add(quizPlayer);

    }

    public void AddCardToPlayer(string playerID, int cardID)
    {
        var player = GetPlayerByID(playerID);
        if (player == null)
        {
            DEV.Instance.DevPrint($"playerID not found: {playerID}");

            return;
        }

        var card = CardsManager.Instance.GetCardByID(cardID);
        if (card == null) return;



        player.AddCard(card);
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