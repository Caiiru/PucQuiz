using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private TMPro.TMP_InputField joinInputField;
    [SerializeField] private TMPro.TextMeshProUGUI feedbackText;

    private String _gameSceneName;
    private String _waitingSceneName = "WaitingRoom";
    
    private NetworkRunner _networkRunnerInstance;
    public static RoomManager Instance { get; private set; }

    private Dictionary<int, string> playerNames = new();

    private String roomCode;


    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #endregion

    private async void Start()
    {

        _networkRunnerInstance = GetComponent<NetworkRunner>();
    }


    public async void CreateRoom()
    {
        roomCode = GenerateRandomRoomCode(4);
        
        Debug.Log("Starting network runner"); 

        var sceneLoadTask = _networkRunnerInstance.StartGame(new StartGameArgs()
        {
            SceneManager = _networkRunnerInstance.GetComponent<NetworkSceneManagerDefault>(),
            GameMode = GameMode.Host,
            SessionName = roomCode,
            PlayerCount = 10,
        });

        await sceneLoadTask;

        if (!sceneLoadTask.IsCompleted)
        {
            Debug.LogError($"Failed to start Network Runner:{sceneLoadTask.Result}");
            if (feedbackText != null) feedbackText.text = "Erro ao iniciar o Network Runner";
            return;
        }
        
        Debug.Log("Network Runner started successfully");
        Debug.Log($"Room created with code: {roomCode}");
        feedbackText.text = $"Room created with code: {roomCode}";
        _networkRunnerInstance.LoadScene(_waitingSceneName, LoadSceneMode.Single);
    }
    public async void JoinRoom()
    {
        if (string.IsNullOrEmpty(joinInputField.text))
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Por favor, insira o código da sala.";
            }
            return;
        }

        roomCode = joinInputField.text.ToUpper(); // Converter para maiúsculo para evitar problemas de case

        var joinGameResult = await _networkRunnerInstance.JoinSessionLobby(SessionLobby.ClientServer, roomCode);

        if (joinGameResult.Ok)
        {
            Debug.Log($"Joining room: {roomCode}");
            if (feedbackText != null)
            {
                feedbackText.text = $"Conectando à sala: {roomCode}";
            }
        }
        else
        {
            Debug.LogError($"Failed to join session {roomCode}: {joinGameResult.ErrorMessage}");
            if (feedbackText != null)
            {
                feedbackText.text = $"Falha ao conectar: {joinGameResult.ErrorMessage}";
            }
        }
    }

    public void SetPlayerName(int clientId, string name)
    {
        if (playerNames.TryAdd(clientId, name))
        {
            Debug.Log($"Player {clientId}, {name} was joined");
        }
    }

    public List<String> GetPlayerNames()
    {
        return playerNames.Values.ToList();
    }
 

    public void StartGame()
    {
        if (_networkRunnerInstance.IsServer)
        {
            _networkRunnerInstance.LoadScene(_gameSceneName, LoadSceneMode.Single);
        }
    }

    private void OnLoadDone(NetworkRunner runner)
    {
        Debug.Log($"Scene loaded: {runner.SceneManager.MainRunnerScene.name}");
        
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


    public NetworkRunner GetNetworkRunner()
    {
        return _networkRunnerInstance;
    }

    public string GetRoomCode()
    {
        return roomCode;
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        SetPlayerName(player.PlayerId,"Great name");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
}



