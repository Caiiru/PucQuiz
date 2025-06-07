using System;
using System.Collections;
using Multiplayer.Lobby;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEditor.PackageManager;
using UnityEngine;

public class QuizPlayer : NetworkBehaviour, IEquatable<QuizPlayer>, IComparable<QuizPlayer>
{

    [Header("Player Name")]

    public NetworkVariable<FixedString32Bytes> PlayerName = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); 
    public NetworkVariable<FixedString32Bytes> ClientId = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Space]
    [Header("Points")]

    public NetworkVariable<int> Score = new(0, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<FixedString32Bytes> cards = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);   // Cards uid separated by , 


    public void Start()
    {
        //DeveloperConsole.Console.AddCommand("playerAddPoints", AddPointsCommand);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


        if (IsOwner)
        {
            PlayerName.Value = GameManager.Instance.LocalPlayerName;
            ClientId.Value = AuthenticationService.Instance.PlayerId;
            Debug.Log($"Sending to host...{ClientId.Value} - {PlayerName.Value}");
            //RegisterPlayerOnServerRPC(AuthenticationService.Instance.PlayerId, this);
        }
 
    } 

    public bool Equals(QuizPlayer other)
    {
        if (other == null) return false;
        QuizPlayer objAsPart = other as QuizPlayer;
        if (objAsPart == null) return false;
        else return Equals(objAsPart);
    }

    public int CompareTo(QuizPlayer other)
    {
        if (other.Score == null)
            return 1;

        else return this.Score.Value.CompareTo(other.Score.Value);
    }
 
}
