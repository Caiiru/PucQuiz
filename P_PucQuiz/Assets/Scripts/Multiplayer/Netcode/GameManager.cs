using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using Unity.Collections;
using Unity.Netcode; 
using Unity.Services.Authentication; 
using UnityEngine; 

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
    public EventHandler OnQuizStarted;
    public EventHandler OnGameStateChanged;
    #endregion


    #region Singleton
    public static GameManager Instance;

    void Awake()
    { 
        Instance = this; 

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
            if(DEV.Instance.OnlineMode == OnlineMode.Relay)
                SendPlayerInfoToServerRpc(AuthenticationService.Instance.PlayerId, LobbyManager.Instance.LocalPlayerName);
            else
            {
                SendPlayerInfoToServerRpc(LobbyManager.Instance.playerID, LobbyManager.Instance.LocalPlayerName);
            }
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
    public void ChangeCurrentGameStateMessage(GameState newState, float _timer, string from)
    {
        Debug.Log($"From {from} im changin the state to {newState}");
        ChangeCurrentGameStateRPC(newState, _timer);
    }
    [Rpc(SendTo.Everyone)]
    public void ChangeCurrentGameStateRPC(GameState newGameState, float _timer)
    {
        if (IsServer)
            Timer.Value = _timer;
        CurrentGameState = newGameState;
        OnGameStateChanged?.Invoke(this, null);

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
        if(CurrentGameState == GameState.WaitingToStart)
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
    public void SetPlayerScore( int newScore)
    {
        //if (!IsServer) return; 

        AddPointsToLocalPLayer(newScore);
        return; 
    }




    public QuizPlayerData[] GetTop5Players()
    {
        
        //List<QuizPlayer> topPlayers = players.OrderByDescending(player => player).Take(players.Count > 5 ? 5 : players.Count).ToList();

        QuizPlayerData[] players = new QuizPlayerData[ConnectedPlayers.Count];
        //Debug.Log($"Players Inside top 5 count: {players.Count()}");

        for(int i = 0; i< players.Count(); i++)
        {
            players[i] = new QuizPlayerData()
            {
                ClientId = "",
                PlayerName="",
                Score=-99,

            };

        }

         
        foreach(var player in ConnectedPlayers)
        {
            if(player.Score < players[0].Score)
            {
                continue;
            }

            for(int i = players.Count(); i > 1; i--)
            {
                players[i] = players[i-1]; 
            }
            players[0] = player;

            //DEV.Instance.DevPrint($"{player.PlayerName} has {player.Score}");

        }



        return players;
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
    [Rpc(SendTo.Everyone)]
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

    internal void AddPointsToLocalPLayer(int points)
    {
        LocalPlayer.Score.Value = points;
        int _index = 0;
        foreach(var player in ConnectedPlayers)
        {
            if(player.ClientId == LocalPlayer.ClientId.Value)
            {
                QuizPlayerData playerData = new QuizPlayerData();
                playerData.ClientId = player.ClientId.Value;
                playerData.PlayerName = player.PlayerName;
                playerData.Score = points; 
                DEV.Instance.DevPrint($"Player {playerData.PlayerName} updated his points to {playerData.Score}");  
                UpdatePlayerInfoOnServerRpc(playerData);

            }
            _index++;
        }
    }

    [Rpc(SendTo.Server)]
    public void UpdatePlayerInfoOnServerRpc(QuizPlayerData playerData)
    {
        int _index = 0;
        foreach(var player in ConnectedPlayers)
        {
            if(player.ClientId == playerData.ClientId)
            {
                ConnectedPlayers[_index] = playerData;

                DEV.Instance.DevPrint($"Player Name {playerData.PlayerName.ToString()} has {playerData.Score} points updated on server");
                break;
            }
            _index++;
        }

        Debug.Log("Player was updated");
    }


    #endregion

}



#region Quiz Player Data
[Serializable]
public struct QuizPlayerData : INetworkSerializable, System.IEquatable<QuizPlayerData>
{
    public FixedString32Bytes ClientId;
    public FixedString32Bytes PlayerName;
    public int Score;
    public FixedString32Bytes cards;
    public bool Equals(QuizPlayerData other)
    {
        return ClientId == other.ClientId && PlayerName == other.PlayerName;
    }


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Score);
        serializer.SerializeValue(ref cards);
    }
    public override int GetHashCode()
    {
        return ClientId.GetHashCode() ^ PlayerName.GetHashCode();
    }
}

#endregion