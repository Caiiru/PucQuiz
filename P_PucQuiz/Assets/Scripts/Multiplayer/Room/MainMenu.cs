using System;
using Fusion;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private PlayerData playerDataPrefab;
    public String nickname;
    public String roomCode;

    [Space] [Header("Scenes")] 
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject waitingScreen;
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject startingScreen;
    [SerializeField] private GameObject gameScreen;

    [SerializeField] private String gameSceneName;
    
    private NetworkRunner _runnerInstance;
    private GameStateController _gameStateController;
    public void StartHost()
    {
        SetPlayerData();
        roomCode = GenerateRandomRoomCode(4);
        StartGame(GameMode.AutoHostOrClient,roomCode);

    }

    public void StartClient()
    {
        SetPlayerData();
        StartGame(GameMode.Client,roomCode.ToUpper());
    }

    private void SetPlayerData()
    {
        var playerData = FindAnyObjectByType<PlayerData>();
        if (playerData == null)
        {
            playerData = Instantiate(playerDataPrefab);
            Debug.Log("Creating player data");
        }
        

        if (string.IsNullOrWhiteSpace(nickname))
        {
            playerData.SetNickname(PlayerData.GetRandomNickName());
        }
        else
        {
            playerData.SetNickname(nickname);
        }
    }

    private async void StartGame(GameMode mode, string roomName)
    {
        _runnerInstance = FindAnyObjectByType<NetworkRunner>();
        _gameStateController = FindAnyObjectByType<GameStateController>();
        if (_runnerInstance == null) _runnerInstance = Instantiate(networkRunnerPrefab);

        _runnerInstance.ProvideInput = true;

        var startGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomName,
        };
        
        mainMenu.gameObject.SetActive(false);
        waitingScreen.gameObject.SetActive(true);
        waitingScreen.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
            mode == GameMode.Host || mode == GameMode.AutoHostOrClient ? "Creating Room..." : "Joining Room..."; 
        var result = await _runnerInstance.StartGame(startGameArgs);

        if (result.Ok)
        {
            Debug.Log(_runnerInstance.IsServer?"Server Started":"Joined to Server");
            waitingScreen.gameObject.SetActive(false);
            lobbyScreen.SetActive(true);
            lobbyScreen.GetComponent<LobbyScreenUI>().UpdateRoomCodeText($"Room Code: {roomCode}");
            /*
            var stateController = FindAnyObjectByType<GameStateController>();
            if(stateController!= null) stateController.UpdateLobbyDisplay();
            */

            if (_runnerInstance.IsServer)
            {
                //_runnerInstance.LoadScene(gameSceneName);
                //_gameStateController.ChangeState(GameStateController.GameStateEnum.Lobby);
            }
        }
        
    }

    public void SetRoomCode(String code) => roomCode = code;

    public void SetNickName(String nick) => nickname = nick;
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
