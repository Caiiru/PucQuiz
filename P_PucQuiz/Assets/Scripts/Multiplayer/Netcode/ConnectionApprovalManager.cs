using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class ConnectionApprovalManager : MonoBehaviour
{

    public struct ConnectionPayload
    {
        public string PlayerAuthID;
        public string PlayerName;
    }

    public static byte[] SerializeConnectionPayload(ConnectionPayload payload)
    {
        string json = JsonConvert.SerializeObject(payload);
        return UTF8Encoding.UTF8.GetBytes(json);
    }
    public static void AproveConnection(Unity.Netcode.NetworkManager.ConnectionApprovalRequest request, Unity.Netcode.NetworkManager.ConnectionApprovalResponse response)
    {
         
        if (request.Payload == null || request.Payload.Length <= 0)
        {
            Debug.LogWarning($"Connection Request From {request.ClientNetworkId} with no name");
            response.Approved = false;
            response.Reason = "Name Invalid or Not Authenticated";
            return;
        }

        string playerAuthID;
        string playerName;
        try
        {
            string jsonPayload = UTF8Encoding.UTF8.GetString(request.Payload);

            ConnectionPayload payload = JsonConvert.DeserializeObject<ConnectionPayload>(jsonPayload);

            playerAuthID = payload.PlayerAuthID;
            playerName = payload.PlayerName;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to deserialize connection payload for client:{request.ClientNetworkId} {e.Message}");
            response.Approved = false;
            response.Reason = "Name Invalid or Not Authenticated";
            response.Pending = false;
            response.CreatePlayerObject = false;
            return;
        } 

        //GameManager.Instance.AddConnectedPlayer(playerAuthID, playerName);

        response.Approved = true;
        //response.CreatePlayerObject = true;
        response.Pending = false;
        response.Reason = string.Empty;

    }
}
