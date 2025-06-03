using UnityEngine;

public class ConnectionApprovalManager : MonoBehaviour
{
    public static void AproveConnection(Unity.Netcode.NetworkManager.ConnectionApprovalRequest request, Unity.Netcode.NetworkManager.ConnectionApprovalResponse response)
    {
        string playerName = "John Doe";

        if (request.Payload == null || request.Payload.Length <= 0)
        {
            Debug.LogWarning($"Connection Request From {request.ClientNetworkId} with no name");
        }
        else
        {
            playerName = System.Text.UTF8Encoding.UTF8.GetString(request.Payload);
        }

        response.Approved = true;
        //response.CreatePlayerObject = true;
        response.Pending = false;
        response.Reason = string.Empty;

        if (response.Approved)
        {
            GameManager.Instance.AddConnectedPlayer(request.ClientNetworkId, playerName)
;        }


    } 
}
