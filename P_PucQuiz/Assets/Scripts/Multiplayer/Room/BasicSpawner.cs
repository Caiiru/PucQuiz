using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;

    public TMPro.TextMeshProUGUI nameText;


    public NetworkPrefabRef playerPrefab;
    private String _userName;

    public Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new();
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HostButton()
    {
        _userName = nameText.text;
        StartGame(GameMode.Host);
    }

    public void JoinButton()
    {
        
        _userName = nameText.text;
        StartGame(GameMode.Client);
    }

    async void StartGame(GameMode mode)
    {
        
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,

            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

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
        if (runner.IsClient && runner.GameMode != GameMode.Host)
        {
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, new Vector3(0, 0, 0),Quaternion.identity, player);
            _spawnedPlayers.Add(player,networkPlayerObject);
            networkPlayerObject.gameObject.GetComponent<PlayerInfo>().PlayerName = _userName;
            Debug.Log($"Player Joined: {_userName}");
            return;
        }
        Debug.Log("Room started");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedPlayers.Remove(player);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Host was shutdown");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    { 
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    { 
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    { 
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    { 
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    { 
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    { 
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    { 
    }

    public void OnConnectedToServer(NetworkRunner runner)
    { 
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    { 
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    { 
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    { 
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    { 
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    { 
    }
}
