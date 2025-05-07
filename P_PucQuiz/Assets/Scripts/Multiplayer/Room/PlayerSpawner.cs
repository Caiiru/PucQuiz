using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class PlayerSpawner : MonoBehaviour,INetworkRunnerCallbacks
{
    public GameObject playerPrefab;
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RequestPlayerName(PlayerRef player, NetworkRunner runner)
    {
        if (player == runner.LocalPlayer)
        {
            Debug.Log("Host requested my name");
            string playerName = "";
            RPC_SendPlayerNameToserver(runner.LocalPlayer, playerName);
        }
    }
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    private void RPC_SendPlayerNameToserver(PlayerRef runnerLocalPlayer, string playerName)
    {
        FusionConnector.Instance.AddPlayerToList(runnerLocalPlayer,playerName);
    }


    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    { 
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
        if (player == runner.LocalPlayer)
        {
            var spawnedPlayer = runner.Spawn(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
            FusionConnector connector = GameObject.FindAnyObjectByType<FusionConnector>();
            if (connector == null)
            {
                Debug.LogError("Connector not found");
                return;
            }

            PlayerInfo playerInfo = spawnedPlayer.GetComponent<PlayerInfo>();
            string playerName = connector.LocalPlayerName;
            playerInfo.PlayerName = string.IsNullOrEmpty(playerName)? $"Player {spawnedPlayer.StateAuthority.PlayerId}":playerName;
            Debug.Log($"Player {playerName} Joined");
            FusionConnector.Instance.OnPlayerJoin(runner,playerInfo.PlayerName.ToString());
            
        }

        if (runner.IsServer)
        {
            RPC_RequestPlayerName(player,runner);
        }

        
        FusionConnector.Instance.OnPlayerJoin(runner);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player left: {runner.LocalPlayer.PlayerId}");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Game Shutdown");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log($"Connection request: {request}");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    { 
        
        Debug.Log($"Connection request failed: {reason}");
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
        Debug.Log("Connected on server");
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
