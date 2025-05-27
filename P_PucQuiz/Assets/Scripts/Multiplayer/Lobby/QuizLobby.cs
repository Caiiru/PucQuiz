using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
public class QuizLobby : MonoBehaviour
{
    //CONST
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_POINTS = "PlayerPoints";
    public const string KEY_RELAY_JOINCODE = "RelayJoinCode";
    public const string KEY_ENCRYPTION_DLTS = "dkts";
    public const string KEY_ENCRYPTION_WSS = "wss";

    [Header("Local")]

    [SerializeField] private String playerName;

    [SerializeField] private string _playerIndex;

    [SerializeField] private EncryptionType encryptionType = EncryptionType.WSS;

    [SerializeField] private bool isHost = false;
 


    [Header("Players")] 

    [SerializeField] private int maxPlayers = 4;

 


    private Lobby _hostLobby;
    private Lobby _joinedLobby;


    private float _hearbeatTimer;
    private float _lobbyUpdateTimer;
    private int _playersJoined = 0;

    NetworkManager _networkManager;
    private string _connectionType => encryptionType == EncryptionType.DTLS ? KEY_ENCRYPTION_DLTS : KEY_ENCRYPTION_WSS;
 
    //SINGLETON
    public static QuizLobby Instance;


    #region Events
    [HideInInspector] public EventHandler<LobbyEventArgs> onJoinedLobby;
    [HideInInspector] public EventHandler onJoiningLobby;
    [HideInInspector] public EventHandler <UpdateLobbyUIArgs> onUpdateLobbyUI; 
    #endregion
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public class UpdateLobbyUIArgs : EventArgs
    {
        public GameObject JoiningPlayer;
    }

    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    async void Start()
    {
        _networkManager = FindAnyObjectByType<NetworkManager>();

        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        _playerIndex = AuthenticationService.Instance.PlayerId;

        
    }


    // Update is called once per frame
    void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();

    }

    private async void HandleLobbyPollForUpdates()
    {
        if (_joinedLobby == null) return;

        _lobbyUpdateTimer -= Time.deltaTime;
        if (_lobbyUpdateTimer > 0) return;

        _lobbyUpdateTimer = 1.5f;
        var lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
        _joinedLobby = lobby;
        HandleConnections();
    }
    private async void HandleLobbyHeartbeat()
    {
        if (_hostLobby == null) return;

        _hearbeatTimer -= Time.deltaTime;
        if (_hearbeatTimer > 0) return;

        _hearbeatTimer = 15;
        await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);

    }

    private void HandleConnections()
    {
        if (_joinedLobby == null) return;
        if (_playersJoined != _joinedLobby.Players.Count)
        {
            _playersJoined = _joinedLobby.Players.Count;
            onJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });
        }

    }

     

    public async void CreateLobby(string userName)
    {
        playerName = userName;

        onJoiningLobby?.Invoke(this, null);
        try
        {
            string roomCode = GenerateRandomRoomCode(4);

            Player player = GetPlayer();
            CreateLobbyOptions _options = new CreateLobbyOptions()
            {
                IsPrivate = false,

                //Player = player
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(roomCode, maxPlayers, _options);
            /*
            Allocation _allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(_allocation);
            
            DataObject relayDataObject = new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode);
            Dictionary<string, DataObject> _updateLobbyOptions = new();
            _updateLobbyOptions.Add(KEY_RELAY_JOINCODE, relayDataObject);
            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = _updateLobbyOptions
            });
            
            JoinAllocation _joinAllocation = new JoinAllocation(_allocation);

            RelayServerData _relayServerData = new RelayServerData(allocation,_connectionType);
            _relayServerData.AllocationId = _allocation.AllocationId;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation:_allocation, _connectionType: _connectionType));
            NetworkManager.Singleton.StartHost();
            */


            _networkManager.StartServer();

            isHost = true;
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;
            onJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby }); 
            Debug.Log($"Hosting Lobby: { _joinedLobby.LobbyCode}");
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }

    public async void JoinLobby(string code, string userName)
    {
        playerName = userName;
        onJoiningLobby?.Invoke(this, null);
        try
        {
            Player player = GetPlayer();
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = player
            };
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCodeOptions);
            if (lobby != null)
            {
                _hostLobby = lobby;
                _joinedLobby = _hostLobby;
                _networkManager.StartClient(); 
                onJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby }); 
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e); 

            //SetScene(menuObject);
        }
    }



    public Lobby GetJoinedLobby()
    {
        return _joinedLobby;
    }

    private String GenerateRandomRoomCode(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string code = "";
        for (int i = 0; i < length; i++)
        {
            code += chars[UnityEngine.Random.Range(0, chars.Length)];
        }
        return code;
    }

     

    private Player GetPlayer()
    {
        PlayerDataObject playerData = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
        var data = new Dictionary<string, PlayerDataObject>();
        data.Add(KEY_PLAYER_NAME, playerData);
        return new Player(
            id: AuthenticationService.Instance.PlayerId,
            connectionInfo: null,
            data: data);
    }

    private async void UpdatePlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        try
        {
            var data = new Dictionary<string, PlayerDataObject>();
            data.Add(KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName));
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId,
                new UpdatePlayerOptions()
                {
                    Data = data
                });
            _joinedLobby = lobby;

            onJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = _joinedLobby });

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1); //exclude host
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to allocate relay: " + e.Message);
            return default;
        }
    }

    async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to get relay code: " + e.Message);
            return default;
        }
    }

    async Task<JoinAllocation> JoinRelay(string relayCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Failed to join relay: " + e.Message);
            return default;
        }
    }

    public void PrintPlayersCount()
    {
        Debug.Log(_joinedLobby.Players.Count);
    }
    public string GetMyIndex()
    {
        return _playerIndex;
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public List<Player> GetPlayers()
    {
        return _joinedLobby.Players;
    }

    public bool GetIsHost()
    {
        return isHost;
    }

    //TODO -> Remove this function later;
    public void UpdatePlayerPoints(int points)
    {
        foreach (var player in GetPlayers())
        {
            var _data = new Dictionary<string, PlayerDataObject>();
            _data.Add(KEY_PLAYER_POINTS, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, $"{points}"));
            _data.Add(KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, player.Data[KEY_PLAYER_NAME].Value));

            player.Data = _data;


        }
    }
}


[System.Serializable]
public enum EncryptionType
{
    DTLS,
    WSS
}