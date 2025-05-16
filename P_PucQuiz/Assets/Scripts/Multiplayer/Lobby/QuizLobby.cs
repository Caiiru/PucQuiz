using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements; 
public class QuizLobby : MonoBehaviour
{
    //CONST
    public const string KEY_PLAYER_NAME = "PlayerName";
    
    
    [Header("Players")] 
    [SerializeField] private Dictionary<int, String> players = new();

    [SerializeField] private String playerName;
    

    [Header("Lobby UI")] 
    [SerializeField] private GameObject lobbyObject;
    [SerializeField] private GameObject loadingObject;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private String lobbySceneName;
    

    private Lobby _hostLobby;
    private Lobby _joinedLobby;
    
    
    private float _hearbeatTimer;
    private float _lobbyUpdateTimer;
    private int _playersJoined = 0;
    
    
    [Header("References")] [SerializeField]
    private UIDocument doc;

    
    //SINGLETON
    public static QuizLobby Instance;

    [HideInInspector]
    public EventHandler<LobbyEventArgs> OnJoinedLobby;
 
    public class LobbyEventArgs : EventArgs
    {
        public Lobby lobby;
    }
 
    private void Awake()
    {
        if(Instance !=null)
            Destroy(this.gameObject);

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    async void Start()
    {
        
        lobbyObject.SetActive(true);
        loadingObject.SetActive(false);
        
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        FindDoc();
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
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs{lobby = _joinedLobby});
        }

    }

    private void ClickEnterButton(ClickEvent evt)
    {
        Debug.Log("Click Button");
        string roomCode = doc.rootVisualElement.Q<TextField>("Code").text;
        playerName = doc.rootVisualElement.Q<TextField>("Name").text;
        bool isHosting = string.IsNullOrEmpty(roomCode);
        Debug.Log(isHosting?"Hosting a new Room.. " : $"Joining a room {roomCode}");
        if(isHosting)
            CreateLobby();
        else
            JoinLobby(roomCode);
        
    }
    
    public async void CreateLobby()
    {
        try
        {
            string roomCode = GenerateRandomRoomCode(4);
            int maxPlayers = 4;
            
            lobbyObject.SetActive(false);
            loadingObject.SetActive(true);
            loadingText.text = "Creating Room...";
            Player player = GetPlayer();
            CreateLobbyOptions _options = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = player
            };
            
            Unity.Services.Lobbies.Models.Lobby lobby = 
                await LobbyService.Instance.CreateLobbyAsync(roomCode, maxPlayers,_options);
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;
            Debug.Log($"Room Code:{lobby.LobbyCode} / max players: {lobby.MaxPlayers}");
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs{lobby = _joinedLobby});
            SceneManager.LoadScene(lobbySceneName);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e); 
            loadingText.text = "Cannot Join"; 
            lobbyObject.SetActive(true);
            loadingObject.SetActive(false);
            FindDoc();
        }
    }

    private async void JoinLobby(string code)
    {
        lobbyObject.SetActive(false);
        loadingObject.SetActive(true);
        loadingText.text = "Joining room...";
        try
        {
            Player player = GetPlayer();
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = player
            };
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code,joinLobbyByCodeOptions);
            Debug.Log($"Joined Room {code}");
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs{lobby = _joinedLobby}); 
            SceneManager.LoadScene(lobbySceneName);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            loadingText.text = "Cannot Join"; 
            lobbyObject.SetActive(true);
            loadingObject.SetActive(false);
            FindDoc();
            
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

    private void FindDoc()
    {
        if (doc == null)
            doc = FindAnyObjectByType<UIDocument>();
        doc.rootVisualElement.Q<Button>(name:"Entrar").RegisterCallback<ClickEvent>(ClickEnterButton);
    }

    private Player GetPlayer()
    {
        PlayerDataObject playerData = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
        var data = new Dictionary<string, PlayerDataObject>();
        data.Add(KEY_PLAYER_NAME,playerData);
        return new Player(
            id: AuthenticationService.Instance.PlayerId,
            connectionInfo:null,
            data: data);
    }

    private async void UpdatePLayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        try
        {
            var _data = new Dictionary<string, PlayerDataObject>();
            _data.Add(KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName));
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId,
                new UpdatePlayerOptions()
                {
                    Data = _data
                });
            _joinedLobby = lobby;
            
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs{lobby = _joinedLobby});

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
