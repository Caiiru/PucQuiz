using System;
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
    [Header("Lobby UI")] 
    [SerializeField] private GameObject lobbyObject;
    [SerializeField] private GameObject loadingObject;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private String lobbySceneName;
    

    private Unity.Services.Lobbies.Models.Lobby _hostLobby;
    private Unity.Services.Lobbies.Models.Lobby _joinedLobby;
    private float _hearbeatTimer;
    private float _lobbyUpdateTimer;
    private int _playersJoined = 0;
    
    
    [Header("References")] [SerializeField]
    private UIDocument doc;

    
    public static QuizLobby Instance;

    [HideInInspector]
    public UnityEvent playerJoined;

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

        if (doc == null)
            doc = FindAnyObjectByType<UIDocument>();
        doc.rootVisualElement.Q<Button>(name:"Entrar").RegisterCallback<ClickEvent>(ClickEnterButton);
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
            playerJoined?.Invoke();
            _playersJoined = _joinedLobby.Players.Count;
        }

    }

    private void ClickEnterButton(ClickEvent evt)
    {
        string roomCode = doc.rootVisualElement.Q<TextField>("Code").text;
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

            CreateLobbyOptions _options = new CreateLobbyOptions()
            {
                
            };
            
            Unity.Services.Lobbies.Models.Lobby lobby = 
                await LobbyService.Instance.CreateLobbyAsync(roomCode, maxPlayers);
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;
            Debug.Log($"Room Code:{lobby.LobbyCode} / max players: {lobby.MaxPlayers}");

            SceneManager.LoadScene(lobbySceneName, LoadSceneMode.Single);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobby(string code)
    {
        lobbyObject.SetActive(false);
        loadingObject.SetActive(true);
        loadingText.text = "Joining room...";
        try
        {
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code);
            
            Debug.Log($"Joined Room {code}");
            _hostLobby = lobby;
            _joinedLobby = _hostLobby;
            SceneManager.LoadScene(lobbySceneName, LoadSceneMode.Single);
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
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

}
