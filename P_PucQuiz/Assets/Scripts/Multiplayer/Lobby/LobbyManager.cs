using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private NetworkManager _networkManager;

    public string LocalPlayerName;
    public string JoinCode;

    void Start()
    {
        _networkManager = NetworkManager.Singleton; 
    }
 

    public async Task<string> StartHostWithRelay(int maxConnections = 5, string _playerName = "null")
    {
        LocalPlayerName = _playerName;
        await InitializeAuth();

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType: "wss"));
        JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


        //Debug.Log($"My AuthID: {AuthenticationService.Instance.PlayerId}"); 
        _networkManager.GetComponent<UnityTransport>().UseWebSockets = true;
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalManager.AproveConnection; 
        return NetworkManager.Singleton.StartServer() ? JoinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string _joinCode, string _playerName)
    {
        LocalPlayerName = _playerName;
        JoinCode = _joinCode;
        await InitializeAuth();

        var connectionPayload = new ConnectionApprovalManager.ConnectionPayload()
        {
            PlayerAuthID = AuthenticationService.Instance.PlayerId,
            PlayerName = _playerName
        };

        NetworkManager.Singleton.NetworkConfig.ConnectionData = ConnectionApprovalManager.SerializeConnectionPayload(payload: connectionPayload); 
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: _joinCode);

            // Configure transport
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "wss"));

        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
        Debug.Log($"My AuthID: {AuthenticationService.Instance.PlayerId}");
        return !string.IsNullOrEmpty(_joinCode) && NetworkManager.Singleton.StartClient();
    }
    public async Task<bool> InitializeAuth()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        return true;
    }

    #region Singleton
    public static LobbyManager Instance;
 

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
    }

    #endregion
}
