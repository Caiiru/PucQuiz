using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
public class QuizLobby : MonoBehaviour
{
    //CONST
    public const string KEY_PLAYER_NAME = "PlayerName";
    public const string KEY_PLAYER_POINTS = "PlayerPoints";
    public const string KEY_RELAY_JOINCODE = "RelayJoinCode";
    public const string KEY_ENCRYPTION_DLTS = "dkts";
    public const string KEY_ENCRYPTION_WSS = "wss";

    [Header("Join Code")]
    public string JoinCode;

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
    [HideInInspector] public EventHandler onUpdateLobbyUI;
    #endregion
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
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

        try
        {
            await UnityServices.InitializeAsync();
            if (AuthenticationService.Instance.IsSignedIn == false)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Debug.Log("Unity Services initialized and signed in!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Unity Services failed to initialize: {e.Message}");
        }

        _playerIndex = AuthenticationService.Instance.PlayerId;

        DeveloperConsole.Console.AddCommand("lobbyData", DisplayLobbyDataCommand);
        DeveloperConsole.Console.AddCommand("relayCode", DisplayRelayCodeCommand);


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

    public async Task<string> StartHostWithRelay(int maxConnections = 5, string _playerName = "null")
    {
        playerName = _playerName;
        await InitializeRelay();
        
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType: "wss"));
        _networkManager.GetComponent<UnityTransport>().UseWebSockets = true;
        isHost = true;
        return NetworkManager.Singleton.StartHost() ? JoinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string _joinCode, string _playerName)
    {
        playerName = _playerName;
        JoinCode = _joinCode;
        await InitializeRelay();
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

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);

            _networkManager.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType: "wss"));
            _networkManager.GetComponent<UnityTransport>().UseWebSockets = true;
            string relayJoinCode = await GetRelayJoinCode(allocation);

            DataObject relayDataObject = new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode);

            Dictionary<string, DataObject> _updateLobbyOptions = new()
            {
                { KEY_RELAY_JOINCODE, relayDataObject }
            };

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = _updateLobbyOptions
            });

            /*
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
            //Debug.Log($"Hosting Lobby: {_joinedLobby.LobbyCode} // Relay Code: {lobby.Data[KEY_RELAY_JOINCODE].Value}");

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


            if (lobby != null && await StartClientRelay(lobby.Data[KEY_RELAY_JOINCODE].Value, "wss"))
            {
                Debug.Log("Start Client Relay");
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

    public async Task<bool> StartClientRelay(string joinCode, string connectionType)
    {
        JoinCode = joinCode;
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
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


    public void DisplayLobbyDataCommand(string[] args)
    {

        foreach (var data in _joinedLobby.Data.Values)
        {

            DEV.Instance.DevPrint(data.Value.ToString());
        }
    }

    public void DisplayRelayCodeCommand(string[] args)
    {
        if (_joinedLobby == null)
        {
            DEV.Instance.DevPrint("Join a Lobby first");
            return;
        }


        DEV.Instance.DevPrint($"Relay Code:{_joinedLobby.Data[KEY_RELAY_JOINCODE].Value}");
    }
}


[System.Serializable]
public enum EncryptionType
{
    DTLS,
    WSS
}